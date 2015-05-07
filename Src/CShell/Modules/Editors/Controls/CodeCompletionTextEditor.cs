using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShell.Completion;
using CShell.Framework.Services;

namespace CShell.Modules.Editors.Controls
{
    public class CodeCompletionTextEditor : CodeTextEditor
    {
        public IReplScriptExecutor ReplExecutor { get; set; }

        protected override string[] GetNamespaces()
        {
            if (ReplExecutor != null)
            {
                var fileExtension = Path.GetExtension(FileName);
                if (fileExtension != null && fileExtension.ToLower() == ".csx")
                {
                    return ReplExecutor.Namespaces.ToArray();
                }
            }
            return null;
        }
    }
}
