using System;
using System.Diagnostics;
using Caliburn.Micro;

namespace CShell.Framework.Services
{
    /// <summary>
    /// A simple logger that writes to the Visual Studio debug log.
    /// From here: http://buksbaum.us/2010/08/08/how-to-do-logging-with-caliburn-micro/
    /// </summary>
    public class DebugLogger : Caliburn.Micro.ILog
    {
        private readonly Type type;
        public DebugLogger(Type type)
        {
            type = type;
        }

        private string CreateLogMessage(string format, params object[] args)
        {
            return string.Format("[{0}] {1}",
                                 DateTime.Now.ToString("o"),
                                 string.Format(format, args));
        }

        #region ILog Members
        public void Error(Exception exception)
        {
            Debug.WriteLine(CreateLogMessage(exception.ToString()), "ERROR");
        }
        public void Info(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "INFO");
        }
        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "WARN");
        }
        #endregion
    }
}
