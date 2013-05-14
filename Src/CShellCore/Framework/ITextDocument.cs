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
