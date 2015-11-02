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