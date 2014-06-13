using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptCs.Contracts;

namespace CShell.Hosting.ReplCommands
{
    public interface IReplCommandWithInfo : IReplCommand
    {
        string Help { get; }
    }
}
