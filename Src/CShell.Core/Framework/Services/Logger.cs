#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2012  Arnova Asset Management Ltd., Lukas Buhler
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptCs.Logging;

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
