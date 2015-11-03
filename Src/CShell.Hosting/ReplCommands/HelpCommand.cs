namespace CShell.Hosting.ReplCommands
{
    using System.Linq;

    using CShell.Framework.Services;

    using ScriptCs.Contracts;

    public class HelpCommand : IReplCommand
    {
        private readonly IReplOutput replOutput;

        public HelpCommand(IReplOutput replOutput)
        {
            this.replOutput = replOutput;
        }

        public string CommandName
        {
            get { return "help"; }
        }

        public string Description
        {
            get { return "Displays this help screen."; }
        }

        public object Execute(IRepl repl, object[] args)
        {
            this.replOutput.WriteLine("The following commands are available in the REPL:");
            foreach (var command in repl.Commands.OrderBy(x => x.Key))
            {
                this.replOutput.WriteLine(string.Format(":{0,-15}{1,10}", command.Key, command.Value.Description));
            }

            return null;
        }
    }
}
