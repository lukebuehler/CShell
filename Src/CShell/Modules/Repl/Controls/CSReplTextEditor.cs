using CShell.Completion;
using ICSharpCode.NRefactory.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CShell.Modules.Repl.Controls
{
    public class CSReplTextEditor : CodeTextEditor
    {
        public CSRepl Repl { get; set; }

        protected override IDocument GetCompletionDocument(out int offset)
        {
            return Repl.GetCompletionDocument(out offset);
        }
    }
}
