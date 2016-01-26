using CShell.Properties;

namespace CShell.Modules.Repl.Controls
{
    internal class CommandHistory
    {
        private int currentPosn;
        private string lastCommand;
        private readonly CommandQueue<string> commandHistory = new CommandQueue<string>(Settings.Default.REPLBuffer);

        internal void Add(string command)
        {
            if(string.IsNullOrWhiteSpace(command))
                return;

            if (command != lastCommand)
            {
                commandHistory.Add(command);
                lastCommand = command;
                currentPosn = commandHistory.Count;
            }
            else
            {
                currentPosn++;
            }
        }

        internal bool DoesPreviousCommandExist()
        {
            return currentPosn > 0;
        }

        internal bool DoesNextCommandExist()
        {
            return currentPosn < commandHistory.Count;
        }

        internal string GetPreviousCommand()
        {
            lastCommand = commandHistory[--currentPosn];
            return lastCommand;
        }

        internal string GetNextCommand()
        {
            if (currentPosn == commandHistory.Count - 1)
            {
                currentPosn++;
                return "";
            }
            else
            { 
                lastCommand = (string)commandHistory[++currentPosn];
                return LastCommand;
            }
        }

        internal string LastCommand
        {
            get { return lastCommand; }
        }

        internal string[] GetCommandHistory()
        {
            return commandHistory.ToArray();
        }
    }
}
