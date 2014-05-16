using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Logging;
using CShell.Framework.Services;
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

        public override ScriptResult Execute(string script, params string[] scriptArgs)
        {
            var result = new ScriptResult();
            try
            {
                repl.EvaluateStarted(script, null);
                var preProcessResult = FilePreProcessor.ProcessScript(script);

                ImportNamespaces(preProcessResult.Namespaces.ToArray());

                foreach (var reference in preProcessResult.References)
                {
                    var referencePath = FileSystem.GetFullPath(Path.Combine(Constants.BinFolder, reference));
                    AddReferences(FileSystem.FileExists(referencePath) ? referencePath : reference);
                }

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
    }
}
