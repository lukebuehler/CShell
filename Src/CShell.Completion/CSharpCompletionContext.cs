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
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;

namespace CShell.Completion
{
    public sealed class CSharpCompletionContext
    {
        public readonly IDocument OriginalDocument;
        public readonly int OriginalOffset;
        public readonly string[] OriginalNamespaces;

        public readonly int Offset;
        public readonly IDocument Document;
        public readonly ICompilation Compilation;
        public readonly IProjectContent ProjectContent;
        public readonly CSharpResolver Resolver;
        public readonly CSharpTypeResolveContext TypeResolveContextAtCaret;
        public readonly ICompletionContextProvider CompletionContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpCompletionContext"/> class.
        /// </summary>
        /// <param name="document">The document, make sure the FileName property is set on the document.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="projectContent">Content of the project.</param>
        /// <param name="namespaces">The refernced namespaces.</param>
        public CSharpCompletionContext(IDocument document, int offset, IProjectContent projectContent, string[] namespaces = null)
        {
            OriginalDocument = document;
            OriginalOffset = offset;
            OriginalNamespaces = namespaces;

            //if the document is a c# script we have to soround the document with some code.
            Document = PrepareCompletionDocument(document, ref offset, namespaces);
            Offset = offset;

            var syntaxTree = new CSharpParser().Parse(Document, Document.FileName);
            syntaxTree.Freeze();
            var unresolvedFile = syntaxTree.ToTypeSystem();

            ProjectContent = projectContent.AddOrUpdateFiles(unresolvedFile);
            //note: it's important that the project content is used that is returned after adding the unresolved file
            Compilation = ProjectContent.CreateCompilation();

            var location = Document.GetLocation(Offset);
            Resolver = unresolvedFile.GetResolver(Compilation, location);
            TypeResolveContextAtCaret = unresolvedFile.GetTypeResolveContext(Compilation, location);
            CompletionContextProvider = new DefaultCompletionContextProvider(Document, unresolvedFile);
        }

        private static Regex replaceRegex = new Regex("[^a-zA-Z0-9_]");
        private static IDocument PrepareCompletionDocument(IDocument document, ref int offset, string[] namespaces = null)
        {
            if (String.IsNullOrEmpty(document.FileName))
                return document;

            //if the code is just a script it it will contain no namestpace, class and method structure and so the code completion will not work properly
            // for it to work we have to suround the code with the appropriate code structure
            //we only process the file if its a .csx file
            var fileExtension = Path.GetExtension(document.FileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(document.FileName);
            if (String.IsNullOrEmpty(fileExtension) || String.IsNullOrEmpty(fileNameWithoutExtension))
                return document;

            if (fileExtension.ToLower() == ".csx")
            {
                var className = replaceRegex.Replace(fileNameWithoutExtension, "");
                className = className.TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9'); //there can be no number at the beginning of the class name
                var header = "";
                if (namespaces != null && namespaces.Length > 0)
                {
                    foreach (var ns in namespaces)
                        header += "using " + ns + "; ";
                    header += Environment.NewLine;
                }
                header += "static class " + className + " {" + Environment.NewLine;
                header += "static void Script(){" + Environment.NewLine;
                var footer = "";
                footer += Environment.NewLine + "}}";

                var code = header + document.Text + footer;
                offset += header.Length;
                return new ReadOnlyDocument(new StringTextSource(code), document.FileName);
            }
            return document;
        }
    }

}
