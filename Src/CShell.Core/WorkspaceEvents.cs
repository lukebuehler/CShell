using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CShell
{
    public class WorkspaceOpeningEventArgs : EventArgs
    {
        public WorkspaceOpeningEventArgs(string workspaceDirectory)
        {
            WorkspaceDirectory = workspaceDirectory;
        }
        public string WorkspaceDirectory { get; private set; }
    }

    public class WorkspaceOpenedEventArgs : EventArgs
    {
        public WorkspaceOpenedEventArgs(string workspaceDirectory)
        {
            WorkspaceDirectory = workspaceDirectory;
        }
        public string WorkspaceDirectory { get; private set; }
    }
}
