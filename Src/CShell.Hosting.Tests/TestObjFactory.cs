using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using CShell.Framework.Services;
using NSubstitute;
using ScriptCs.Contracts;
using ScriptCs.ReplCommands;

namespace CShell.Hosting.Tests
{
    public class TestObjFactory
    {
        public TestObjFactory()
        {
            this.Log = Substitute.For<ILog>();
            this.LogProvider = Substitute.For<ILogProvider>();

            this.Repl = Substitute.For<IRepl>();
            this.ReplOutput = Substitute.For<IReplOutput>();
            this.ObjectSerializer = Substitute.For<IObjectSerializer>();
            this.FileSystem = Substitute.For<IFileSystem>();
            this.FilePreProcessor = Substitute.For<IFilePreProcessor>();
            this.ScriptEngine = Substitute.For<IScriptEngine>();
            this.DefaultReferences = Substitute.For<IDefaultReferences>();
            this.DefaultReferences.Assemblies.Returns(new List<Assembly>());
            this.DefaultReferences.AssemblyPaths.Returns(new List<string>());
            this.DefaultReferences.Namespaces.Returns(new List<string>());

            this.PackageInstaller = Substitute.For<IPackageInstaller>();
            this.PackageAssemblyResolver = Substitute.For<IPackageAssemblyResolver>();
            this.InstallationProvider = Substitute.For<IInstallationProvider>();

            this.Commands = new List<IReplCommand>()
            {
                new ReplCommands.HelpCommand(this.ReplOutput),
                new ReplCommands.ClearCommand(this.ReplOutput),
                new ReplCommands.ResetCommand(),
                new ReplCommands.InstallCommand(PackageInstaller, PackageAssemblyResolver, Log, InstallationProvider),
            };
        }
        public ReplScriptExecutor GetReplExecutor()
        {
            return new ReplScriptExecutor(this.ReplOutput,
                this.ObjectSerializer,
                this.FileSystem,
                this.FilePreProcessor,
                this.ScriptEngine,
                this.LogProvider,
                this.Commands,
                this.DefaultReferences
                );
        }

        public ILog Log { get; private set; }
        public ILogProvider LogProvider { get; private set; }

        public IReplOutput ReplOutput { get;private set; }
        public IRepl Repl { get;private set; }
        public IScriptEngine ScriptEngine { get; private set; }
        public List<IReplCommand> Commands { get; private set; }


        public IDefaultReferences DefaultReferences { get; private set; }
        public IObjectSerializer ObjectSerializer { get; private set; }

        public IFilePreProcessor FilePreProcessor { get; private set; }
        public IFileSystem FileSystem { get; private set; }

        public IPackageAssemblyResolver PackageAssemblyResolver { get; private set; }
        public IPackageInstaller PackageInstaller { get; private set; }
        public IInstallationProvider InstallationProvider { get; private set; }

    }
}
