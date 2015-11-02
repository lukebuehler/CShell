using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.AvalonDock;

namespace CShell.Modules.Shell.Views
{
    public interface IShellView
    {
        DockingManager DockingManager { get;}

        CShell.Workspace.WindowLocation GetWindowLocation();
        void RestoreWindowLocation(CShell.Workspace.WindowLocation windowLocation);
    }
}
