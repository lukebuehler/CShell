using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShell.Framework.Services;

namespace CShell
{
    public interface IReplExecutorFactory
    {
        IReplExecutor Create(string workspaceDirectory);
    }
}
