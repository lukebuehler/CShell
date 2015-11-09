using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShell.Framework.Services
{
    //from here: https://github.com/dbuksbaum/Caliburn.Micro.Logging/blob/master/src/Caliburn.Micro.Logging.NLog/Caliburn.Micro.Logging.NLog/NLogLogger.cs

    /// <summary>
    /// Adapter for Caliburn.Micro logging to Common.Logging
    /// </summary>
    public class LoggerNLog : Caliburn.Micro.ILog
    {
        private readonly NLog.Logger _innerLogger;

        /// <summary>
        /// Creates an NLog caliburn logger
        /// </summary>
        /// <param name="type"></param>
        public LoggerNLog(Type type)
        {
            _innerLogger = NLog.LogManager.GetLogger(type.Name);
        }

        #region ILog Members
        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public void Error(Exception exception)
        {
            _innerLogger.Error(exception, exception.Message);
        }
        /// <summary>
        /// Logs the message as info.
        /// </summary>
        /// <param name="format">A formatted message.</param><param name="args">Parameters to be injected into the formatted message.</param>
        public void Info(string format, params object[] args)
        {
            _innerLogger.Info(format, args);
        }
        /// <summary>
        /// Logs the message as a warning.
        /// </summary>
        /// <param name="format">A formatted message.</param><param name="args">Parameters to be injected into the formatted message.</param>
        public void Warn(string format, params object[] args)
        {
            _innerLogger.Warn(format, args);
        }
        #endregion

        #region Implementation of ILogExtended
        /// <summary>
        /// Logs the message as error.
        /// </summary>
        /// <param name="format">A formatted message.</param>
        /// <param name="args">Parameters to be injected into the formatted message.</param>
        public void Error(string format, params object[] args)
        {
            _innerLogger.Error(format, args);
        }
        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="format">A formatted message.</param>
        /// <param name="args">Parameters to be injected into the formatted message.</param>
        public void Error(Exception exception, string format, params object[] args)
        {
            _innerLogger.ErrorException(string.Format(format, args), exception);
        }
        #endregion
    }
}
