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