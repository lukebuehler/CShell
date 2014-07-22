using CShell.Framework.Services;
using CShell.Hosting.ReplCommands;
using ScriptCs.Contracts;

namespace CShell.Hosting.ReplCommands
{
    public class ClearCommand : IReplCommandWithInfo
    {
        private readonly IRepl repl;

        public ClearCommand(IRepl repl)
        {
            this.repl = repl;
        }

        public string CommandName
        {
            get { return "clear"; }
        }

        public string Help
        {
            get { return "Clears all text from the REPL."; }
        }

        public object Execute(IScriptExecutor replexecutor, object[] args)
        {
            repl.Clear();
            return null;
        }
    }
}