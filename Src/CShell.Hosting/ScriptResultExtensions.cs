using System;
using System.Collections.Generic;
using ScriptCs.Contracts;

namespace CShell.Hosting
{
    public static class ScriptResultExtensions
    {
        public static bool HasWarnings(this ScriptResult scriptResult)
        {
            if (scriptResult.CompileExceptionInfo != null)
            {
                var ex = scriptResult.CompileExceptionInfo.SourceException;
                if (IsMessage(ex.Message, "warning"))
                    return true;
            }
            if (scriptResult.ExecuteExceptionInfo != null)
            {
                var ex = scriptResult.ExecuteExceptionInfo.SourceException;
                if (IsMessage(ex.Message, "warning"))
                    return true;
            }
            return false;
        }

        public static bool HasErrors(this ScriptResult scriptResult)
        {
            if (scriptResult.CompileExceptionInfo != null)
            {
                var ex = scriptResult.CompileExceptionInfo.SourceException;
                if (IsMessage(ex.Message, "error"))
                    return true;
            }
            if (scriptResult.ExecuteExceptionInfo != null)
            {
                return true;
            }
            return false;
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

        public static string[] GetMessages(this ScriptResult scriptResult)
        {
            var msgs = new List<string>();
            if (scriptResult.CompileExceptionInfo != null)
            {
                var ex = scriptResult.CompileExceptionInfo.SourceException;
                msgs.Add(ex.Message);
            }
            if (scriptResult.ExecuteExceptionInfo != null)
            {
                var ex = scriptResult.ExecuteExceptionInfo.SourceException;
                msgs.Add(ex.Message);
            }
            return msgs.ToArray();
        }
    }
}
