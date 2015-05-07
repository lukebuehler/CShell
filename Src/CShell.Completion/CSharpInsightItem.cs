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

using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using CShell.Completion.DataItems;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;

namespace CShell.Completion
{
    public sealed class CSharpInsightItem
    {
        public readonly IParameterizedMember Method;

        public CSharpInsightItem(IParameterizedMember method)
        {
            this.Method = method;
        }

        TextBlock header;

        public object Header
        {
            get
            {
                if (header == null)
                {
                    header = new TextBlock();
                    GenerateHeader();
                }
                return header;
            }
        }

        int highlightedParameterIndex = -1;

        public void HighlightParameter(int parameterIndex)
        { 
            if (highlightedParameterIndex == parameterIndex)
                return;
            this.highlightedParameterIndex = parameterIndex;
            if (header != null)
                GenerateHeader();
        }

        void GenerateHeader()
        {
            CSharpAmbience ambience = new CSharpAmbience();
            ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
            var stringBuilder = new StringBuilder();
            var formatter = new ParameterHighlightingOutputFormatter(stringBuilder, highlightedParameterIndex);
            ambience.ConvertEntity(Method, formatter, FormattingOptionsFactory.CreateSharpDevelop());
            var inlineBuilder = new HighlightedInlineBuilder(stringBuilder.ToString());
            inlineBuilder.SetFontWeight(formatter.parameterStartOffset, formatter.parameterLength, FontWeights.Bold);
            header.Inlines.Clear();
            header.Inlines.AddRange(inlineBuilder.CreateRuns());
        }

        public object Content
        {
            get { return Documentation; }
        }

        private string documentation;
        public string Documentation
        {
            get
            {
                if (documentation == null)
                {
                    if (Method.Documentation == null)
                        documentation = "";
                    else
                        documentation = EntityCompletionData.XmlDocumentationToText(Method.Documentation);
                }
                return documentation;
            }
        }

        sealed class ParameterHighlightingOutputFormatter : TextWriterTokenWriter
        {
            StringBuilder b;
            int highlightedParameterIndex;
            int parameterIndex;
            internal int parameterStartOffset;
            internal int parameterLength;

            public ParameterHighlightingOutputFormatter(StringBuilder b, int highlightedParameterIndex)
                : base(new StringWriter(b))
            {
                this.b = b;
                this.highlightedParameterIndex = highlightedParameterIndex;
            }

            public override void StartNode(AstNode node)
            {
                if (parameterIndex == highlightedParameterIndex && node is ParameterDeclaration)
                {
                    parameterStartOffset = b.Length;
                }
                base.StartNode(node);
            }

            public override void EndNode(AstNode node)
            {
                base.EndNode(node);
                if (node is ParameterDeclaration)
                {
                    if (parameterIndex == highlightedParameterIndex)
                        parameterLength = b.Length - parameterStartOffset;
                    parameterIndex++;
                }
            }
        }
    }
}
