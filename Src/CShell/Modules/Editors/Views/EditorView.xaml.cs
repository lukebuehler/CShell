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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using CShell.Framework.Services;
using CShell.Modules.Editors.ViewModels;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;

namespace CShell.Modules.Editors.Views
{
	/// <summary>
	/// Interaction logic for EditorView.xaml
	/// </summary>
	public partial class EditorView : UserControl
	{
		public EditorView()
		{
			InitializeComponent();
		}

        #region Commend / Uncomment
        public void Comment()
        {
            var document = textEditor.Document;
            var start = document.GetLineByOffset(textEditor.SelectionStart);
            var end = document.GetLineByOffset(textEditor.SelectionStart + textEditor.SelectionLength);
            using (document.RunUpdate())
            {
                for (DocumentLine line = start; line!=null && line.LineNumber <= end.LineNumber; line = line.NextLine)
                {
                    if(!IsLineCommented(line))
                        document.Insert(line.Offset, "//");
                }
            }
        }

        public void Uncomment()
        {
            var document = textEditor.Document;
            var start = document.GetLineByOffset(textEditor.SelectionStart);
            var end = document.GetLineByOffset(textEditor.SelectionStart + textEditor.SelectionLength);
            using (document.RunUpdate())
            {
                for (DocumentLine line = start; line != null && line.LineNumber <= end.LineNumber; line = line.NextLine)
                {
                    if (IsLineCommented(line))
                        document.Remove(line.Offset, 2);
                }
            }
        }

        private bool IsLineCommented(DocumentLine line)
        {
            var lineText = textEditor.Document.GetText(line.Offset, line.Length);
            var trimmed = lineText.Trim();
            return trimmed.IndexOf("//",0).Equals(0);
        }
        #endregion
    }//end class
}
