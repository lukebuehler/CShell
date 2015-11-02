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
