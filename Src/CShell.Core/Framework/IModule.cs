using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using ScriptCs.Contracts;

namespace CShell.Framework
{
	public interface IModule : System.IDisposable
	{
	    int Order { get; }

        void Configure();
        void Start();
	}
}