using Common.Logging;
using CShell.Framework.Services;
using CShell.Hosting;
using NSubstitute;
using ScriptCs.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CShell.Tests
{
    public class TestObjFactory
    {
        public TestObjFactory()
        {
            this.Repl = Substitute.For<IRepl>();
            this.ObjectSerializer = Substitute.For<IObjectSerializer>();
            this.FileSystem = Substitute.For<IFileSystem>();
            this.FilePreProcessor = Substitute.For<IFilePreProcessor>();
            this.ScriptEngine = Substitute.For<IScriptEngine>();
            this.PackageInstaller = Substitute.For<IPackageInstaller>();
            this.PackageAssemblyResolver = Substitute.For<IPackageAssemblyResolver>();
            this.Log = Substitute.For<ILog>();
        }
        public ReplExecutor GetReplExecutor()
        {
            return new ReplExecutor(this.Repl,
                this.ObjectSerializer,
                this.FileSystem,
                this.FilePreProcessor,
                this.ScriptEngine,
                this.PackageInstaller,
                this.PackageAssemblyResolver,
                this.Log);
        }

        public IRepl Repl { get;private set; }

        public ILog Log { get; private set; }

        public IPackageAssemblyResolver PackageAssemblyResolver { get; private set; }

        public IPackageInstaller PackageInstaller { get; private set; }

        public IScriptEngine ScriptEngine { get; private set; }

        public IFilePreProcessor FilePreProcessor { get; private set; }

        public IFileSystem FileSystem { get; private set; }

        public IObjectSerializer ObjectSerializer { get; private set; }
    }
}
