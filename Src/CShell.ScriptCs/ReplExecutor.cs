using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Logging;
using CShell.Framework.Services;
using CShell.Util;
using ScriptCs;
using ScriptCs.Contracts;

namespace CShell.ScriptCs
{
    public class ReplExecutor : ScriptExecutor, IReplExecutor
    {
        private readonly IRepl repl;
        private readonly IObjectSerializer serializer;

        public ReplExecutor(
            IRepl repl,
            IObjectSerializer serializer,
            IFileSystem fileSystem,
            IFilePreProcessor filePreProcessor,
            IScriptEngine scriptEngine,
            ILog logger)
            : base(fileSystem, filePreProcessor, scriptEngine, logger)
        {
            this.repl = repl;
            this.serializer = serializer;
        }

        public string WorkspaceDirectory { get { return base.FileSystem.CurrentDirectory; } }

        public event EventHandler<EventArgs> AssemblyReferencesChanged;
        protected virtual void OnAssemblyReferencesChanged()
        {
            EventHandler<EventArgs> handler = AssemblyReferencesChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public override ScriptResult Execute(string script, params string[] scriptArgs)
        {
            var result = new ScriptResult();
            try
            {
                repl.EvaluateStarted(script, null);
                var preProcessResult = FilePreProcessor.ProcessScript(script);

                ImportNamespaces(preProcessResult.Namespaces.ToArray());

                var referencesToAdd = preProcessResult.References.Select(reference =>
                    {
                        var referencePath = FileSystem.GetFullPath(Path.Combine(Constants.BinFolder, reference));
                        return FileSystem.FileExists(referencePath) ? referencePath : reference;
                    })
                    .ToArray();
               
                if(referencesToAdd.Length > 0)
                    AddReferencesAndNotify(referencesToAdd);

                //replControl.Foreground = Brushes.Cyan;

                result = ScriptEngine.Execute(preProcessResult.Code, null, References, Namespaces, ScriptPackSession);
                if (result == null) return new ScriptResult();

                if (result.CompileExceptionInfo != null)
                {
                    //replControl.Foreground = Brushes.Red;
                    //replControl.WriteResultLine(result.CompileExceptionInfo.SourceException.Message);
                }

                if (result.ExecuteExceptionInfo != null)
                {
                    //Console.ForegroundColor = ConsoleColor.Red;
                    //replControl.WriteResultLine(result.ExecuteExceptionInfo.SourceException.Message);
                }

                if (result.IsPendingClosingChar)
                {
                    return result;
                }

                if (result.ReturnValue != null)
                {
                    //Console.ForegroundColor = ConsoleColor.Yellow;

                    //var serializedResult = serializer.Serialize(result.ReturnValue);
                    //replControl.WriteResultLine(serializedResult);
                }
                else if (result.CompileExceptionInfo == null && result.ExecuteExceptionInfo == null)
                {
                    //replControl.WriteResultLine("");
                }

                return result;
            }
            catch (FileNotFoundException fileEx)
            {
                RemoveReferences(fileEx.FileName);
                //Console.ForegroundColor = ConsoleColor.Red;
                //replControl.WriteResultLine("\r\n" + fileEx + "\r\n");
                return new ScriptResult { CompileExceptionInfo = ExceptionDispatchInfo.Capture(fileEx) };
            }
            catch (Exception ex)
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                //replControl.WriteResultLine("\r\n" + ex + "\r\n");
                return new ScriptResult { ExecuteExceptionInfo = ExceptionDispatchInfo.Capture(ex) };
            }
            finally
            {
                repl.EvaluateCompleted(result);
                //Console.ResetColor();
            }
        }

        public void AddReferencesAndNotify(params Assembly[] references)
        {
            base.AddReferences(references);
            OnAssemblyReferencesChanged();
        }

        public void RemoveReferencesAndNotify(params Assembly[] references)
        {
            base.RemoveReferences(references);
            OnAssemblyReferencesChanged();
        }

        public void AddReferencesAndNotify(params string[] references)
        {
            base.AddReferences(references);
            OnAssemblyReferencesChanged();
        }

        public void RemoveReferencesAndNotify(params string[] references)
        {
            base.RemoveReferences(references);
            OnAssemblyReferencesChanged();
        }

        public string[] GetReferencesAsPaths()
        {
            var paths = new List<string>();
            paths.AddRange(References.PathReferences);
            paths.AddRange(References.Assemblies.Select(a=>a.GetName().Name));
            return paths.ToArray();
        }

        public string[] GetReferencesAsFullPaths()
        {
            var paths = new List<string>();
            foreach (var reference in References.PathReferences)
            {
                var fullPath = reference;
                //look in the bin folder
                if(!File.Exists(fullPath))
                    fullPath = FileSystem.GetFullPath(Path.Combine(Constants.BinFolder, reference));
                //try to resolve as relaive path
                if (!File.Exists(fullPath))
                    fullPath = PathHelper.ToAbsolutePath(FileSystem.CurrentDirectory, reference);
                //try to find in GAC
                if (!File.Exists(fullPath))
                {
                    var assemblyName = GlobalAssemblyCache.FindBestMatchingAssemblyName(reference);
                    if(assemblyName != null)
                        fullPath = GlobalAssemblyCache.FindAssemblyInNetGac(assemblyName);
                }

                if(File.Exists(fullPath))
                    paths.Add(fullPath);
            }
            paths.AddRange(References.Assemblies.Select(a => a.Location));
            return paths.ToArray();
        }

        public string[] GetNamespaces()
        {
            return Namespaces.ToArray();
        }
    }
}
