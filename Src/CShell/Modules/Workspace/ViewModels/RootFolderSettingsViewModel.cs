using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using CShell.Framework.Results;
using Caliburn.Micro;

namespace CShell.Modules.Workspace.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RootFolderSettingsViewModel : Screen
    {
        public RootFolderSettingsViewModel()
        {
            DisplayName = "Change Root Folder";
            RootFolder = CShell.Shell.Workspace.RootFolder;
        }

        private string rootFolder;
        public string RootFolder
        {
            get { return rootFolder; }
            set { rootFolder = value; 
                NotifyOfPropertyChange(()=>RootFolder); 
                NotifyOfPropertyChange(()=>CanOk);
            }
        }

        public bool CanOk
        {
            get { return !String.IsNullOrEmpty(rootFolder) && System.IO.Directory.Exists(rootFolder); }
        }

        public void Ok()
        {
            //setting this property will also update the UI
            CShell.Shell.Workspace.RootFolder = rootFolder;
            this.TryClose(true);
        }

        public void Cancel()
        {
            this.TryClose(false);
        }

        public IEnumerable<IResult> Browse()
        {
            var folderResult = Show.FolderDialog(rootFolder);
            yield return folderResult;
            var folder = folderResult.SelectedFolder;
            if (System.IO.Directory.Exists(folder))
                RootFolder = folder;
        }
    }
}
