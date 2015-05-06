using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShell.Hosting.Package;
using CShell.Hosting.ReplCommands;
using ScriptCs;
using ScriptCs.Contracts;
using ScriptCs.Engine.Roslyn;
using ScriptCs.Logging;

namespace CShell.Hosting
{
    public static class HostingHelpers
    {
        public static void ConfigureHostingCatalog(AggregateCatalog catalog)
        {
            //add types from dlls
            var hostingBuilder = new RegistrationBuilder();
            ConfigureHostingRegistrationBuilder(hostingBuilder);
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IScriptEngine).Assembly, hostingBuilder)); //ScriptCS.Contracts
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ScriptServices).Assembly, hostingBuilder)); //ScriptCS.Core
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(RoslynScriptEngine).Assembly, hostingBuilder)); //CShell.Engine.Roslyn
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(HostingHelpers).Assembly, hostingBuilder)); //CShell.Hosting

            //add singletons
            var container = new CompositionContainer(catalog);
            var batch = new CompositionBatch();
            ConfigureHostingbatch(batch);
            container.Compose(batch);
        }

        private static void ConfigureHostingRegistrationBuilder(RegistrationBuilder builder)
        {
            builder.ForTypesDerivedFrom<ILineProcessor>().Export<ILineProcessor>();
            builder.ForTypesDerivedFrom<IReplCommand>().Export<IReplCommand>();

            builder.ForType<ReplLogger>().SelectConstructor(b => b.First(c => c.GetParameters().Length == 1)).Export<ILog>();
            builder.ForType<FileSystem>().Export<IFileSystem>();
            builder.ForType<FileSystemMigrator>().Export<IFileSystemMigrator>();
            builder.ForType<ReplScriptHostFactory>().Export<IScriptHostFactory>();
            builder.ForType<RoslynReplEngine>().Export<IScriptEngine>();
            builder.ForType<FilePreProcessor>().Export<IFilePreProcessor>();
            builder.ForType<ReplScriptExecutor>().Export<IRepl>();

            builder.ForType<PackageContainer>().Export<IPackageContainer>();
            builder.ForType<PackageAssemblyResolver>().Export<IPackageAssemblyResolver>();
            builder.ForType<NugetInstallationProvider>().Export<IInstallationProvider>();
            builder.ForType<PackageInstaller>().Export<IPackageInstaller>();
            builder.ForType<NullScriptLibraryComposer>().Export<IScriptLibraryComposer>();

            builder.ForType<ScriptPackResolver>().Export<IScriptPackResolver>();
            builder.ForType<AssemblyUtility>().Export<IAssemblyUtility>();
            builder.ForType<AssemblyResolver>().Export<IAssemblyResolver>();
            builder.ForType<ObjectSerializer>().Export<IObjectSerializer>();
            builder.ForType<MockConsole>().Export<IConsole>();

            builder.ForType<ScriptExecutor>().Export<IScriptExecutor>();
            builder.ForType<ScriptServices>().Export<ScriptServices>();

            builder.ForType<ReplScriptExecutorFactory>().Export<IReplScriptExecutorFactory>();
        }

        private static void ConfigureHostingbatch(CompositionBatch batch)
        {
            //batch.AddExportedValue<IRepl>(null);
        }

        public static void ConfigureModuleRegistrationBuilder(RegistrationBuilder builder)
        {
            builder.ForTypesDerivedFrom<IReplCommand>()
                .Export<IReplCommand>();

            builder.ForTypesDerivedFrom<ILineProcessor>()
                .Export<ILineProcessor>();
        }
    }
}
