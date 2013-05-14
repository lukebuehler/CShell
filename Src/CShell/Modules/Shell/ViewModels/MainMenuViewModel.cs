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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CShell.Framework.Menus;
using CShell.Framework.Results;
using CShell.Framework.Services;
using Caliburn.Micro;
using Microsoft.Win32;

namespace CShell.Modules.Shell.ViewModels
{
	[Export(typeof(IMenu))]
	public class MainMenuViewModel : MenuModel
	{
		[Import] 
		private IShell _shell;

		public MainMenuViewModel()
		{
			Add(
                new MenuItem("_File"),
                new MenuItem("_Edit"),
				new MenuItem("_View"),
                new MenuItem("_Workspace"),
                new MenuItem("Ev_aluate"),
                new MenuItem("_Help"));
		}

		private IEnumerable<IResult> OpenFile()
		{
			var dialog = new OpenFileDialog();
			yield return Show.Dialog(dialog);
			yield return Show.Document(dialog.FileName);
		}

		private IEnumerable<IResult> Exit()
		{
			_shell.Close();
			yield break;
		}
	}
}