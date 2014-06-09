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
using System.ComponentModel.Composition;
using System.Linq;
using CShell.Framework.Services;
using Caliburn.Micro;

namespace CShell.Framework.Results
{
	public class OpenDocumentResult : OpenResultBase<IDocument>
	{
		private readonly IDocument document;
        private readonly Type documentType;
		private readonly Uri uri;

		[Import]
		private IShell shell;

		public OpenDocumentResult(IDocument document)
		{
            this.document = document;
		}

		public OpenDocumentResult(string path)
		{
		    if (path == null) throw new ArgumentNullException("path");
		    uri = new Uri(System.IO.Path.GetFullPath(path));
		}

	    public OpenDocumentResult(Uri uri)
	    {
	        this.uri = uri;
	    }

        public OpenDocumentResult(Type documentType)
		{
            this.documentType = documentType;
		}

        public override void Execute(CoroutineExecutionContext context)
		{
			var doc = document ??
				(uri == null
					? (IDocument)IoC.GetInstance(documentType, null)
					: shell.GetDocument(uri));

            if (doc == null)
			{
				OnCompleted(null);
				return;
			}

			if (_setData != null)
                _setData(doc);

			if (_onConfigure != null)
                _onConfigure(doc);

            doc.Deactivated += (s, e) =>
			{
				if (_onShutDown != null)
                    _onShutDown(doc);

				OnCompleted(null);
			};

            shell.OpenDocument(doc);
		}

       
	}
}