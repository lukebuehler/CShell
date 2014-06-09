using Common.Logging;
using CShell.Framework.Services;
using CShell.Hosting.Package;
using ScriptCs;
using ScriptCs.Contracts;
using ScriptCs.Engine.Roslyn;
using ScriptCs.ReplCommands;
using LogLevel = Common.Logging.LogLevel;

namespace CShell.Hosting
{
    public class ScriptServicesBuilder
    {

        public ScriptServicesBuilder()
        {
        }

        public ScriptServices Build(IRepl repl)
        {
            //TODO: this need to be wired up properly using our own IoC container from caliburn.

            var underlyingLogger = LogManager.GetCurrentClassLogger();
            var replLogger = new ReplLogger(LogLevel.Info, repl, underlyingLogger);
            ILog logger = replLogger;

            var fileSystem = new FileSystem();
            var engine = new RoslynScriptEngine(new ReplScriptHostFactory(), logger);
            var filePreProcessor = new FilePreProcessor(fileSystem, logger, new ILineProcessor[] {new LoadLineProcessor(fileSystem), new ReferenceLineProcessor(fileSystem), new UsingLineProcessor() });
            var packageAssemblyResolver = new PackageAssemblyResolver(fileSystem, new PackageContainer(fileSystem, logger), logger);
            var installationProvider = new NugetInstallationProvider(fileSystem, logger);

            return new ScriptServices(
                fileSystem,
                packageAssemblyResolver,
                new ScriptExecutor(fileSystem, filePreProcessor, engine, logger),
                engine,
                filePreProcessor,
                new ScriptPackResolver(new IScriptPack[0]),
                new PackageInstaller(installationProvider, logger),
                null, //IObjectSerializer
                logger,
                new AssemblyResolver(fileSystem, packageAssemblyResolver, new AssemblyUtility(), logger),
                new IReplCommand[]{new ResetCommand(), new ResetCommand(), },
                null, //IConsole
                installationProvider
                );
        }

    }
}
