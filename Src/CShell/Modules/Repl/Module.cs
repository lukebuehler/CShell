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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using CShell.Framework;
using CShell.Framework.Menus;
using CShell.Framework.Results;
using CShell.Framework.Services;
using CShell.Modules.Repl.ViewModels;
using Caliburn.Micro;

namespace CShell.Modules.Repl
{
	[Export(typeof(IModule))]
	public class Module : ModuleBase
	{
        public Module()
        {
            Order = 2;
        }

        public override void Configure(IModuleConfiguration configuration)
        { }

        public override void Start()
        {
		    MainMenu.All.First(x => x.Name == "View")
		        .Add(
                    new MenuItem("Output", OpenOutput).WithIcon("Resources/Icons/Output.png"),
                    new MenuItem("C# Interactive", OpenRepl).WithIcon("Resources/Icons/Output.png"));

            var replViewModel = IoC.Get<ReplViewModel>();
            Shell.ShowTool(replViewModel);
		}

        private IEnumerable<IResult> OpenOutput()
        {
            yield return Show.Tool<OutputViewModel>();
        }

        private IEnumerable<IResult> OpenRepl()
        {
            yield return Show.Tool<ReplViewModel>();
        }
	}
}