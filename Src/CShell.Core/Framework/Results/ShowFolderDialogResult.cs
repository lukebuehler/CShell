using System;
using System.Windows.Forms;
using Caliburn.Micro;
using Microsoft.Win32;

namespace CShell.Framework.Results
{
	public class ShowFolderDialogResult : IResult
	{
	    public event EventHandler<ResultCompletionEventArgs> Completed;

	    public ShowFolderDialogResult()
	    { }

        public ShowFolderDialogResult(string selectedFolder)
        {
            this.SelectedFolder = selectedFolder;
        }

	    public string SelectedFolder { get; set; }

	    public void Execute(CoroutineExecutionContext context)
	    {
	        var dialog = new FolderBrowserDialog();
            if(!String.IsNullOrEmpty(SelectedFolder))
                dialog.SelectedPath = SelectedFolder;
	        var result = dialog.ShowDialog();
	        SelectedFolder = dialog.SelectedPath;
			Completed(this, new ResultCompletionEventArgs { WasCancelled = result != DialogResult.OK });
		}
	}
}