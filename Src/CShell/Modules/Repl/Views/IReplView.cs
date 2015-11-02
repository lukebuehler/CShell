using CShell.Framework.Services;
using CShell.Modules.Repl.Controls;

namespace CShell.Modules.Repl.Views
{
    public interface IReplView
    {
        IReplOutput GetReplOutput();
    }
}
