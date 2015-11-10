using System;
using System.Collections.Generic;
using System.Windows.Media;
using CShell.Framework.Services;
using ScriptCs.Contracts;
using Logger = ScriptCs.Contracts.Logger;

namespace CShell.Hosting
{
    public class ReplLogProvider : ILogProvider
    {
        private readonly LogLevel _consoleLogLevel;
        private readonly IReplOutput _replOutput;

        public ReplLogProvider(IReplOutput repl)
            :this(repl, LogLevel.Info)
        {}

        public ReplLogProvider(IReplOutput repl, LogLevel consoleLogLevel)
        {
            if (repl == null) throw new ArgumentNullException("repl");

            _consoleLogLevel = consoleLogLevel;
            _replOutput = repl;
        }

        public Logger GetLogger(string name)
        {
            var log = new ReplLog(name, _replOutput, _consoleLogLevel);
            return log.Log;
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            throw new NotImplementedException();
        }
    }


    internal class ReplLog : ILog
    {
        private readonly string _name;
        private readonly NLog.ILogger _nlogLogger;
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

        internal ReplLog(string name, IReplOutput repl, LogLevel consoleLogLevel)
        {
            if (repl == null) throw new ArgumentNullException("repl");

            _name = name;
            _consoleLogLevel = consoleLogLevel;
            _replOutput = repl;
            _nlogLogger = NLog.LogManager.GetLogger(name);
        }

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception, params object[] formatParameters)
        {
            string message = String.Empty;
            if (messageFunc != null)
            {
                message = messageFunc();
            }

            if (String.IsNullOrEmpty(message) && exception == null)
                return true;

            //forward log messages to NLog
            _nlogLogger.Log(ConvertLogLevel(logLevel), exception, message, formatParameters);

            //now log to the REPL output
            if (logLevel < _consoleLogLevel)
                return true;

            if (formatParameters != null && formatParameters.Length > 0)
                message = String.Format(message, formatParameters);

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

        private static NLog.LogLevel ConvertLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return NLog.LogLevel.Trace;
                case LogLevel.Debug:
                    return NLog.LogLevel.Debug;
                case LogLevel.Info:
                    return NLog.LogLevel.Info;
                case LogLevel.Warn:
                    return NLog.LogLevel.Warn;
                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                case LogLevel.Fatal:
                    return NLog.LogLevel.Fatal;
                default:
                    return NLog.LogLevel.Info;
            }
        }
    }
}
