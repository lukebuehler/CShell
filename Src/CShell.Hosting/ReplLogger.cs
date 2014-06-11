using System;
using System.Collections.Generic;
using System.Windows.Media;
using Common.Logging;
using Common.Logging.Factory;
using CShell.Framework.Services;

namespace CShell.Hosting
{
    public class ReplLogger : AbstractLogger
    {
        private readonly LogLevel _consoleLogLevel;
        private readonly IRepl _repl;
        private readonly ILog _log;
        private readonly Dictionary<Common.Logging.LogLevel, Color> colors =
            new Dictionary<Common.Logging.LogLevel, Color>
            {
                { Common.Logging.LogLevel.Fatal, Colors.Red },
                { Common.Logging.LogLevel.Error, Colors.DarkRed },
                { Common.Logging.LogLevel.Warn, Colors.DarkOrange },
                { Common.Logging.LogLevel.Info, Colors.Gray },
                { Common.Logging.LogLevel.Debug, Colors.DarkGray },
                { Common.Logging.LogLevel.Trace, Colors.DarkMagenta },
            };

        public ReplLogger(IRepl repl)
            :this(repl, LogLevel.Info, LogManager.GetCurrentClassLogger())
        {}

        public ReplLogger(IRepl repl, LogLevel consoleLogLevel, ILog log)
        {
            if (repl == null) throw new ArgumentNullException("repl");
            if (log == null) throw new ArgumentNullException("log");

            _consoleLogLevel = consoleLogLevel;
            _repl = repl;
            _log = log;
        }

        public override bool IsFatalEnabled
        {
            get { return true; }
        }

        public override bool IsErrorEnabled
        {
            get { return true; }
        }

        public override bool IsWarnEnabled
        {
            get { return true; }
        }

        public override bool IsInfoEnabled
        {
            get { return _log.IsInfoEnabled || _consoleLogLevel != LogLevel.Error; }
        }

        public override bool IsDebugEnabled
        {
            get { return _log.IsDebugEnabled || _consoleLogLevel == LogLevel.Debug || _consoleLogLevel == LogLevel.Trace; }
        }

        public override bool IsTraceEnabled
        {
            get { return _log.IsTraceEnabled || _consoleLogLevel == LogLevel.Trace; }
        }

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            if (message == null)
                message = String.Empty;

            var consoleLog = false;
            switch (level)
            {
                case Common.Logging.LogLevel.Fatal:
                    consoleLog = true;
                    if (_log.IsFatalEnabled)
                    {
                        _log.Fatal(message, exception);
                    }

                    break;
                case Common.Logging.LogLevel.Error:
                    consoleLog = true;
                    if (_log.IsErrorEnabled)
                    {
                        _log.Error(message, exception);
                    }

                    break;
                case Common.Logging.LogLevel.Warn:
                    consoleLog = true;
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn(message, exception);
                    }

                    break;
                case Common.Logging.LogLevel.Info:
                    consoleLog = _consoleLogLevel != LogLevel.Error;
                    if (_log.IsInfoEnabled)
                    {
                        _log.Info(message, exception);
                    }

                    break;
                case Common.Logging.LogLevel.Debug:
                    consoleLog = _consoleLogLevel == LogLevel.Debug || _consoleLogLevel == LogLevel.Trace;
                    if (_log.IsDebugEnabled)
                    {
                        _log.Debug(message, exception);
                    }

                    break;
                case Common.Logging.LogLevel.Trace:
                    consoleLog = _consoleLogLevel == LogLevel.Trace;
                    if (_log.IsTraceEnabled)
                    {
                        _log.Trace(message, exception);
                    }

                    break;
            }

            if (consoleLog)
            {
                var prefix = level == LogLevel.Info
                    ? null
                    : string.Concat(level.ToString().ToUpperInvariant(), ": ");

                var originalOutputColor = _repl.OutputColor;
                Color color;
                if (!colors.TryGetValue(level, out color))
                {
                    color = Colors.Black;
                }
                _repl.OutputColor = color;

                try
                {
                    _repl.WriteLine(string.Concat(prefix, message.ToString()));
                }
                finally
                {
                    _repl.OutputColor = originalOutputColor;
                }
            }
        }
    }
}
