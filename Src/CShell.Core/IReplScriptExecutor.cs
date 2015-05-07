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
    public interface IReplScriptExecutor : IRepl, IScriptExecutor
    {
        event EventHandler<EventArgs> AssemblyReferencesChanged;

        string WorkspaceDirectory { get; }
        ICompletion ReplCompletion { get; }
        ICompletion DocumentCompletion { get; }

        string[] GetReferencesAsPaths();
        string[] GetVariables();

        void ExecuteConfigScript();
        void ExecuteReferencesScript();
    }
}
