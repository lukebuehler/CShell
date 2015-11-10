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

        public ITextDocument Document { get; private set; }

        [Import]
        public IShell Shell { get; set; }

        [Import]
        public CShell.Workspace Workspace { get; set; }

        public AddReferencesResult(ITextDocument doc, string file)
        {
            this.Document = doc;
            this.assemblyPaths = new[] { file };
        }

        public AddReferencesResult(ITextDocument doc, IEnumerable<string> files)
        {
            this.Document = doc;
            this.assemblyPaths = files;
        }

        public AddReferencesResult(ITextDocument doc, AssemblyName assemblyName)
        {
            this.Document = doc;
            this.assemblyNames = new[] { assemblyName };
        }

        public AddReferencesResult(ITextDocument doc, IEnumerable<AssemblyName> assemblyNames)
        {
            this.Document = doc;
            this.assemblyNames = assemblyNames;
        }

        public override void Execute(Caliburn.Micro.CoroutineExecutionContext context)
        {
            Framework.Services.Execute.OnUIThreadEx(() =>
            {
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
                        Document.Prepend(refsToInsert);
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
