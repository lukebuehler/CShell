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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CShell.Framework;
using CShell.Framework.Menus;
using CShell.Framework.Results;
using CShell.Framework.Services;
using CShell.Modules.Workspace.Results;
using CShell.Modules.Workspace.ViewModels;
using Caliburn.Micro;
using Microsoft.Win32;

namespace CShell.Modules.Workspace
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
        public Module()
        {
            Order = 3;
        }

        public override void Configure(IModuleConfiguration configuration)
        { }

        public override void Start()
        {
			MainMenu.All.First(x => x.Name == "View")
                .Add(new MenuItem("Workspace Explorer", OpenWorkspaceExplorer).WithIcon("Resources/Icons/FileBrowser.png"));

            var workspaceViewModel = IoC.Get<WorkspaceViewModel>();
		    //workspaceViewModel.IsVisible = false;
            Shell.ShowTool(workspaceViewModel);

		    var addNewFile = new MenuItem("Add New File...", AddNewFile);
		    var addFolder = new MenuItem("Add New Folder...", AddNewFolder);
		    var addReference = new MenuItem("Add References from Files...", AddFileReferences);
            var addReferenceGac = new MenuItem("Add References from GAC...", AddGacReferences);
            var copyReference = new MenuItem("Copy References to Bin...", CopyReferences);
            var managePackages = new MenuItem("Manage NuGet Packages...", MangePackages);
            //populate the menu
            MainMenu.First(item => item.Name == "Workspace")
                .Add(
                    addNewFile,
                    MenuItemBase.Separator,
                    addFolder,
                    MenuItemBase.Separator,
                    addReference,
                    addReferenceGac,
                    MenuItemBase.Separator,
                    copyReference,
                    MenuItemBase.Separator,
                    managePackages
                );
		}

		private static IEnumerable<IResult> OpenWorkspaceExplorer()
		{
			yield return Show.Tool<WorkspaceViewModel>();
		}

        private IEnumerable<IResult> AddNewFile()
        {
            return GetSelectedFolder().AddNewFile();
        }

        private IEnumerable<IResult> AddNewFolder()
        {
            return GetSelectedFolder().AddNewFolder();
        }

        public static IEnumerable<IResult> AddFileReferences()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = CShell.Constants.AssemblyFileFilter;
            dialog.Multiselect = true;
            yield return Show.Dialog(dialog);
            if (dialog.FileNames != null && dialog.FileNames.Length > 0)
            {
                yield return new AddReferencesResult(dialog.FileNames);
            }
        }

        public static IEnumerable<IResult> AddGacReferences()
        {
            var windowSettings = new Dictionary<string, object> { { "SizeToContent", SizeToContent.Manual }, { "Width", 500.0 }, { "Height", 500.0 } };
            var dialog = new AssemblyGacViewModel();
            yield return Show.Dialog(dialog, windowSettings);
            var selectedAssemblies = dialog.SelectedAssemblies.Select(item => item.AssemblyName).ToArray();
            if (selectedAssemblies.Length > 0)
            {
                yield return new AddReferencesResult(selectedAssemblies);
            }
        }

        public static IEnumerable<IResult> CopyReferences()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = CShell.Constants.AssemblyFileFilter;
            dialog.Multiselect = true;
            yield return Show.Dialog(dialog);
            if (dialog.FileNames != null && dialog.FileNames.Length > 0)
            {
                yield return new CopyReferencesResult(dialog.FileNames);
            }
        }

        public static IEnumerable<IResult> MangePackages()
        {
            var windowSettings = new Dictionary<string, object> { { "SizeToContent", SizeToContent.Manual }, { "Width", 500.0 }, { "Height", 500.0 } };
            var dialog = new AssemblyPackagesViewModel();
            yield return Show.Dialog(dialog, windowSettings);
        }


        private FolderViewModel GetSelectedFolder()
        {
            var workspaceViewModel = IoC.Get<WorkspaceViewModel>();
            var fileRoot = (FolderViewModel)workspaceViewModel.Tree.Children.First(vm => vm is FolderViewModel);
            var selected = (FolderViewModel)fileRoot.GetAllChildren().FirstOrDefault(vm => vm is FolderViewModel && vm.IsSelected);
            if (selected == null || selected is FolderBinViewModel || selected is FolderPackagesViewModel)
                selected = fileRoot;
            return selected;
        }
	}
}