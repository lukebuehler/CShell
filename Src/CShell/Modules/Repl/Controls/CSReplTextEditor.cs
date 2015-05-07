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

        protected override string[] GetNamespaces()
        {
            return Repl.ReplExecutor.Namespaces.ToArray();
        }

        protected override IDictionary<string, string> GetAdditionalCompletions()
        {
            if (Repl.ReplExecutor.Commands != null && Repl.ReplExecutor.Commands.Count > 0)
            {
                return Repl.ReplExecutor.Commands.ToDictionary(kv => ":"+kv.Key, kv=>kv.Value.Description);
            }
            return null;
        }
    }
}
