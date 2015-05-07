using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CShell.Completion;
using CShell.Framework.Services;
using ScriptCs;
using ScriptCs.Contracts;
using ScriptCs.Logging;

namespace CShell.Hosting
{
    public class ReplScriptExecutor : ScriptExecutor, IReplScriptExecutor
    {
        public static readonly string[] DefaultReferencesCShell =
        {
            typeof(Shell).Assembly.Location, //CShell.Core
        };

        public static readonly string[] DefaultNamespacesCShell =
        {
             "CShell",
        };

        private readonly IReplOutput replOutput;
        private readonly IObjectSerializer serializer;

        public ReplScriptExecutor(
            IReplOutput replOutput,
            IObjectSerializer serializer,
            IFileSystem fileSystem,
            IFilePreProcessor filePreProcessor,
            IScriptEngine scriptEngine,
            ILog logger,
            IEnumerable<IReplCommand> replCommands)
            : base(fileSystem, filePreProcessor, scriptEngine, logger)
        {
            this.replOutput = replOutput;
            this.serializer = serializer;
            Commands = replCommands != null ? replCommands
                .Where(x => x.GetType().Namespace.StartsWith("CShell")) //hack to only include CShell commands for not
                .Where(x => x.CommandName != null)
                .ToDictionary(x => x.CommandName, x => x)
                : new Dictionary<string, IReplCommand>();

            replCompletion = new CSharpCompletion(true);
            replCompletion.AddReferences(GetReferencesAsPaths());
            //since it's quite expensive to initialize the "System." references we clone the REPL code completion
            documentCompletion = replCompletion.Clone();

            AddReferences(DefaultReferencesCShell);
            ImportNamespaces(DefaultNamespacesCShell);
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


        public string Buffer { get; private set; }

        public Dictionary<string, IReplCommand> Commands { get; private set; }

        public override void Initialize(IEnumerable<string> paths, IEnumerable<IScriptPack> scriptPacks, params string[] scriptArgs)
        {
            base.Initialize(paths, scriptPacks, scriptArgs);
            ExecuteReferencesScript();
            ExecuteConfigScript();
        }

        public override ScriptResult Execute(string script, params string[] scriptArgs)
        {
            ScriptResult result = null;
            try
            {
                replOutput.EvaluateStarted(script, null);

                if (script.StartsWith(":"))
                {
                    var tokens = script.Split(' ');
                    if (tokens[0].Length > 1)
                    {
                        if (Commands.ContainsKey(tokens[0].Substring(1)))
                        {
                            var command = Commands[tokens[0].Substring(1)];
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

                    InjectScriptLibraries(FileSystem.CurrentDirectory, preProcessResult, ScriptPackSession.State);

                    Buffer = (Buffer == null)
                        ? preProcessResult.Code
                        : Buffer + Environment.NewLine + preProcessResult.Code;

                    var namespaces = Namespaces.Union(preProcessResult.Namespaces).ToList();
                    var references = References.Union(preProcessResult.References);

                    if (preProcessResult.References != null && preProcessResult.References.Count > 0)
                    {
                        OnAssemblyReferencesChanged();
                    }

                    result = ScriptEngine.Execute(Buffer, scriptArgs, references, namespaces, ScriptPackSession);

                    if (result == null)
                    {
                        result = ScriptResult.Empty;
                    }
                    else
                    {
                        if (result.InvalidNamespaces.Any())
                        {
                            RemoveNamespaces(result.InvalidNamespaces.ToArray());
                        }

                        if (result.IsCompleteSubmission)
                        {
                            Buffer = null;
                        }
                    }
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
                replOutput.EvaluateCompleted(result);
            }
            return result ?? ScriptResult.Empty;
        }


        private static string GetInvalidCommandArgumentMessage(string argument)
        {
            return string.Format(CultureInfo.InvariantCulture, "Argument is not a valid expression: {0}", argument);
        }

        public override void AddReferences(params Assembly[] references)
        {
            base.AddReferences(references);
            replCompletion.AddReferences(references);
            documentCompletion.AddReferences(references);
            OnAssemblyReferencesChanged();
        }

        public override void RemoveReferences(params Assembly[] references)
        {
            base.RemoveReferences(references);
            replCompletion.RemoveReferences(references);
            documentCompletion.RemoveReferences(references);
            OnAssemblyReferencesChanged();
        }

        public override void AddReferences(params string[] references)
        {
            base.AddReferences(references);
            replCompletion.AddReferences(references);
            documentCompletion.AddReferences(references);
            OnAssemblyReferencesChanged();
        }

        public override void RemoveReferences(params string[] references)
        {
            base.RemoveReferences(references);
            replCompletion.RemoveReferences(references);
            documentCompletion.RemoveReferences(references);
            OnAssemblyReferencesChanged();
        }

        public string[] GetReferencesAsPaths()
        {
            var paths = new List<string>();
            paths.AddRange(References.Paths);
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
            AddReferences(DefaultReferencesCShell);
            ImportNamespaces(DefaultNamespacesCShell);
            replOutput.Clear();
            ExecuteReferencesScript();
        }

        public string[] GetVariables()
        {
            var replEngine = ScriptEngine as IReplEngine;
            if (replEngine != null)
            {
                var varsArray = replEngine.GetLocalVariables(ScriptPackSession)
                    .Where(x => !x.StartsWith("submission", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                return varsArray;
            }
            return new string[0];
        }


        public void ExecuteConfigScript()
        {
            var configPath = Path.Combine(WorkspaceDirectory, CShell.Constants.ConfigFile);
            if (File.Exists(configPath))
            {
                var configScript = File.ReadAllText(configPath);
                Execute(configScript);
            }
        }

        public void ExecuteReferencesScript()
        {
            var refPath = Path.Combine(WorkspaceDirectory, CShell.Constants.ReferencesFile);
            if (File.Exists(refPath))
            {
                var configScript = File.ReadAllText(refPath);
                Execute(configScript);
            }

            var binPath = Path.Combine(WorkspaceDirectory, CShell.Constants.BinFolder);
            if (Directory.Exists(binPath))
            {
                var binFiles = Directory.EnumerateFiles(binPath, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                AddReferences(binFiles);
            }
        }
    }
}
