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
using CShell.Framework;
using CShell.Framework.Results;
using CShell.Framework.Services;
using CShell.Properties;
using Caliburn.Micro;

namespace CShell.Modules.Workspace.ViewModels
{
	[Export]
    [Export(typeof(ITool))]
    public class WorkspaceViewModel : Tool
	{
	    private readonly IShell shell;
        private readonly CShell.Workspace workspace;

	    [ImportingConstructor]
        public WorkspaceViewModel(CShell.Workspace workspace)
        {
            this.shell = shell;
            DisplayName = "Workspace Explorer";
	        this.workspace = workspace;
            this.workspace.PropertyChanged += WorkspaceOnPropertyChanged;
        }

	    #region Display
        private TreeViewModel tree;
        public TreeViewModel Tree
        {
            get { return tree; }
        }

		public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Left; }
		} 

		public override Uri IconSource
		{
			get { return new Uri("pack://application:,,,/CShell;component/Resources/Icons/FileBrowser.png"); }
		}

        public override Uri Uri
        {
            get { return new Uri("tool://cshell/workspace"); }
        }
        #endregion

        private void WorkspaceOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "WorkspaceDirectory")
            {   //teardown the current workspace
                if (tree != null)
                {
                    tree.Dispose();
                    tree = null;
                    NotifyOfPropertyChange(() => Tree);
                }

                if (workspace.WorkspaceDirectory != null && Directory.Exists(workspace.WorkspaceDirectory))
                {
                    tree = new TreeViewModel();

                    //add the assembly references
                    var refs = new AssemblyReferencesViewModel(workspace.ReplExecutor);
                    tree.Children.Add(refs);

                    //add the file tree
                    //var files = new FileReferencesViewModel(workspace.Files, null);
                    var files = new FolderRootViewModel(workspace.WorkspaceDirectory, workspace);
                    tree.Children.Add(files);

                    NotifyOfPropertyChange(() => Tree);

                    Settings.Default.LastWorkspace = workspace.WorkspaceDirectory;
                    Settings.Default.Save();
                }
            }
        }

        public IEnumerable<IResult> Open(object node)
        {
            var fileVM = node as FileViewModel;
            if(fileVM != null)
                yield return Show.Document(fileVM.RelativePath);
            yield break;
        }

        public IEnumerable<IResult> Selected(object node)
        {
            yield break;
        }
    }
}