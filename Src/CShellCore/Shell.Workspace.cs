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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace CShell
{
    public static partial class Shell
    {
        private static Workspace workspace = null;
        private static readonly object instanceLock = new object();

        /// <summary>
        /// Gets the instance of the currently open workspace, if no workspace is open returns null.
        /// </summary>
        public static Workspace Workspace
        {
            get
            {
                lock (instanceLock)
                {
                    return workspace;
                }
            }
        }

        /// <summary>
        /// Opens a default workspace.
        /// </summary>
        public static void OpenWorkspace()
        {
            if (File.Exists(Constants.CShellDefaultFile))
            {
                OpenWorkspace(Constants.CShellDefaultFile);
            }
            else
            {
                throw new FileNotFoundException("Could not find default cshell file: " + Constants.CShellDefaultFile);
            }
        }

        /// <summary>
        /// Opens a workspace based on a .cshell file.
        /// </summary>
        /// <param name="cshellFile">The file path to the .cshell file.</param>
        /// <returns>A task that completes once the workspace is opened.</returns>
        public static Task OpenWorkspace(string cshellFile)
        {
            var eventAggregator = IoC.Get<IEventAggregator>();

            lock (instanceLock)
            {
                if (!File.Exists(cshellFile))
                    Workspace.CreateWorkspaceFile(cshellFile);
                workspace = new Workspace(cshellFile);
            }
            //send the opening event
            if (eventAggregator != null)
                eventAggregator.Publish(new WorkspaceOpeningEventArgs(Workspace));

            return Workspace.Open()
                .ContinueWith(t =>
                {
                    //send the opened event
                    if (eventAggregator != null)
                        eventAggregator.Publish(new WorkspaceOpenedEventArgs(Workspace));
                });
        }

        /// <summary>
        /// Closes the current workspace.
        /// </summary>
        /// <returns>A task that completes once the workspace is closed.</returns>
        public static Task CloseWorkspace()
        {
            if (Workspace == null)
            {
                var t = new TaskCompletionSource<object>();
                t.SetResult(null);
                return t.Task;
            }

            //send the closing event
            var eventAggregator = IoC.Get<IEventAggregator>();
            if (eventAggregator != null)
                eventAggregator.Publish(new WorkspaceClosingEventArgs(Workspace));

            return Workspace.Close()
                 .ContinueWith(t =>
                 {
                     lock (instanceLock)
                     {
                         workspace = null;
                     }
                     if (eventAggregator != null)
                         eventAggregator.Publish(new WorkspaceClosedEventArgs());
                 });
        }
    }
}
