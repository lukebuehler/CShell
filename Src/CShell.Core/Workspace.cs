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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Caliburn.Micro;
using CShell.Framework.Services;
using CShell.Util;

namespace CShell
{
    [Export]
    public sealed partial class Workspace : PropertyChangedBase
    {
        private readonly IShell shell;
        private readonly IReplExecutorFactory replExecutorFactory;

        private IReplExecutor replExecutor;

        [ImportingConstructor]
        public Workspace(IShell shell, IReplExecutorFactory replExecutorFactory)
        {
            this.shell = shell;
            this.replExecutorFactory = replExecutorFactory;
        }

        public IReplExecutor ReplExecutor
        {
            get { return replExecutor; }
        }

        public string WorkspaceDirectory { get; private set; }


        public void SetWorkspaceDirectory(string dir)
        {
            if (!String.IsNullOrEmpty(WorkspaceDirectory))
            {
                //run teardown script
                //try to save the layout
                SaveLayout();
            }

            replExecutor = null;
            WorkspaceDirectory = dir;

            if (!String.IsNullOrEmpty(WorkspaceDirectory))
            {
                WorkspaceDirectory = Path.GetFullPath(WorkspaceDirectory);
                //create executor
                replExecutor = replExecutorFactory.Create(WorkspaceDirectory);
                //restore layout
                LoadLayout();

                //load libs

                //run startup scripts
            }

            NotifyOfPropertyChange(() => WorkspaceDirectory);
            NotifyOfPropertyChange(() => ReplExecutor);
        }

       

        #region Helpers

        public static void CreateEmptyFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            var templateFile = Path.Combine(exeDir, Constants.CShellTemplatesPath, "Empty" + extension);
            var emptyFileText = "";
            if (File.Exists(templateFile))
                emptyFileText = File.ReadAllText(templateFile);
            File.WriteAllText(filePath, emptyFileText);
        }
        #endregion

    }//end class
}
