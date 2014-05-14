#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2013  Arnova Asset Management Ltd., Lukas Buhler
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
using Caliburn.Micro;

namespace CShell.Framework.Services
{
    /// <summary>
    /// Uses NLog to write out the log messages.
    /// </summary>
    public class NLogLogger : ILog
    {
        private readonly NLog.Logger _innerLogger;

        public NLogLogger(Type type)
        {
            _innerLogger = NLog.LogManager.GetLogger(type.Name);
        }

        public void Error(Exception exception)
        {
            _innerLogger.ErrorException(exception.Message, exception);
        }
        public void Info(string format, params object[] args)
        {
            _innerLogger.Info(format, args);
        }
        public void Warn(string format, params object[] args)
        {
            _innerLogger.Warn(format, args);
        }
    }
}
