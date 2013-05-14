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
using System.IO;
using System.Linq;
using System.Text;
using CShell.Framework.Results;
using Caliburn.Micro;

namespace CShell.Modules.Workspace.ViewModels
{
    public class RootFolderViewModel : FolderViewModel
    {
        private readonly CShell.Workspace workspace;
        private FileSystemWatcher fileSystemWatcher;

        public RootFolderViewModel(string path, CShell.Workspace workspace)
            :base(path, workspace)
        {
            this.workspace = workspace;
            DisplayName = directoryInfo.FullName;
            IsEditable = false;
            IsExpanded = true;


            Initialize();
        }

        private void Initialize()
        {
            workspace.PropertyChanged += WorkspaceOnPropertyChanged;
            fileSystemWatcher = new FileSystemWatcher();

            fileSystemWatcher.Path = directoryInfo.FullName;
            fileSystemWatcher.IncludeSubdirectories = true;

            // Add event handlers.
            fileSystemWatcher.Changed += OnChanged;
            fileSystemWatcher.Created += OnChanged;
            fileSystemWatcher.Deleted += OnChanged;
            fileSystemWatcher.Renamed += OnRenamed;

            // Begin watching.
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void WorkspaceOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "RootFolder")
            {
                fileSystemWatcher.EnableRaisingEvents = false;
                directoryInfo = new DirectoryInfo(workspace.RootFolder);
                DisplayName = directoryInfo.FullName;

                Children.Clear();
                Children.AddRange(LoadChildren());
                NotifyOfPropertyChange(() => DisplayName);
                NotifyOfPropertyChange(() => ToolTip);
                NotifyOfPropertyChange(() => RelativePath);

                fileSystemWatcher.Path = directoryInfo.FullName;
                fileSystemWatcher.EnableRaisingEvents = true;
            }
            if(args.PropertyName == "Filter")
            {
                Children.Clear();
                Children.AddRange(LoadChildren());
            }
        }

        // Define the event handlers. 
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            if(e.ChangeType == WatcherChangeTypes.Deleted || e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Renamed)
            {
                //Console.WriteLine("Reloading children");
                Children.Clear();
                Children.AddRange(LoadChildren());
            }
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            //Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            if(e.ChangeType == WatcherChangeTypes.Renamed)
            {
                //Console.WriteLine("Reloading children");
                Children.Clear();
                Children.AddRange(LoadChildren());
            }
        }

        #region Edit Root Folder
        public IEnumerable<IResult> ChangeRootFolder()
        {
            yield return Show.Dialog<RootFolderSettingsViewModel>();

        }

        #endregion

    }//end class
}
