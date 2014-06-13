using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ScriptCs.Contracts;

namespace CShell
{
    public interface IReplExecutor : IScriptExecutor
    {
        event EventHandler<EventArgs> AssemblyReferencesChanged;

        string WorkspaceDirectory { get; }
        ICompletion ReplCompletion { get; }
        ICompletion DocumentCompletion { get; }
        IEnumerable<IReplCommand> ReplCommands { get; } 

        string[] GetNamespaces();
        string[] GetReferencesAsPaths();
        string[] GetVariables();
        void AddReferencesAndNotify(params Assembly[] references);
        void RemoveReferencesAndNotify(params Assembly[] references);
        void AddReferencesAndNotify(params string[] references);
        void RemoveReferencesAndNotify(params string[] references);
        
    }
}
