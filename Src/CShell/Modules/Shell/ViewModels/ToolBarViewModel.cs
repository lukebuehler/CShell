using System.ComponentModel.Composition;
using CShell.Framework.Menus;
using CShell.Framework.Services;

namespace CShell.Modules.Shell.ViewModels
{
    [Export(typeof(IToolBar))]
    public class ToolBarViewModel : ToolBarModel
    {
        [Import]
        private IShell _shell;
    }
}
