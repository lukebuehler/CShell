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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caliburn.Micro;
using CShell.Framework.Services;

namespace CShell.Framework.Results
{
    public class ChangeWorkspaceResult : ResultBase
    {
        private string workspaceDirectory;

        [Import]
        private Workspace workspace;

        /// <summary>
        /// Creates a result that will open a workspace.
        /// </summary>
        public ChangeWorkspaceResult(string workspaceDirectory)
        {
            this.workspaceDirectory = workspaceDirectory;
        }

        public override void Execute(CoroutineExecutionContext context)
        {
            
            try
            {
                //some of this is synchronous which can mess up the UI (especially on startup), so we execute it on a seperate thread
                Task.Factory.StartNew(() => workspace.SetWorkspaceDirectory(workspaceDirectory))
                    .ContinueWith(t2 => OnCompleted(t2.Exception));
            }
            catch (Exception ex)
            {
                OnCompleted(ex);
            }
        }
    }
}
