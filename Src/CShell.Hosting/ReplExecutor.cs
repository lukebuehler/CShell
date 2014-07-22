using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using Common.Logging;
using CShell.Completion;
using CShell.Framework.Services;
using ScriptCs;
using ScriptCs.Contracts;

namespace CShell.Hosting
{
    public class ReplExecutor : ScriptExecutor, IReplExecutor
    {
        private readonly IRepl repl;
        private readonly IObjectSerializer serializer;
        private readonly IEnumerable<IReplCommand> replCommands;

        public ReplExecutor(
            IRepl repl,
            IObjectSerializer serializer,
            IFileSystem fileSystem,
            IFilePreProcessor filePreProcessor,
            IScriptEngine scriptEngine,
            ILog logger,
            IEnumerable<IReplCommand> replCommands)
            : base(fileSystem, filePreProcessor, scriptEngine, logger)
        {
            this.repl = repl;
            this.serializer = serializer;
            this.replCommands = replCommands;

            replCompletion = new CSharpCompletion(true);
            replCompletion.AddReferences(GetReferencesAsPaths());

            //since it's quite expensive to initialize the "System." references we clone the REPL code completion
            documentCompletion = replCompletion.Clone();
        }

        public string WorkspaceDirectory { get { return base.FileSystem.CurrentDirectory; } }

        public event EventHandler<EventArgs> AssemblyReferencesChanged;
        protected virtual void OnAssemblyReferencesChanged()
        {
            EventHandler<EventArgs> handler = AssemblyReferencesChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private readonly ICompletion replCompletion;
        private readonly ICompletion documentCompletion;

        public ICompletion ReplCompletion
        {
            get { return replCompletion; }
        }

        public ICompletion DocumentCompletion
        {
            get { return documentCompletion; }
        }

        public IEnumerable<IReplCommand> ReplCommands
        {
            get { return replCommands; }
        } 

        public override ScriptResult Execute(string script, params string[] scriptArgs)
        {
            var result = new ScriptResult();
            repl.EvaluateStarted(script, null);

            try
            {
                if (script.StartsWith(":"))
                {
                    var tokens = script.Split(' ');
                    if (tokens[0].Length > 1)
                    {
                        var command = replCommands.FirstOrDefault(x => x.CommandName == tokens[0].Substring(1));

                        if (command != null)
                        {
                            var argsToPass = new List<object>();
                            foreach (var argument in tokens.Skip(1))
                            {
                                var argumentResult = ScriptEngine.Execute(argument, scriptArgs, References, Namespaces, ScriptPackSession);

                                if (argumentResult.CompileExceptionInfo != null)
                                {
                                    throw new Exception(
                                        GetInvalidCommandArgumentMessage(argument),
                                        argumentResult.CompileExceptionInfo.SourceException);
                                }

                                if (argumentResult.ExecuteExceptionInfo != null)
                                {
                                    throw new Exception(
                                        GetInvalidCommandArgumentMessage(argument),
                                        argumentResult.ExecuteExceptionInfo.SourceException);
                                }

                                if (!argumentResult.IsCompleteSubmission)
                                {
                                    throw new Exception(GetInvalidCommandArgumentMessage(argument));
                                }

                                argsToPass.Add(argumentResult.ReturnValue);
                            }

                            var commandResult = command.Execute(this, argsToPass.ToArray());
                            if (commandResult is ScriptResult)
                                result = commandResult as ScriptResult;
                            else
                                result = new ScriptResult(commandResult);
                        }
                        else
                        {
                            throw new Exception("Command not found: " + tokens[0].Substring(1));
                        }
                    }
                }
                else
                {
                    var preProcessResult = FilePreProcessor.ProcessScript(script);

                    ImportNamespaces(preProcessResult.Namespaces.ToArray());

                    foreach (var reference in preProcessResult.References)
                    {
                        var referencePath = FileSystem.GetFullPath(Path.Combine(FileSystem.BinFolder, reference));
                        AddReferences(FileSystem.FileExists(referencePath) ? referencePath : reference);
                    }

                    result = ScriptEngine.Execute(preProcessResult.Code, scriptArgs, References, Namespaces, ScriptPackSession);

                    if (result != null && result.IsCompleteSubmission)
                        PrepareVariables();
                }
            }
            catch (FileNotFoundException fileEx)
            {
                RemoveReferences(fileEx.FileName);
                result = new ScriptResult(compilationException:fileEx);
            }
            catch (Exception ex)
            {
                result = new ScriptResult(executionException:ex);
            }
            finally
            {
                repl.EvaluateCompleted(result);
            }
            return result;
        }


        private static string GetInvalidCommandArgumentMessage(string argument)
        {
            return string.Format(CultureInfo.InvariantCulture, "Argument is not a valid expression: {0}", argument);
        }


        public void AddReferencesAndNotify(params Assembly[] references)
        {
            base.AddReferences(references);
            replCompletion.AddReferences(references);
            documentCompletion.AddReferences(references);
            OnAssemblyReferencesChanged();
        }

        public void RemoveReferencesAndNotify(params Assembly[] references)
        {
            base.RemoveReferences(references);
            replCompletion.RemoveReferences(references);
            documentCompletion.RemoveReferences(references);
            OnAssemblyReferencesChanged();
        }

        public void AddReferencesAndNotify(params string[] references)
        {
            base.AddReferences(references);
            replCompletion.AddReferences(references);
            documentCompletion.AddReferences(references);
            OnAssemblyReferencesChanged();
        }

        public void RemoveReferencesAndNotify(params string[] references)
        {
            base.RemoveReferences(references);
            replCompletion.RemoveReferences(references);
            documentCompletion.RemoveReferences(references);
            OnAssemblyReferencesChanged();
        }

        public string[] GetReferencesAsPaths()
        {
            var paths = new List<string>();
            paths.AddRange(References.PathReferences);
            paths.AddRange(References.Assemblies.Select(a=>a.GetName().Name));
            return paths.ToArray();
        }

        public string[] GetNamespaces()
        {
            return Namespaces.ToArray();
        }

        public override void Reset()
        {
            base.Reset();
            repl.Clear();
        }

        #region Variables
        private string[] variables;
        private void PrepareVariables()
        {
            //see: http://stackoverflow.com/questions/13056208/how-to-get-declared-variables-and-other-definitions
            //this code has to be evaluated from within Roslyn to get the right results
            var result = ScriptEngine.Execute("Assembly.GetExecutingAssembly().DefinedTypes", null, References, Namespaces.Concat(new[] { "System.Reflection" }), ScriptPackSession);
            var runtimeTypes = result.ReturnValue as IEnumerable<TypeInfo>;
            if(runtimeTypes == null)
                return;

            var variableLookup = new Dictionary<string, FieldInfo>();
            foreach (var type in runtimeTypes.Reverse())
            {
                //we are only interested in actual submissions, this filters out class definitions and so on
                if(!type.Name.StartsWith("Submission#"))
                    continue;

                foreach (var variable in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    //not interested in the script host variables (which are in every submission)
                    //only keep the first occurence of a variable, since variables can be redefined
                    if (variable.FieldType != typeof(ReplScriptHost) && !variableLookup.ContainsKey(variable.Name))
                        variableLookup.Add(variable.Name, variable);
                }
            }
            variables = variableLookup.Select(v => v.Value.ToString()).ToArray();
            //clean anonymous types
            //convert the variables from general .NET generics format (List`1[int]) to C# format (List<int>)
            var anonymousRegex = new Regex(@"\<\>f__AnonymousType[0-9]*#[0-9]*");
            var genericsRegex = new Regex(@"\`[0-9]*\[");
            for (int i = 0; i < variables.Length; i++)
            {
                variables[i] = anonymousRegex.Replace(variables[i], "AnonymousType");
                variables[i] = genericsRegex.Replace(variables[i], "<");
                variables[i] = variables[i].Replace(']', '>');
            }
        }

        public string[] GetVariables()
        {
            if(variables == null)
                return new string[0];
            return variables;
        }
        #endregion

    }
}
