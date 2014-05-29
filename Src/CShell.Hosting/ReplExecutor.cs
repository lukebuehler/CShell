using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Versioning;
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
        private readonly IPackageInstaller packageInstaller;
        private readonly IPackageAssemblyResolver resolver;

        public ReplExecutor(
            IRepl repl,
            IObjectSerializer serializer,
            IFileSystem fileSystem,
            IFilePreProcessor filePreProcessor,
            IScriptEngine scriptEngine,
            IPackageInstaller packageInstaller,
            IPackageAssemblyResolver resolver,
            ILog logger)
            : base(fileSystem, filePreProcessor, scriptEngine, logger)
        {
            this.repl = repl;
            this.serializer = serializer;
            this.packageInstaller = packageInstaller;
            this.resolver = resolver;

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

        public override ScriptResult Execute(string script, params string[] scriptArgs)
        {
            var result = new ScriptResult();
            repl.EvaluateStarted(script, null);

            try
            {
                if (script.StartsWith(":"))
                {
                    var arguments = script.Split(' ');
                    var command = arguments[0].Substring(1);

                    var argsToPass = new List<object>();
                    foreach (var argument in arguments.Skip(1))
                    {
                        try
                        {
                            var argumentResult = ScriptEngine.Execute(argument, scriptArgs, References, DefaultNamespaces, ScriptPackSession);
                            //if Roslyn can evaluate the argument, use its value, otherwise assume the string
                            argsToPass.Add(argumentResult.ReturnValue ?? argument);
                        }
                        catch (Exception)
                        {
                            argsToPass.Add(argument);
                        }
                    }

                    var commandResult = HandleReplCommand(command, argsToPass.ToArray());
                    result = new ScriptResult(commandResult);
                    return result;
                }

                var preProcessResult = FilePreProcessor.ProcessScript(script);

                ImportNamespaces(preProcessResult.Namespaces.ToArray());

                var referencesToAdd = preProcessResult.References.Select(reference =>
                {
                    var referencePath = FileSystem.GetFullPath(Path.Combine(Constants.BinFolder, reference));
                    return FileSystem.FileExists(referencePath) ? referencePath : reference;
                })
                    .ToArray();

                if (referencesToAdd.Length > 0)
                    AddReferencesAndNotify(referencesToAdd);
                result = ScriptEngine.Execute(preProcessResult.Code, scriptArgs, References, Namespaces, ScriptPackSession);
                if (result == null) return new ScriptResult();

                return result;
            }
            catch (FileNotFoundException fileEx)
            {
                RemoveReferences(fileEx.FileName);
                return new ScriptResult(compilationException:fileEx);
            }
            catch (Exception ex)
            {
                return new ScriptResult(executionException:ex);
            }
            finally
            {
                repl.EvaluateCompleted(result);
            }
        }

        private object HandleReplCommand(string command, object[] args)
        {
            if(string.IsNullOrWhiteSpace(command))
                return "The REPL command was empty.";

            command = command.ToLower();
            if (command == "help")
            {
                return "Available commands are: " + Environment.NewLine +
                       " :help                   - displays this information" + Environment.NewLine +
                       " :clear                  - clears the REPL" + Environment.NewLine +
                       " :install <package name> - installs a NuGet package";
            }
            if (command == "clear")
            {
                repl.Clear();
                return null;
            }
            if (command == "install")
            {
                if (args == null || args.Length == 0) return null;

                string version = null;
                var allowPre = false;
                if (args.Length >= 2)
                {
                    version = args[1].ToString();
                    if (args.Length == 3)
                    {
                        allowPre = true;
                    }
                }

                Logger.InfoFormat("Installing {0}", args[0]);

                var packageRef = new PackageReference(args[0].ToString(), new FrameworkName(".NETFramework,Version=v4.0"), version);
                packageInstaller.InstallPackages(new[] { packageRef }, allowPre);
                resolver.SavePackages();

                var dlls = resolver.GetAssemblyNames(FileSystem.CurrentDirectory).Except(References.PathReferences).ToArray();
                AddReferencesAndNotify(dlls);

                foreach (var dll in dlls)
                {
                    Logger.InfoFormat("Added reference to {0}", dll);
                }

                return null;
            }

            return "Unknown REPL command: "+command;
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





       
    }
}
