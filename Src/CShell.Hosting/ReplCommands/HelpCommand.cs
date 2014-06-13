using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptCs.Contracts;

namespace CShell.Hosting.ReplCommands
{
    public class HelpCommand : IReplCommandWithInfo
    {
        public string CommandName
        {
            get { return "help"; }
        }

        public string Help
        {
            get { return "Displays this help screen."; }
        }

        public object Execute(IScriptExecutor repl, object[] args)
        {
            var replExecutor = repl as IReplExecutor;
            if(replExecutor == null)
                throw new Exception("Could not find repl executor to get available commands.");

            var replCommands = replExecutor.ReplCommands;
            var commands = "Available REPL commands:" + Environment.NewLine;
            foreach (var replCommand in replCommands)
            {
                var helpText = replCommand is IReplCommandWithInfo ? ((IReplCommandWithInfo) replCommand).Help : "";
                commands += String.Format(" :{0,-15} - {1}", replCommand.CommandName, helpText);
                commands += Environment.NewLine;
            }
            return commands;
        }

        
    }
}
