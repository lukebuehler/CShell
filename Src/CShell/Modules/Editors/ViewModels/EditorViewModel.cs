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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using CShell.Completion;
using CShell.Framework;
using CShell.Framework.Services;
using CShell.Modules.Editors.Controls;
using CShell.Modules.Editors.Views;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Execute = CShell.Framework.Services.Execute;

namespace CShell.Modules.Editors.ViewModels
{
	public class EditorViewModel : Document, ITextDocument
	{
	    private readonly CShell.Workspace workspace;
	    private string originalText;
		private string path;
		private string fileName;
		private bool isDirty;
	    private CodeCompletionTextEditor textEditor;
	    private EditorView editorView;

	    public EditorViewModel(CShell.Workspace workspace)
	    {
	        this.workspace = workspace;
	    }

	    public string File
	    {
            get { return path; }
	    }

        public override Uri Uri { get; set; }

		public override bool IsDirty
		{
			get { return isDirty; }
            set
            {
                if (value == isDirty)
                    return;

                isDirty = value;
                if (isDirty)
                    DisplayName = fileName + "*";
                else
                    DisplayName = fileName;
                NotifyOfPropertyChange(() => IsDirty);
                NotifyOfPropertyChange(() => DisplayName);
            }
		}

		public override void CanClose(System.Action<bool> callback)
		{
		    //callback(!IsDirty);
            if (!IsDirty)
            {
                callback(true);
                return;
            }

            Execute.OnUIThreadEx(() =>
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save this document before closing?" + Environment.NewLine + Uri.AbsolutePath, "Confirmation", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    Save();
                    callback(true);
                }
                else if (result == MessageBoxResult.No)
                {
                    callback(true);
                }
                else
                {
                    // Cancel
                    callback(false);
                }
            });
		}

		public void Open(Uri uri)
		{
		    this.Uri = uri;
		    var decodedPath = Uri.UnescapeDataString(uri.AbsolutePath);
			this.path = Path.GetFullPath(decodedPath);
			fileName = Path.GetFileName(path);
		    DisplayName = fileName;
		}

		protected override void OnViewLoaded(object view)
		{
            editorView = (EditorView)view;
            textEditor = editorView.textEditor;
            if(System.IO.File.Exists(path))
                textEditor.OpenFile(path);
            originalText = textEditor.Text;

            textEditor.TextChanged += delegate
			{
                IsDirty = string.Compare(originalText, textEditor.Text) != 0;
			};

            //some other settings
		    var extension = Path.GetExtension(path);
		    extension = extension == null ? "" : extension.ToLower();
		    textEditor.ShowLineNumbers = true;
            textEditor.SyntaxHighlighting = GetHighlighting(extension);

		    if (workspace != null && workspace.ReplExecutor.DocumentCompletion != null && (extension == ".cs" || extension == ".csx"))
		    {
		        textEditor.Completion = workspace.ReplExecutor.DocumentCompletion;
		        textEditor.ReplExecutor = workspace.ReplExecutor;
		    }

		    //debug to see what commands are available in the editor
            //var c = textEditor.TextArea.CommandBindings;
            //foreach (System.Windows.Input.CommandBinding cmd in c)
            //{
            //    var rcmd = cmd.Command as RoutedCommand;
            //    if(rcmd != null)
            //    {
            //        Debug.Print(rcmd.Name + "  "+ rcmd.InputGestures.ToString());
            //    }
            //}
		}

        public override void Save()
        {
            Execute.OnUIThreadEx(() =>
            {
                textEditor.Save(path);
                originalText = textEditor.Text;
                IsDirty = false;
            });
        }

        public override void SaveAs(string newFile)
        {
            Execute.OnUIThreadEx(() =>
            {
                textEditor.Save(newFile);
                this.path = newFile;
                fileName = Path.GetFileName(newFile);
                Uri = new Uri(System.IO.Path.GetFullPath(newFile));

                originalText = textEditor.Text;
                IsDirty = false;
                DisplayName = fileName;
                NotifyOfPropertyChange(() => DisplayName);
            });
        }

        public string GetSelectionOrCurrentLine()
        {
            var code = textEditor.SelectedText;
            int offsetLine;
            var doc = textEditor.Document;

            // if there is no selection, just use the current line
            if (string.IsNullOrEmpty(code))
            {
                offsetLine = doc.GetLocation(textEditor.CaretOffset).Line;
                var line = doc.GetLineByNumber(offsetLine);
                var lineText = doc.GetText(line.Offset, line.Length);
                code = lineText;
            }
            else
                offsetLine = doc.GetLocation(textEditor.SelectionStart + textEditor.SelectionLength).Line;

            textEditor.TextArea.Caret.Line = offsetLine + 1;
            textEditor.ScrollToLine(offsetLine + 1);
            return code;
        }

		public override bool Equals(object obj)
		{
			var other = obj as EditorViewModel;
		    return other != null && Uri == other.Uri;
		}

        private IHighlightingDefinition GetHighlighting(string fileExtension)
        {
            var def = HighlightingManager.Instance.GetDefinitionByExtension(fileExtension);
            //if the definition was not found try the custom extensions
            if(def == null)
            {
                switch (fileExtension)
                {
                    case ".cshell":
                    case ".csx":
                        def = HighlightingManager.Instance.GetDefinition("C#");
                        break;
                }
            }
            return def;

            //var resourceManager = IoC.Get<IResourceManager>();
            //resourceManager.GetBitmap("Resources/Icon.ico",
            //    Assembly.GetExecutingAssembly().GetAssemblyName());

            //using (Stream s = myAssembly.GetManifestResourceStream("MyHighlighting.xshd"))
            //{
            //    using (XmlTextReader reader = new XmlTextReader(s))
            //    {
            //        textEditor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            //    }
            //}
        }

        #region ITextDocument
        public void Undo()
        {
            Execute.OnUIThreadEx(()=>textEditor.Undo());
        }

        public void Redo()
        {
            Execute.OnUIThreadEx(() => textEditor.Redo());
        }

        public void Cut()
        {
            Execute.OnUIThreadEx(() => textEditor.Cut());
        }

        public void Copy()
        {
            Execute.OnUIThreadEx(() => textEditor.Copy());
        }

        public void Paste()
        {
            Execute.OnUIThreadEx(() => textEditor.Paste());
        }

        public void SelectAll()
        {
            Execute.OnUIThreadEx(() => textEditor.SelectAll());
        }

        public void Select(int start, int length)
        {
            start = Math.Abs(start);
            length = Math.Abs(length);
            Execute.OnUIThreadEx(() =>
            {
                if (start > textEditor.Document.TextLength)
                    start = textEditor.Document.TextLength - 1;
                if (start + length > textEditor.Document.TextLength)
                    length = textEditor.Document.TextLength - start;
                textEditor.Select(start, length);
            });
        }

        public void Comment()
        {
            Execute.OnUIThreadEx(() => editorView.Comment());
        }

        public void Uncomment()
        {
            Execute.OnUIThreadEx(() => editorView.Uncomment());
        }

        public string Text
        {
            get
            {
                var txt = "";
                Execute.OnUIThreadEx(() => txt = textEditor.Text);
                return txt;
            }
            set
            {
                Execute.OnUIThreadEx(() =>
                {
                    if (value == null)
                        value = "";
                    using(textEditor.Document.RunUpdate())
                    {
                        textEditor.Document.Text = value;
                    }
                });
            }
        }
        #endregion


       
    }
}