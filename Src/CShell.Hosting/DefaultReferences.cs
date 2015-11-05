namespace CShell.Hosting
{
    using System.Collections.Generic;
    using System.Reflection;

    public class DefaultReferences : IDefaultReferences
    {
        public DefaultReferences()
        {
            this.Assemblies = new List<Assembly>();
            this.AssemblyPaths = new List<string>();
            this.Namespaces = new List<string>();
        }

        public List<Assembly> Assemblies { get; private set; }
        
        public List<string> AssemblyPaths { get; private set; }
        
        public List<string> Namespaces { get; private set; }
    }
}
