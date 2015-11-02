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