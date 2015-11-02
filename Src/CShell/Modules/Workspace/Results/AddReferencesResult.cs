using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CShell.Framework;
using CShell.Framework.Results;
using CShell.Framework.Services;
using Microsoft.Win32;

namespace CShell.Modules.Workspace.Results
{
    public class AddReferencesResult : ResultBase
    {
        private readonly IEnumerable<string> assemblyPaths;
        private readonly IEnumerable<AssemblyName> assemblyNames;

        public ITextDocument Document { get; set; }
        public string FilePath { get; set; }

        [Import]
        public IShell Shell { get; set; }

        [Import]
        public CShell.Workspace Workspace { get; set; }

        public AddReferencesResult(string file)
        {
            this.assemblyPaths = new[] { file };
        }

        public AddReferencesResult(IEnumerable<string> files)
        {
            this.assemblyPaths = files;
        }

        public AddReferencesResult(AssemblyName assemblyName)
        {
            this.assemblyNames = new[] { assemblyName };
        }

        public AddReferencesResult(IEnumerable<AssemblyName> assemblyNames)
        {
            this.assemblyNames = assemblyNames;
        }

        public override void Execute(Caliburn.Micro.CoroutineExecutionContext context)
        {
            Task.Run(() =>
            {
                //if the document is null, open the references document
                if (Document == null)
                {
                    if (FilePath == null)
                        FilePath = Constants.ReferencesFile;
                    Document = CShell.Shell.GetTextDoc(FilePath);
                }
            })
            .ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    OnCompleted(t.Exception);
                    return;
                }
                try
                {
                    var refsToInsert = "";
                    if (assemblyPaths != null)
                    {
                        foreach (var path in assemblyPaths)
                        {
                            refsToInsert += "#r \"" + GetReferencePath(path) + "\"" + Environment.NewLine;
                        }
                    }
                    if (assemblyNames != null)
                    {
                        foreach (var assemblyName in assemblyNames)
                        {
                            refsToInsert += "#r \"" + assemblyName.FullName + "\"" + Environment.NewLine;
                        }
                    }
                    if (!String.IsNullOrEmpty(refsToInsert))
                    {
                        Document.Text = refsToInsert + Document.Text;
                        Document.Save();

                        Workspace.ReplExecutor.Execute(refsToInsert, Document.DisplayName);
                    }

                    OnCompleted(null);
                }
                catch (Exception ex)
                {
                    OnCompleted(ex);
                }
            });
            
        }

        private string GetReferencePath(string path)
        {
            var fullPath = Path.GetFullPath(path);
            var fullPathDir = Path.GetDirectoryName(fullPath);
            var fullBinPathDir = Path.Combine(Workspace.WorkspaceDirectory, Constants.BinFolder);
            if (fullPathDir.Equals(fullBinPathDir, StringComparison.OrdinalIgnoreCase))
                return Path.GetFileName(fullPath);
            else
                return path;
        }

    }//end class
}
