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

// This file is based on code from the SharpDevelop project:
//   Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \Doc\sharpdevelop-copyright.txt)
//   This code is distributed under the GNU LGPL (for details please see \Doc\COPYING.LESSER.txt)
#endregion

using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory.CSharp;

namespace CShell.Completion
{
    /// <summary>
    /// Output formatter that creates a dictionary from AST nodes to segments in the output text.
    /// </summary>
    public class SegmentTrackingOutputFormatter : TextWriterTokenWriter
    {
        Dictionary<AstNode, ICSharpCode.AvalonEdit.Document.ISegment> segments = new Dictionary<AstNode, ICSharpCode.AvalonEdit.Document.ISegment>();
        Stack<int> startOffsets = new Stack<int>();
        readonly StringWriter stringWriter;

        public IDictionary<AstNode, ICSharpCode.AvalonEdit.Document.ISegment> Segments
        {
            get { return segments; }
        }

        public SegmentTrackingOutputFormatter(StringWriter stringWriter)
            : base(stringWriter)
        {
            this.stringWriter = stringWriter;
        }

        public static IDictionary<AstNode, ICSharpCode.AvalonEdit.Document.ISegment> WriteNode(StringWriter writer, AstNode node, CSharpFormattingOptions policy, ICSharpCode.AvalonEdit.TextEditorOptions options)
        {
            var formatter = new SegmentTrackingOutputFormatter(writer);
            formatter.IndentationString = options.IndentationString;
            var visitor = new CSharpOutputVisitor(formatter, policy);
            node.AcceptVisitor(visitor);
            return formatter.Segments;
        }

        public override void StartNode(AstNode node)
        {
            base.StartNode(node);
            startOffsets.Push(stringWriter.GetStringBuilder().Length);
        }

        public override void EndNode(AstNode node)
        {
            int startOffset = startOffsets.Pop();
            StringBuilder b = stringWriter.GetStringBuilder();
            int endOffset = b.Length;
            while (endOffset > 0 && b[endOffset - 1] == '\r' || b[endOffset - 1] == '\n')
                endOffset--;
            segments.Add(node, new TextSegment { StartOffset = startOffset, EndOffset = endOffset });
            base.EndNode(node);
        }
    }
}
