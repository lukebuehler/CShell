#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2012  Arnova Asset Management Ltd., Lukas Buhler
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion
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

        public override void Configure(IModuleConfiguration configuration)
        {}

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
