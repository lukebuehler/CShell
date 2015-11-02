using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShell.Framework.Services
{
    /// <summary>
    /// Adapter for Caliburn.Micro logging to Common.Logging
    /// </summary>
    public class Logger : Caliburn.Micro.ILog
    {
       // private readonly ILog innerLogger;

        public Logger(Type type)
        {
            //this.innerLogger = LogManager.GetLogger(type);
        }

        public void Error(Exception exception)
        {
            //innerLogger.Error(exception.Message, exception);
        }

        public void Info(string format, params object[] args)
        {
            //innerLogger.InfoFormat(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            //innerLogger.WarnFormat(format, args);
        }
    }
}
