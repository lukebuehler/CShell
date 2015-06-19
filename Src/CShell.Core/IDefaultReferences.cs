using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CShell
{
    public interface IDefaultReferences
    {
        List<Assembly> Assemblies { get; }
        List<string> AssemblyPaths { get; }
        List<string> Namespaces { get; }
    }
}
