using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShell.Framework.Services;

namespace CShell
{
    public interface IReplScriptExecutorFactory
    {
        IReplScriptExecutor Create(string workspaceDirectory);
    }
}
