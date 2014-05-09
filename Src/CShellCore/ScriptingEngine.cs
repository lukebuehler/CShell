#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2013  Arnova Asset Management Ltd., Lukas Buhler
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CShellCore.CodeCompletion;
using Mono.CSharp;

using CShell.Code;
using CShell.Util;


namespace CShell
{
    public class EvaluatorResult
    {
        public string Input;
        public string SourceFile;

        public object Result;
        public bool HasResult;
        public bool InputComplete;
        public bool HasErrors;
        public string[] Errors;
        public bool HasWarnings;
        public string[] Warnings;
    }

    public class ScriptingEngine : IDisposable, ICSharpScriptProvider
    {
        private readonly Evaluator evaluator;
        private readonly CompilerContext context;
        private readonly TaskFactory taskFactory;
        private readonly CancellationTokenSource tokenSource;
        private readonly CSharpCompletion codeCompletion;

        public ScriptingEngine()
        {
            tokenSource = new CancellationTokenSource();
            taskFactory = new TaskFactory(tokenSource.Token);

            context = new CompilerContext(new Mono.CSharp.CompilerSettings(), new ConsoleReportPrinter());
            evaluator = new Evaluator(context);
            evaluator.InteractiveBaseClass = typeof(ScriptingInteractiveBase);
            evaluator.DescribeTypeExpressions = true;

            ScriptingInteractiveBase.Evaluator = evaluator;
            var errorStream = new GuiStream(TextType.Error, OnConsoleOutput);
            var guiOutput = new StreamWriter(errorStream);
            guiOutput.AutoFlush = true;
            Console.SetError(guiOutput);
            ScriptingInteractiveBase.Output = guiOutput;

            var stdoutStream = new GuiStream(TextType.Output, OnConsoleOutput);
            guiOutput = new StreamWriter(stdoutStream);
            guiOutput.AutoFlush = true;
            Console.SetOut(guiOutput);
            ScriptingInteractiveBase.Error = guiOutput;

            codeCompletion = new CSharpCompletion(this);

            Evaluate("using System; using System.Linq; using System.Collections; using System.Collections.Generic;");

            //init the code completion so that the first character typed is not delayed
            //var readOnlyDocument = new ReadOnlyDocument(new StringTextSource(""), "init.csx");
            //codeCompletion.GetCompletions(readOnlyDocument, 0);
        }

        public void Dispose()
        {
            tokenSource.Cancel();
        }

        #region Events
        public event EventHandler<ConsoleEventArgs> ConsoleOutput;
        private void OnConsoleOutput(string text, TextType textType)
        {
            var args = new ConsoleEventArgs() { Text = text, TextType = textType };
            OnConsoleOutput(args);
        }
        private void OnConsoleOutput(ConsoleEventArgs e)
        {
            EventHandler<ConsoleEventArgs> handler = ConsoleOutput;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<EvaluateStartedEventArgs> EvaluateStarted;
        private void OnEvaluateStarted(string input, string sourceFile)
        {
            var e = new EvaluateStartedEventArgs() {Input = input, SourceFile = sourceFile};
            EventHandler<EvaluateStartedEventArgs> handler = EvaluateStarted;
            if (handler != null) handler(this, e);
        }

        public event EventHandler<EvaluateCompletedEventArgs> EvaluateCompleted;
        private void OnEvaluateCompleted(EvaluateCompletedEventArgs e)
        {
            EventHandler<EvaluateCompletedEventArgs> handler = EvaluateCompleted;
            if (handler != null) handler(this, e);
        }
        #endregion

        #region Code Evaluation
        public Task<EvaluatorResult> EvaluateAsync(string input)
        {
            return EvaluateAsync(input, null);
        }

        public Task<EvaluatorResult> EvaluateAsync(string input, string sourceFile)
        {
            return EvaluateAsync(input, sourceFile, CancellationToken.None);
        }

        public Task<EvaluatorResult> EvaluateAsync(string input, string sourceFile, CancellationToken cancellationToken)
        {
            var task = taskFactory.StartNew(inputState =>
            {
                var result = Evaluate(input, sourceFile);
                return result;
            }, cancellationToken);
            return task;
        }

        public EvaluatorResult Evaluate(string input)
        {
            return Evaluate(input, null);
        }

        public EvaluatorResult Evaluate(string input, string sourceFile)
        {
            OnEvaluateStarted(input, sourceFile);

            var evalResult = new EvaluatorResult();
            evalResult.InputComplete = true;
            evalResult.Input = input;
            evalResult.SourceFile = sourceFile;
            evalResult.HasErrors = false;
            evalResult.HasResult = false;

            string partialCommand = null;
            object result = null;
            bool resultSet = false;
            var errorwriter = new StringWriter();

            var oldPrinter = context.Report.SetPrinter(new StreamReportPrinter(errorwriter));

            try
            {
                partialCommand = evaluator.Evaluate(input, out result, out resultSet);
            }
            catch (Exception e)
            {
                evalResult.HasErrors = true;
                evalResult.Errors = new[]{e.ToString()};
            }
            finally
            {
                context.Report.SetPrinter(oldPrinter);
            }

            //// Partial input
            if (partialCommand != null)
            {
                evalResult.InputComplete = false;
                evalResult.Input = partialCommand;
            }
            string errorMessage = errorwriter.ToString();
            if (errorMessage.Length > 0)
            {
                //split the errors according to lines and parse each line
                //check if its errors or only warnings!
                var messages = errorMessage.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var warnings = messages.Where(msg => IsMessage(msg, "warning")).ToList();
                var errors = messages.Where(msg => IsMessage(msg, "error")).ToList();

                evalResult.HasWarnings = warnings.Any();
                evalResult.Warnings = warnings.ToArray();

                var allErrors = evalResult.Errors ?? new string[0];
                allErrors = allErrors.Concat(errors).ToArray();
                evalResult.HasErrors = allErrors.Any();
                evalResult.Errors = allErrors;
            }

            if (resultSet)
            {
                evalResult.HasResult = true;
                evalResult.Result = result;
            }
            
            //process the input for code completion later on
            if(!evalResult.HasErrors)
                codeCompletion.ProcessInput(input, sourceFile);

            OnEvaluateCompleted(new EvaluateCompletedEventArgs(evalResult, null, false, null));
            return evalResult;
        }

        private static bool IsMessage(string message, string messageType)
        {
            //the messages have following format
            // (2,1): error (123): Bla bla bla
            var messageParts = message.Split(':');
            if (messageParts.Length >= 2)
            {
                var type = messageParts[1].Trim();
                return type.StartsWith(messageType, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public void Interrupt()
        {
            evaluator.Interrupt();
        }
        #endregion

        #region Assembly References
        public void ReferenceAssembly(Assembly assembly)
        {
            evaluator.ReferenceAssembly(assembly);
            codeCompletion.AddAssembly(assembly.Location);
        }

        public void ReferenceAssembly(string file)
        {
            evaluator.LoadAssembly(file);
            codeCompletion.AddAssembly(file);
        }
        #endregion

        #region Code Completion
        public CSharpCompletion CodeCompletion
        {
            get { return codeCompletion; }
        }


        public string GetUsing()
        {
            return evaluator.GetUsing();
        }

        public string GetVars()
        {
            //create the variables
            var variables = evaluator.GetVars().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(variable => variable.Split('='))
                .Where(variableParts => variableParts.Length > 1)
                .Select(variableParts => variableParts[0] + ";");
            var variablesString = String.Join(Environment.NewLine, variables) + Environment.NewLine;
            return variablesString;
        }
        #endregion

    }
}
