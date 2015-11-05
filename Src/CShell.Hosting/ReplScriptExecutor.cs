namespace CShell.Hosting
{
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

    using ILog = ScriptCs.Logging.ILog;

    public class ReplScriptExecutor : ScriptExecutor, IReplScriptExecutor
    {
        private readonly IReplOutput replOutput;
        private readonly IObjectSerializer serializer;
        private readonly IDefaultReferences defaultReferences;

        public ReplScriptExecutor(
            IReplOutput replOutput,
            IObjectSerializer serializer,
            IFileSystem fileSystem,
            IFilePreProcessor filePreProcessor,
            IScriptEngine scriptEngine,
            ILog logger,
            IEnumerable<IReplCommand> replCommands,
            IDefaultReferences defaultReferences)
            : base(fileSystem, filePreProcessor, scriptEngine, logger)
        {
            this.replOutput = replOutput;
            this.serializer = serializer;
            this.defaultReferences = defaultReferences;
            this.Commands = replCommands != null ? replCommands
                .Where(x => x.GetType().Namespace.StartsWith("CShell")) // hack to only include CShell commands for now
                .Where(x => x.CommandName != null)
                .ToDictionary(x => x.CommandName, x => x)
                : new Dictionary<string, IReplCommand>();

            this.replCompletion = new CSharpCompletion(true);
            this.replCompletion.AddReferences(this.GetReferencesAsPaths());

            // since it's quite expensive to initialize the "System." references we clone the REPL code completion
            this.documentCompletion = this.replCompletion.Clone();

            this.AddDefaultReferencesAndNamespaces();
        }

        public string WorkspaceDirectory
        {
            get
            {
                return base.FileSystem.CurrentDirectory;
            }
        }

        public event EventHandler<EventArgs> AssemblyReferencesChanged;
        protected virtual void OnAssemblyReferencesChanged()
        {
            var handler = this.AssemblyReferencesChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private readonly ICompletion replCompletion;
        private readonly ICompletion documentCompletion;

        public ICompletion ReplCompletion
        {
            get { return this.replCompletion; }
        }

        public ICompletion DocumentCompletion
        {
            get { return this.documentCompletion; }
        }

        public string Buffer { get; private set; }

        public Dictionary<string, IReplCommand> Commands { get; private set; }

        public override void Initialize(IEnumerable<string> paths, IEnumerable<IScriptPack> scriptPacks, params string[] scriptArgs)
        {
            base.Initialize(paths, scriptPacks, scriptArgs);
            this.ExecuteReferencesScript();
            this.ExecuteConfigScript();
        }

        public override ScriptResult Execute(string script, params string[] scriptArgs)
        {
            ScriptResult result = null;
            try
            {
                this.replOutput.EvaluateStarted(script, null);

                if (script.StartsWith(":"))
                {
                    var tokens = script.Split(' ');
                    if (tokens[0].Length > 1)
                    {
                        if (this.Commands.ContainsKey(tokens[0].Substring(1)))
                        {
                            var command = this.Commands[tokens[0].Substring(1)];
                            var argsToPass = new List<object>();
                            foreach (var argument in tokens.Skip(1))
                            {
                                var argumentResult = this.ScriptEngine.Execute(
                                    argument,
                                    scriptArgs,
                                    this.References,
                                    this.Namespaces,
                                    this.ScriptPackSession);

                                if (argumentResult == null)
                                {
                                    argsToPass.Add(argument);
                                    continue;
                                }

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
                            {
                                result = commandResult as ScriptResult;
                            }
                            else
                            {
                                result = new ScriptResult(commandResult);
                            }
                        }
                        else
                        {
                            throw new Exception("Command not found: " + tokens[0].Substring(1));
                        }
                    }
                }
                else
                {
                    var preProcessResult = this.FilePreProcessor.ProcessScript(script);

                    this.ImportNamespaces(preProcessResult.Namespaces.ToArray());

                    foreach (var reference in preProcessResult.References)
                    {
                        var referencePath = this.FileSystem.GetFullPath(Path.Combine(this.FileSystem.BinFolder, reference));
                        this.AddReferences(this.FileSystem.FileExists(referencePath) ? referencePath : reference);
                    }

                    this.InjectScriptLibraries(this.FileSystem.CurrentDirectory, preProcessResult, this.ScriptPackSession.State);

                    this.Buffer = (this.Buffer == null)
                        ? preProcessResult.Code
                        : this.Buffer + Environment.NewLine + preProcessResult.Code;

                    var namespaces = this.Namespaces.Union(preProcessResult.Namespaces).ToList();
                    var references = this.References.Union(preProcessResult.References);

                    if (preProcessResult.References != null && preProcessResult.References.Count > 0)
                    {
                        this.OnAssemblyReferencesChanged();
                    }

                    result = this.ScriptEngine.Execute(this.Buffer, scriptArgs, references, namespaces, this.ScriptPackSession);

                    if (result == null)
                    {
                        result = ScriptResult.Empty;
                    }
                    else
                    {
                        if (result.InvalidNamespaces.Any())
                        {
                            this.RemoveNamespaces(result.InvalidNamespaces.ToArray());
                        }

                        if (result.IsCompleteSubmission)
                        {
                            this.Buffer = null;
                        }
                    }
                }
            }
            catch (FileNotFoundException fileEx)
            {
                this.RemoveReferences(fileEx.FileName);
                result = new ScriptResult(compilationException: fileEx);
            }
            catch (Exception ex)
            {
                result = new ScriptResult(executionException: ex);
            }
            finally
            {
                this.replOutput.EvaluateCompleted(result);
            }

            return result ?? ScriptResult.Empty;
        }

        private static string GetInvalidCommandArgumentMessage(string argument)
        {
            return string.Format(CultureInfo.InvariantCulture, "Argument is not a valid expression: {0}", argument);
        }

        private void AddDefaultReferencesAndNamespaces()
        {
            this.AddReferences(typeof(Shell).Assembly);
            this.ImportNamespaces(typeof(Shell).Namespace);
            this.AddReferences(this.defaultReferences.Assemblies.Distinct().ToArray());
            this.AddReferences(this.defaultReferences.AssemblyPaths.Distinct().ToArray());
            this.ImportNamespaces(this.defaultReferences.Namespaces.Distinct().ToArray());
        }

        public override void AddReferences(params Assembly[] references)
        {
            base.AddReferences(references);
            this.replCompletion.AddReferences(references);
            this.documentCompletion.AddReferences(references);
            this.OnAssemblyReferencesChanged();
        }

        public override void RemoveReferences(params Assembly[] references)
        {
            base.RemoveReferences(references);
            this.replCompletion.RemoveReferences(references);
            this.documentCompletion.RemoveReferences(references);
            this.OnAssemblyReferencesChanged();
        }

        public override void AddReferences(params string[] references)
        {
            base.AddReferences(references);
            this.replCompletion.AddReferences(references);
            this.documentCompletion.AddReferences(references);
            this.OnAssemblyReferencesChanged();
        }

        public override void RemoveReferences(params string[] references)
        {
            base.RemoveReferences(references);
            this.replCompletion.RemoveReferences(references);
            this.documentCompletion.RemoveReferences(references);
            this.OnAssemblyReferencesChanged();
        }

        public string[] GetReferencesAsPaths()
        {
            var paths = new List<string>();
            paths.AddRange(this.References.Paths);
            paths.AddRange(this.References.Assemblies.Select(a => a.GetName().Name));
            return paths.ToArray();
        }

        public string[] GetNamespaces()
        {
            return this.Namespaces.ToArray();
        }

        public override void Reset()
        {
            base.Reset();
            this.AddDefaultReferencesAndNamespaces();
            this.replOutput.Clear();
            this.ExecuteReferencesScript();
        }

        public string[] GetVariables()
        {
            var replEngine = this.ScriptEngine as IReplEngine;
            if (replEngine != null)
            {
                var varsArray = replEngine.GetLocalVariables(this.ScriptPackSession)
                    .Where(x => !x.StartsWith("submission", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                return varsArray;
            }

            return new string[0];
        }

        public void ExecuteConfigScript()
        {
            var configPath = Path.Combine(this.WorkspaceDirectory, CShell.Constants.ConfigFile);
            if (File.Exists(configPath))
            {
                var configScript = File.ReadAllText(configPath);
                this.Execute(configScript);
            }
        }

        public void ExecuteReferencesScript()
        {
            var refPath = Path.Combine(this.WorkspaceDirectory, CShell.Constants.ReferencesFile);
            if (File.Exists(refPath))
            {
                var configScript = File.ReadAllText(refPath);
                this.Execute(configScript);
            }

            var binPath = Path.Combine(this.WorkspaceDirectory, CShell.Constants.BinFolder);
            if (Directory.Exists(binPath))
            {
                var binFiles = Directory.EnumerateFiles(binPath, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    .ToArray();
                this.AddReferences(binFiles);
            }
        }
    }
}
