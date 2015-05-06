using ScriptCs.Contracts;

namespace CShell.Hosting.ReplCommands
{
    public class ResetCommand : IReplCommand
    {
        public string CommandName
        {
            get { return "reset"; }
        }

        public string Description
        {
            get { return "Resets the REPL environment and clears all text."; }
        }

        public object Execute(IRepl repl, object[] args)
        {
            repl.Reset();
            return null;
        }
    }
}