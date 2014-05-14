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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caliburn.Micro;
using Application = System.Windows.Application;

namespace CShell.Framework.Results
{
    public class CloseWorkspaceResult : ResultBase
    {
        private readonly bool closeApp;

        /// <summary>
        /// Closes the current workspace.
        /// </summary>
        public CloseWorkspaceResult()
        {}

        /// <summary>
        /// Closes the current workspace and if the close app flag is set also the whole CShell application.
        /// </summary>
        public CloseWorkspaceResult(bool closeApp)
        {
            this.closeApp = closeApp;
        }

        public override void Execute(ActionExecutionContext context)
        {
            //try
            //{
            //    Shell.CloseWorkspace()
            //        .ContinueWith(t =>
            //        {
            //            OnCompleted(t.Exception);
            //            if (closeApp)
            //                Caliburn.Micro.Execute.OnUIThread(()=>Application.Current.MainWindow.Close());
            //        });
            //}
            //catch (Exception ex)
            //{
            //    OnCompleted(ex);
            //}
        }
    }
}
