using System;
using Common.Logging;
using Common.Logging.Simple;
using CShell.ScriptCs.Package;
using ScriptCs;
using ScriptCs.Contracts;
using ScriptCs.Engine.Roslyn;
using IFileSystem = ScriptCs.Contracts.IFileSystem;
using LogLevel = Common.Logging.LogLevel;

namespace CShell.ScriptCs
{
    public class ScriptServicesBuilder
    {
        private IFileSystem fileSystem = new FileSystem();
        private ILog logger;

        public ScriptServices Build()
        {
            //TODO: this need to be wired up properly using our own IoC container from caliburn.

            logger = new ConsoleOutLogger("Console", LogLevel.Debug, true, false, false, "HH:mm:ss");
            var engine = new RoslynScriptEngine(new ReplScriptHostFactory(), logger);
            var filePreProcessor = new FilePreProcessor(fileSystem, logger, new ILineProcessor[] {});
            var packageAssemblyResolver = new PackageAssemblyResolver(fileSystem, new PackageContainer(fileSystem, logger), logger);
            var installationProvider = new NugetInstallationProvider(fileSystem, logger);

            return new ScriptServices(
                new FileSystem(),
                packageAssemblyResolver,
                new ScriptExecutor(fileSystem, filePreProcessor, engine, logger),
                engine,
                filePreProcessor,
                new ScriptPackResolver(new IScriptPack[0]),
                new PackageInstaller(installationProvider, logger),
                null,
                logger,
                new AssemblyResolver(fileSystem, packageAssemblyResolver, new AssemblyUtility(), logger),
                null,
                installationProvider
                );
        }

    }
}
