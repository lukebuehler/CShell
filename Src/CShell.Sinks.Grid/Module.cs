using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShell.Framework;

namespace CShell.Sinks.Grid
{
    [Export(typeof(IModule))]
    public class Module : ModuleBase
    {
        [Import]
        public IDefaultReferences DefaultReferences { get; set; }

        public override void Configure()
        {
            if (DefaultReferences != null)
            {
                DefaultReferences.Assemblies.Add(typeof(GridSinkViewModel).Assembly);
                DefaultReferences.Namespaces.Add(typeof(GridSinkViewModel).Namespace);
            }
        }

        public override void Start()
        {}
    }
}
