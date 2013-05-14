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

namespace CShell.Framework.Services
{
    public static class Execute
    {
        public static void OnUIThread(this Action action)
        {
            Caliburn.Micro.Execute.OnUIThread(action);
        }

        /// <summary>
        /// Executes an action and marshals any exceptions that occur to the calling thread.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void OnUIThreadEx(this Action action)
        {
            Exception exception = null;
            Caliburn.Micro.Execute.OnUIThread(()=>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            });
            if (exception != null)
                throw exception;
        }
    }
}
