using System;
using System.Collections.Generic;
using System.Windows.Media;
using CShell.Framework.Services;
using ScriptCs.Logging;

namespace CShell.Hosting
{
    public class ReplLogger : ILog
    {
        private readonly LogLevel _consoleLogLevel;
        private readonly IReplOutput _replOutput;
        private readonly Dictionary<LogLevel, Color> colors =
            new Dictionary<LogLevel, Color>
            {
                { LogLevel.Fatal, Colors.Red },
                { LogLevel.Error, Colors.DarkRed },
                { LogLevel.Warn, Colors.DarkOrange },
                { LogLevel.Info, Colors.Gray },
                { LogLevel.Debug, Colors.DarkGray },
                { LogLevel.Trace, Colors.DarkMagenta },
            };

        public ReplLogger(IReplOutput repl)
            :this(repl, LogLevel.Info)
        {}

        public ReplLogger(IReplOutput repl, LogLevel consoleLogLevel)
        {
            if (repl == null) throw new ArgumentNullException("repl");

            _consoleLogLevel = consoleLogLevel;
            _replOutput = repl;
        }


        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
        {
            if (logLevel < _consoleLogLevel)
                return false;
            if (messageFunc == null)
                return true;

            var message = messageFunc();
            if (message == null)
                message = String.Empty;


            var prefix = logLevel == LogLevel.Info
                ? null
                : string.Concat(logLevel.ToString().ToUpperInvariant(), ": ");

            var originalOutputColor = _replOutput.ResultColor;
            Color color;
            if (!colors.TryGetValue(logLevel, out color))
            {
                color = Colors.Black;
            }
            _replOutput.ResultColor = color;

            try
            {
                _replOutput.WriteLine(prefix + message);
            }
            finally
            {
                _replOutput.ResultColor = originalOutputColor;
            }
            return true;
        }

    }
}
