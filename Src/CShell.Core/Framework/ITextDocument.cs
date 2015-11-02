using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CShell.Framework
{
    public interface ITextDocument : IDocument
    {
        void Undo();
        void Redo();
        void Cut();
        void Copy();
        void Paste();
        void SelectAll();

        /// <summary>
        /// Selects a section in the text.
        /// </summary>
        /// <param name="start">The start index of the selection.</param>
        /// <param name="length">The length of the selection.</param>
        void Select(int start, int length);

        void Comment();
        void Uncomment();

        string Text { get; set; }
    }
}
