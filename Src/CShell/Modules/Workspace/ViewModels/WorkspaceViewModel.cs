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
using CShell.Framework;
using CShell.Framework.Results;
using CShell.Framework.Services;
using CShell.Properties;
using Caliburn.Micro;

namespace CShell.Modules.Workspace.ViewModels
{
	[Export]
    [Export(typeof(ITool))]
    public class WorkspaceViewModel : Tool, IHandle<WorkspaceOpenedEventArgs>, IHandle<WorkspaceClosingEventArgs>
	{
        [ImportingConstructor]
        public WorkspaceViewModel(IEventAggregator eventAggregator)
        {
            DisplayName = "Workspace Explorer";
            eventAggregator.Subscribe(this);
            var currentWorkspace = CShell.Shell.Workspace;
            if (currentWorkspace != null)
                LoadWorkspace(currentWorkspace);
        }

	    private void LoadWorkspace(CShell.Workspace workspace)
        {
            tree = new TreeViewModel();
            CShellFile = new CShellFileViewModel(workspace.CShellFile);
            tree.Children.Add(cShellFile);

            //add the assembly references
	        var refs = new AssemblyReferencesViewModel(workspace.Assemblies);
            tree.Children.Add(refs);

            //add the file tree
            //var files = new FileReferencesViewModel(workspace.Files, null);
            var files = new RootFolderViewModel(workspace.RootFolder, workspace);
            tree.Children.Add(files);

            NotifyOfPropertyChange(() => Tree);
        }

        private void UnloadWorkspace(CShell.Workspace workspace)
        {
            if(tree != null)
            {
                tree.Dispose();
                tree = null;
                NotifyOfPropertyChange(() => Tree);
            }
        }

	    #region Display
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

	    private CShellFileViewModel cShellFile;
        public CShellFileViewModel CShellFile
	    {
	        get { return cShellFile; }
	        set { cShellFile = value; NotifyOfPropertyChange(()=>CShellFile);}
	    }

	    private TreeViewModel tree;
	    public TreeViewModel Tree
	    {
            get { return tree; }
	    }

        public void Handle(WorkspaceOpenedEventArgs message)
        {
            LoadWorkspace(message.Workspace);
            //save the .cshell file path
            Settings.Default.LastWorkspace = message.Workspace.CShellFile;
            Settings.Default.Save();
        }

        public void Handle(WorkspaceClosingEventArgs message)
        {
            UnloadWorkspace(message.Workspace);
        }

        public IEnumerable<IResult> Open(object node)
        {
            var fileVM = node as FileViewModel;
            if(fileVM != null)
                yield return Show.Document(fileVM.RelativePath);
            var fileCShellVm = node as CShellFileViewModel;
            if(fileCShellVm != null)
                yield return Show.Document(fileCShellVm.RelativePath);
            yield break;
        }

        public IEnumerable<IResult> Selected(object node)
        {
            yield break;
        }
    }
}