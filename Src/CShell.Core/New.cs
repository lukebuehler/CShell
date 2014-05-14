using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using CShell.Framework.Services;
using ScriptCs;

namespace CShell.ScriptCs
{
    public class WorkspaceNew
    {
        private readonly IRepl repl;
        private readonly IReplExecutorFactory replExecutorFactory;

        private IReplExecutor replExecutor;

        public WorkspaceNew(IRepl repl, IReplExecutorFactory replExecutorFactory)
        {
            this.repl = repl;
            this.replExecutorFactory = replExecutorFactory;
        }

        public string RootFolder { get; private set; }

        public void SetRootFolder(string dir)
        {
            Close();
            Open(dir);
        }

        private void Open(string dir)
        {
            RootFolder = dir;
            replExecutor = replExecutorFactory.Create(repl);

        }

        private void Close()
        {
            replExecutor = null;
        }
    }

    public class Refs
    {
        
    }

    public class Ref
    {
        
    }
}
