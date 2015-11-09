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

namespace CShell.Hosting
{
    public static class HostingHelpers
    {
        public static void ConfigureHostingCatalog(AggregateCatalog catalog)
        {
            //add types from dlls
            var hostingBuilder = new RegistrationBuilder();
            ConfigureHostingRegistrationBuilder(hostingBuilder);
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Common.Logging.LogManager).Assembly, hostingBuilder)); //Common.Logging
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
            builder.ForTypesDerivedFrom<IReplCommand>().SelectNonObsoleteConstructor().Export<IReplCommand>();

            builder.ForType<ReplLogProvider>().SelectConstructor(b => b.First(c => c.GetParameters().Length == 1)).Export<ILogProvider>();
            builder.ForType<CShell.Hosting.FileSystem>().Export<IFileSystem>(); //override bin and nuget locations
            builder.ForType<FileSystemMigrator>().SelectNonObsoleteConstructor().Export<IFileSystemMigrator>();
            builder.ForType<FilePreProcessor>().SelectNonObsoleteConstructor().Export<IFilePreProcessor>();

            builder.ForType<ReplScriptHostFactory>().Export<IScriptHostFactory>();
            builder.ForType<RoslynReplEngine>().SelectNonObsoleteConstructor().Export<IScriptEngine>();
            builder.ForType<ReplScriptExecutor>().Export<IRepl>();

            builder.ForType<PackageContainer>().Export<IPackageContainer>();
            builder.ForType<PackageAssemblyResolver>().SelectNonObsoleteConstructor().Export<IPackageAssemblyResolver>();
            builder.ForType<NugetInstallationProvider>().Export<IInstallationProvider>();
            builder.ForType<PackageInstaller>().Export<IPackageInstaller>();

            builder.ForType<NullScriptLibraryComposer>().Export<IScriptLibraryComposer>();
            builder.ForType<ScriptPackResolver>().Export<IScriptPackResolver>();

            builder.ForType<AssemblyUtility>().Export<IAssemblyUtility>();
            builder.ForType<AssemblyResolver>().SelectNonObsoleteConstructor().Export<IAssemblyResolver>();
            builder.ForType<ObjectSerializer>().Export<IObjectSerializer>();
            builder.ForType<MockConsole>().Export<IConsole>();
            builder.ForType<DefaultReferences>().Export<IDefaultReferences>();

            builder.ForType<ScriptExecutor>().SelectNonObsoleteConstructor().Export<IScriptExecutor>();
            builder.ForType<ScriptServices>().SelectNonObsoleteConstructor().Export<ScriptServices>();

            builder.ForType<ReplScriptExecutorFactory>().Export<IReplScriptExecutorFactory>();
        }

        private static PartBuilder SelectNonObsoleteConstructor(this PartBuilder builder)
        {
            return builder.SelectConstructor(
                b => b.FirstOrDefault(cis => cis.CustomAttributes.All(ci => ci.AttributeType != typeof(ObsoleteAttribute))) ?? b.First()
                );
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


        public static void TestIfAllExportsCanBeResolved(CompositionContainer container)
        {
            ILogProvider logProvider = container.GetExportedValue<ILogProvider>();
            IFileSystem fileSystem = container.GetExportedValue<IFileSystem>();
            IPackageContainer packageContainer = container.GetExportedValue<IPackageContainer>();
            IAssemblyUtility asUtil = container.GetExportedValue<IAssemblyUtility>();

            IFilePreProcessor filePreProcessor = container.GetExportedValue<IFilePreProcessor>();
            IObjectSerializer objectSerializer = container.GetExportedValue<IObjectSerializer>();


            IPackageAssemblyResolver packageAssemblyResolver = container.GetExportedValue<IPackageAssemblyResolver>();
            IAssemblyResolver assemblyResolver = container.GetExportedValue<IAssemblyResolver>();
            IScriptPackResolver scriptPackResolver = container.GetExportedValue<IScriptPackResolver>();
            IPackageInstaller packageInstaller = container.GetExportedValue<IPackageInstaller>();
            IFileSystemMigrator fileSystemMigrator = container.GetExportedValue<IFileSystemMigrator>();
            IConsole console = container.GetExportedValue<IConsole>();
            IInstallationProvider installationProvider = container.GetExportedValue<IInstallationProvider>();
            IScriptLibraryComposer scriptLibraryComposer = container.GetExportedValue<IScriptLibraryComposer>();

            IEnumerable<IReplCommand> replCommands = container.GetExportedValues<IReplCommand>();

            IScriptEngine engine = container.GetExportedValue<IScriptEngine>();
            IRepl repl = container.GetExportedValue<IRepl>();
            IScriptExecutor executor = container.GetExportedValue<IScriptExecutor>();
            ScriptServices scriptServices = container.GetExportedValue<ScriptServices>();
        }
    }
}
