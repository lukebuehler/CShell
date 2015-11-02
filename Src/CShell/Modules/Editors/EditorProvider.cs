using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using CShell.Framework;
using CShell.Framework.Services;
using CShell.Modules.Editors.ViewModels;

namespace CShell.Modules.Editors
{
	[Export(typeof(IDocumentProvider))]
	public class EditorProvider : IDocumentProvider
	{
		private readonly List<string> extensions = new List<string>
        {
            ".cshell",
            ".cs",
            ".csx",
            ".txt",
            ".cmd",
            ".xml",
            ".config",
        };

        [Import]
        public CShell.Workspace Workspace { get; set; }

		public bool Handles(Uri uri)
		{
			var extension = Path.GetExtension(uri.AbsolutePath);
			return uri.Scheme == "file" && extensions.Contains(extension);
		}

        public IDocument Create(Uri uri)
		{
			var editor = new EditorViewModel(Workspace);
			editor.Open(uri);
			return editor;
		}
	}
}