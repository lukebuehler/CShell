using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CShell
{
    public class WorkspaceException : Exception
    {
        public WorkspaceException()
        {}

        public WorkspaceException(string message)
            :base(message)
        { }

        public WorkspaceException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
