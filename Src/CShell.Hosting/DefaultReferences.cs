using System;
using System.Collections.Generic;
using System.Reflection;

namespace CShell.Hosting
{
    public class DefaultReferences : IDefaultReferences
    {
        public DefaultReferences()
        {
            Assemblies = new List<Assembly>();
            AssemblyPaths = new List<string>();
            Namespaces = new List<string>();
        }
        public List<Assembly> Assemblies { get; private set; }
        public List<string> AssemblyPaths { get; private set; }
        public List<string> Namespaces { get; private set; }
    }
}
