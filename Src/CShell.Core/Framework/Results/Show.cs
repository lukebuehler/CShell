using System.Collections.Generic;
using Microsoft.Win32;

namespace CShell.Framework.Results
{
	public static class Show
	{
        public static ShowDialogResult Dialog(object dialogViewModel, IDictionary<string, object> setting = null)
        {
            return new ShowDialogResult(dialogViewModel){Settings = setting};
        }

        public static ShowDialogResult Dialog<TViewModel>(IDictionary<string, object> setting = null)
        {
            return new ShowDialogResult(typeof(TViewModel)){Settings = setting};
        }

		public static ShowCommonDialogResult Dialog(CommonDialog commonDialog)
		{
			return new ShowCommonDialogResult(commonDialog);
		}

        public static ShowFolderDialogResult FolderDialog(string selectedFolder = null)
        {
            return new ShowFolderDialogResult(selectedFolder);
        }

		public static ShowToolResult<TTool> Tool<TTool>()
			where TTool : ITool
		{
			return new ShowToolResult<TTool>();
		}

		public static ShowToolResult<TTool> Tool<TTool>(TTool tool)
			where TTool : ITool
		{
			return new ShowToolResult<TTool>(tool);
		}

		public static OpenDocumentResult Document(IDocument document)
		{
			return new OpenDocumentResult(document);
		}

		public static OpenDocumentResult Document(string path)
		{
			return new OpenDocumentResult(path);
		}

		public static OpenDocumentResult Document<T>()
				where T : IDocument
		{
			return new OpenDocumentResult(typeof(T));
		}
	}
}