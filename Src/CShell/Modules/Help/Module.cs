using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CShell.Framework;
using CShell.Framework.Menus;
using CShell.Framework.Results;
using CShell.Modules.Help.ViewModels;
using Caliburn.Micro;

namespace CShell.Modules.Help
{
    [Export(typeof (IModule))]
    public class Module : ModuleBase
    {
        public Module()
        {
            Order = 6;
        }

        public override void Start()
        {
            //populate the menu
            MainMenu.First(item => item.Name == "Help")
                .Add(
                    new MenuItem("View Help", ViewHelp).WithGlobalShortcut(ModifierKeys.Control, Key.F1),
                    MenuItemBase.Separator,
                    new MenuItem("About CShell", About)
                );
        }

        public IEnumerable<IResult> ViewHelp()
        {
            System.Diagnostics.Process.Start(Constants.CShellHelp);
            yield break;
        }

        public IEnumerable<IResult> About()
        {
            yield return Show.Dialog<AboutViewModel>();
        }

       
    }
}
