using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShell.Framework;

namespace CShell.Sinks.Xhtml
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
                DefaultReferences.Assemblies.Add(typeof(XhtmlSinkViewModel).Assembly);
                DefaultReferences.Namespaces.Add(typeof(XhtmlSinkViewModel).Namespace);
            }
        }

        public override void Start()
        {}
    }
}
