using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CShell.Framework.Results;
using Microsoft.Win32;

namespace CShell.Modules.Workspace.Results
{
    public class AddFilesResult : ResultBase
    {
        private readonly IEnumerable<string> files;

        public AddFilesResult(string file)
        {
            this.files = new string[] {file};
        }

        public AddFilesResult(IEnumerable<string> files)
        {
            this.files = files;
        }

        public override void Execute(Caliburn.Micro.CoroutineExecutionContext context)
        {
            foreach (var file in files)
            {
                if(!File.Exists(file))
                {
                    CShell.Workspace.CreateEmptyFile(file);
                }
            }
            OnCompleted(null);
        }
    }
}
