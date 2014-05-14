using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using CShell.ScriptCs.Package;
using NuGet;
using ScriptCs;
using ScriptCs.Contracts;
using ScriptCs.Engine.Roslyn;
using ScriptCs.Exceptions;
using IFileSystem = ScriptCs.Contracts.IFileSystem;

namespace CShell.ScriptCs
{
    public class ScriptServicesBuilder
    {
        private IFileSystem fileSystem = new FileSystem();
        private ILog logger;

        public ScriptServices Build()
        {
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
