using System;
using System.Windows.Input;
using Caliburn.Micro;

namespace CShell.Framework
{
	public interface IDocument : IScreen
	{
        ICommand CloseCommand { get; }

	    Uri Uri { get; }

	    bool IsDirty { get; }
        void Save();
        void SaveAs(string newFile);
	}
}