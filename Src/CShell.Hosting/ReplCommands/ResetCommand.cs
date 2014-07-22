using ScriptCs.Contracts;

namespace CShell.Hosting.ReplCommands
{
    public class ResetCommand : IReplCommandWithInfo
    {
        public string CommandName
        {
            get { return "reset"; }
        }

        public string Help
        {
            get { return "Resets the REPL environment and clears all text."; }
        }

        public object Execute(IScriptExecutor repl, object[] args)
        {
            repl.Reset();
            return null;
        }
    }
}