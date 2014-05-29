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