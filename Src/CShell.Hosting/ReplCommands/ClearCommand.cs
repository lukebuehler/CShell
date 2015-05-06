using CShell.Framework.Services;
using CShell.Hosting.ReplCommands;
using ScriptCs.Contracts;

namespace CShell.Hosting.ReplCommands
{
    public class ClearCommand : IReplCommand
    {
        private readonly IReplOutput replOutput;

        public ClearCommand(IReplOutput replOutput)
        {
            this.replOutput = replOutput;
        }

        public string CommandName
        {
            get { return "clear"; }
        }

        public string Description
        {
            get { return "Clears all text from the REPL."; }
        }

        public object Execute(IRepl repl, object[] args)
        {
            replOutput.Clear();
            return null;
        }
    }
}