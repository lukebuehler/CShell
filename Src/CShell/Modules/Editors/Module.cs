#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2012  Arnova Asset Management Ltd., Lukas Buhler
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
using CShell.Modules.Editors.ViewModels;
using Caliburn.Micro;

namespace CShell.Modules.Editors
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
        public Module()
        {
            Order = 4;
        }

        public override void Configure(IModuleConfiguration configuration)
        {}

        public override void Start()
        {
            //Edit
            var undo = new MenuItem("Undo", Undo).WithIcon("Resources/Icons/Icons.16x16.UndoIcon.png")
                .WithGlobalShortcut(ModifierKeys.Control, Key.Z);
            var redo = new MenuItem("Redo", Redo).WithIcon("Resources/Icons/Icons.16x16.RedoIcon.png")
                .WithGlobalShortcut(ModifierKeys.Control, Key.Y);
            var cut = new MenuItem("Cut", Cut).WithIcon("Resources/Icons/Icons.16x16.CutIcon.png")
                .WithGlobalShortcut(ModifierKeys.Control, Key.X);
            var copy = new MenuItem("Copy", Copy).WithIcon("Resources/Icons/Icons.16x16.CopyIcon.png")
                .WithGlobalShortcut(ModifierKeys.Control, Key.C);
            var paste = new MenuItem("Paste", Paste).WithIcon("Resources/Icons/Icons.16x16.PasteIcon.png")
                .WithGlobalShortcut(ModifierKeys.Control, Key.V);
            var selectAll = new MenuItem("Select All", SelectAll)
                .WithGlobalShortcut(ModifierKeys.Control, Key.A);
            var comment = new MenuItem("Comment Selection", Comment).WithIcon("Resources/Icons/Icons.16x16.CommentRegion.png")
                .WithGlobalShortcut(ModifierKeys.Control, Key.K);
            var uncomment = new MenuItem("Uncomment Selection", Uncomment).WithIcon("Resources/Icons/Icons.16x16.CommentRegion.png")
                .WithGlobalShortcut(ModifierKeys.Control, Key.U);
            //populate the menu
            MainMenu.First(item => item.Name == "Edit")
                .Add(
                    undo,
                    redo,
                    MenuItemBase.Separator,
                    cut,
                    copy,
                    paste,
                    MenuItemBase.Separator,
                    selectAll,
                    MenuItemBase.Separator,
                    comment,
                    uncomment
                );
            //populate the toolbar
            ToolBar.Add(
                    MenuItemBase.Separator,
                    cut,
                    copy,
                    paste,
                    MenuItemBase.Separator,
                    undo,
                    redo
                );


            //eval code
		    var runFile = new MenuItem("Run", ExecuteFile)
                .WithIcon("Resources/Icons/Icons.16x16.RunAllIcon.png")
		        .WithGlobalShortcut(ModifierKeys.Alt | ModifierKeys.Shift, Key.Enter);
            var runSelection = new MenuItem("Run Selection", ExecuteSelection)
                .WithIcon("Resources/Icons/Icons.16x16.RunProgramIcon.png")
                .WithGlobalShortcut(ModifierKeys.Alt, Key.Enter);
            //populate the menu
            MainMenu.First(item => item.Name == "Evaluate")
                .Add(
                    runFile,
                    runSelection
                );
            //populate the toolbar
            ToolBar.Add(
                    MenuItemBase.Separator,
                    runFile,
                    runSelection
                );
            
		}

        #region Evaluate Code
        public IEnumerable<IResult> ExecuteFile()
	    {
	        var editor = Shell.ActiveItem as EditorViewModel;
            if(editor != null)
            {
                var code = editor.Text;
                yield return Run.Code(code, editor.File);
            }
	    }

        public IEnumerable<IResult> ExecuteSelection()
        {
            var editor = Shell.ActiveItem as EditorViewModel;
            if (editor != null)
            {
                var code = editor.GetSelectionOrCurrentLine();
                yield return Run.Code(code, editor.File);
            }
        }
        #endregion

        #region Edit
        public IEnumerable<IResult> Undo()
        {
            return TryDocumentAction(doc => doc.Undo());
        }

        public IEnumerable<IResult> Redo()
        {
            return TryDocumentAction(doc => doc.Redo());
        }

        public IEnumerable<IResult> Cut()
        {
            return TryDocumentAction(doc => doc.Cut());
        }

        public IEnumerable<IResult> Copy()
        {
            return TryDocumentAction(doc => doc.Copy());
        }

        public IEnumerable<IResult> Paste()
        {
            return TryDocumentAction(doc => doc.Paste());
        }

        public IEnumerable<IResult> SelectAll()
        {
            return TryDocumentAction(doc => doc.SelectAll());
        }

        public IEnumerable<IResult> Comment()
        {
            return TryDocumentAction(doc => doc.Comment());
        }

        public IEnumerable<IResult> Uncomment()
        {
            return TryDocumentAction(doc => doc.Uncomment());
        }

        private IEnumerable<IResult> TryDocumentAction(Action<ITextDocument> action)
        {
            var doc = Shell.ActiveItem as ITextDocument;
            if (doc != null)
                action(doc);
            return null;
        }
	    #endregion


       
    }
}