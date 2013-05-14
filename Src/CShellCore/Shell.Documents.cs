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
using System.IO;
using System.Linq;
using System.Text;
using CShell.Framework;
using CShell.Framework.Results;
using CShell.Framework.Services;
using Caliburn.Micro;

namespace CShell
{
    public static partial class Shell
    {
        /// <summary>
        /// Gets all available documents including sinks.
        /// </summary>
        public static IEnumerable<IDocument> GetDocuments()
        {
            var shell = shellLazy.Value;
            return shell.Documents.ToArray();
        }

        /// <summary>
        /// Gets or creates specific document and opens it.
        /// </summary>
        /// <param name="uri">The URI of the document. Can be a file path URI.</param>
        public static IDocument GetDocument(Uri uri)
        {
            return GetDocument(uri, false);
        }

        /// <summary>
        /// Gets or creates specific document and opens it.
        /// </summary>
        /// <param name="uri">The URI of the document. Can be a file path URI.</param>
        /// <param name="suppressOpen"><c>true</c> if the document should not be opened.</param>
        public static IDocument GetDocument(Uri uri, bool suppressOpen)
        {
            var doc = GetDocuments().FirstOrDefault(s => s.Uri == uri);
            if (doc == null)
            {
                doc = IoC.GetAllInstances(typeof(IDocumentProvider))
                    .Cast<IDocumentProvider>()
                    .Where(provider => provider.Handles(uri))
                    .Select(provider => provider.Create(uri))
                    .FirstOrDefault();

                if (doc != null && !suppressOpen)
                    shellLazy.Value.ActivateDocument(doc);
            }
            return doc;
        }

        /// <summary>
        /// Gets all available text documents.
        /// </summary>
        public static IEnumerable<ITextDocument> GetTextDocuments()
        {
            var shell = shellLazy.Value;
            return shell.Documents.OfType<ITextDocument>().ToArray();
        }

        /// <summary>
        /// Gets or creates specific text document and opens it.
        /// </summary>
        /// <param name="filePath">The path to the file. Can be relative to the root path of the workspace, e.g. "subfolder/file.csx"</param>
        public static ITextDocument GetTextDocument(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException("filePath");
            var uri = new Uri(System.IO.Path.GetFullPath(filePath));
            var doc = GetDocument(uri);
            return doc as ITextDocument;
        }

        /// <summary>
        /// Gets the current text document.
        /// </summary>
        public static ITextDocument TextDocument
        {
            get { return shellLazy.Value.ActiveItem as ITextDocument; }
        }

        #region Files
        /// <summary>
        /// Tries to open a file as a document.
        /// </summary>
        /// <param name="filePath">The path to the file. Can be relative to the root path of the workspace, e.g. "subfolder/file.csx"</param>
        /// <returns><c>true</c> if the file was found, otherwise <c>false</c>.</returns>
        public static bool TryOpen(string filePath)
        {
            if (Workspace == null)
                return false;
            if (File.Exists(filePath))
            {
                Open(filePath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Opens a file as a document.
        /// </summary>
        /// <param name="filePath">The path to the file. Can be relative to the root path of the workspace, e.g. "subfolder/file.csx"</param>
        /// <exception cref="ArgumentNullException">If the file path is null or empty.</exception>
        /// <exception cref="FileNotFoundException">If the file doesn't exist.</exception>
        public static void Open(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException("filePath");
            if(File.Exists(filePath))
                Show.Document(filePath).BeginExecute();
            else
                throw new FileNotFoundException("filePath not found.", filePath);
        }
        #endregion
    }
}
