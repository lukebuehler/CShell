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
    public class CopyReferencesResult : ResultBase
    {
        private readonly IEnumerable<string> assemblyPaths;

        [Import]
        public IShell Shell { get; set; }

        [Import]
        public CShell.Workspace Workspace { get; set; }

        public CopyReferencesResult(string file)
        {
            this.assemblyPaths = new[] { file };
        }

        public CopyReferencesResult(IEnumerable<string> files)
        {
            this.assemblyPaths = files;
        }

        public override void Execute(Caliburn.Micro.CoroutineExecutionContext context)
        {
            try
            {
                var binDir = Path.Combine(Workspace.WorkspaceDirectory, Constants.BinFolder);
                if (!Directory.Exists(binDir))
                    Directory.CreateDirectory(binDir);
                var copiedReferences = new List<string>();
                foreach (var assemblyPath in assemblyPaths)
                {
                    if (File.Exists(assemblyPath))
                    {
                        var destFileName = Path.Combine(binDir, Path.GetFileName(assemblyPath));
                        File.Copy(assemblyPath, destFileName, true);
                        copiedReferences.Add(destFileName);
                    }
                }

                //make sure they are available for code completion
                Workspace.ReplExecutor.AddReferences(copiedReferences.ToArray());

                OnCompleted(null);
            }
            catch (Exception ex)
            {
                OnCompleted(ex);
            }
        }

      

    }//end class
}
