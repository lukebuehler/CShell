using System.Collections.Generic;
using System.Windows.Media;
using System.Xml;
using CShell.Framework.Menus;
using Caliburn.Micro;

namespace CShell.Framework.Services
{
    public interface IShell : IConductor
	{
		string Title { get; set; }
		ImageSource Icon { get; set; }
        IMenu MainMenu { get; }
        IToolBar ToolBar { get; }
		IStatusBar StatusBar { get; }

		IScreen ActiveItem { get; }
        IEnumerable<IDocument> Documents { get; }

        IEnumerable<ITool> Tools { get; }
        void ShowTool(ITool model);

		void OpenDocument(IDocument model);
		void CloseDocument(IDocument document);
		void ActivateDocument(IDocument document);

        void Opened(string[] args);
		void Close();

        void SaveLayout(XmlWriter xmlWriter);
        void LoadLayout(XmlReader xmlReader);
        CShell.Workspace.WindowLocation GetWindowLocation();
        void RestoreWindowLocation(CShell.Workspace.WindowLocation windowLocation);
	}
}