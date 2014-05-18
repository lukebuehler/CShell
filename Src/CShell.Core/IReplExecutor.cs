using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ScriptCs.Contracts;

namespace CShell
{
    public interface IReplExecutor : IScriptExecutor
    {
        AssemblyReferences AssemblyReferences { get; }
        string WorkspaceDirectory { get; }

        event EventHandler<EventArgs> AssemblyReferencesChanged;

        void AddReferencesAndNotify(params Assembly[] references);
        void RemoveReferencesAndNotify(params Assembly[] references);
        void AddReferencesAndNotify(params string[] references);
        void RemoveReferencesAndNotify(params string[] references);
    }
}
