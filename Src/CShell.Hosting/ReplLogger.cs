namespace CShell.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Media;

    using CShell.Framework.Services;

    using ScriptCs.Logging;

    public class ReplLogger : ILog
    {
        private readonly LogLevel consoleLogLevel;
        private readonly IReplOutput replOutput;
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
            : this(repl, LogLevel.Info)
        {
        }

        public ReplLogger(IReplOutput repl, LogLevel consoleLogLevel)
        {
            if (repl == null)
            {
                throw new ArgumentNullException("repl");
            }

            this.consoleLogLevel = consoleLogLevel;
            this.replOutput = repl;
        }


        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
        {
            if (logLevel < this.consoleLogLevel)
            {
                return false;
            }
            if (messageFunc == null)
            {
                return true;
            }

            var message = messageFunc();
            if (message == null)
            {
                message = string.Empty;
            }

            if (formatParameters != null && formatParameters.Length > 0)
            {
                message = string.Format(message, formatParameters);
            }

            var prefix = logLevel == LogLevel.Info
                ? null
                : string.Concat(logLevel.ToString().ToUpperInvariant(), ": ");

            var originalOutputColor = this.replOutput.ResultColor;
            Color color;
            if (!this.colors.TryGetValue(logLevel, out color))
            {
                color = Colors.Black;
            }

            this.replOutput.ResultColor = color;

            try
            {
                this.replOutput.WriteLine(prefix + message);
            }
            finally
            {
                this.replOutput.ResultColor = originalOutputColor;
            }

            return true;
        }
    }
}
