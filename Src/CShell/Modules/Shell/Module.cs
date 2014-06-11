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
using System.Windows.Input;
using CShell.Framework;
using CShell.Framework.Menus;
using CShell.Framework.Results;
using CShell.Framework.Services;
using Caliburn.Micro;
using Microsoft.Win32;

namespace CShell.Modules.Shell
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
        public Module()
	    {
	        Order = 1;
	    }

        public override void Configure(IModuleConfiguration configuration)
        { }

        public override void Start()
        {
		    var openWorkspace = new MenuItem("Open Workspace...", OpenWorkspace);
            var openFile = new MenuItem("_Open File...", OpenFile)
                .WithIcon("/Resources/Icons/Open.png")
                .WithGlobalShortcut(ModifierKeys.Control, Key.O);
            var closeFile = new MenuItem("_Close", CloseFile);
		    var save = new MenuItem("Save", Save)
		        .WithIcon("Resources/Icons/Icons.16x16.SaveIcon.png")
		        .WithGlobalShortcut(ModifierKeys.Control, Key.S);
		    var saveAs = new MenuItem("Save As...", SaveAs);
            var saveAll = new MenuItem("Save All", SaveAll)
                .WithIcon("Resources/Icons/Icons.16x16.SaveAllIcon.png")
                .WithGlobalShortcut(ModifierKeys.Control | ModifierKeys.Shift, Key.S);
		    var exit = new MenuItem("E_xit", Exit);
            
            //populate the menu
            MainMenu.First(item=>item.Name == "File")
                .Add(
                    openWorkspace,
                    openFile,
                    MenuItemBase.Separator,
                    closeFile,
                    MenuItemBase.Separator,
                    save,
                    saveAs,
                    saveAll,
                    MenuItemBase.Separator,
                    exit
                );

            //populate the toolbar
            ToolBar.Add(
                    openFile,
                    save,
                    saveAll
                );
		}

        //private IEnumerable<IResult> NewWorkspace()
        //{
        //    var dialog = new SaveFileDialog();
        //    dialog.Filter = CShell.Constants.CShellFileTypes;
        //    dialog.DefaultExt = CShell.Constants.CShellFileExtension;
        //    yield return Show.Dialog(dialog);
        //    yield return new CloseWorkspaceResult();
        //    yield return new OpenWorkspaceResult(dialog.FileName);
        //}

        private IEnumerable<IResult> OpenWorkspace()
        {
            var folderResult = Show.FolderDialog();
            yield return folderResult;
            var folder = folderResult.SelectedFolder;
            yield return new ChangeWorkspaceResult(folder);
        }


	    private IEnumerable<IResult> NewFile()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = CShell.Constants.FileTypes;
            dialog.DefaultExt = CShell.Constants.DefaultExtension;
            yield return Show.Dialog(dialog);
            yield return Show.Document(dialog.FileName);
        }

        private IEnumerable<IResult> OpenFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = CShell.Constants.FileFilter;
            yield return Show.Dialog(dialog);
            yield return Show.Document(dialog.FileName);
        }

        private IEnumerable<IResult> CloseFile()
        {
            if(Shell.ActiveItem != null)
                Shell.ActiveItem.TryClose();
            yield break;
        }

        private IEnumerable<IResult> Save()
        {
            var doc = Shell.ActiveItem as IDocument;
            if (doc != null)
            {
                yield return new SaveDocumentResult(doc);
            }
        }

        private IEnumerable<IResult> SaveAs()
        {
            var doc = Shell.ActiveItem as IDocument;
            if (doc != null)
            {
                var dialog = new SaveFileDialog();
                yield return Show.Dialog(dialog);
                yield return new SaveDocumentResult(doc, dialog.FileName);
            }
        }

        private IEnumerable<IResult> SaveAll()
        {
            return Shell.Documents.Select(doc => new SaveDocumentResult(doc));
        }


        private IEnumerable<IResult> Exit()
        {
            Shell.Close();
            yield break;
        }
	}
}