using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShell.Framework;

namespace CShell.Sinks.Charting
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
                DefaultReferences.Assemblies.Add(typeof(ChartSink).Assembly);
                DefaultReferences.Namespaces.Add(typeof(ChartSink).Namespace);
            }
        }

        public override void Start()
        {}
    }
}
