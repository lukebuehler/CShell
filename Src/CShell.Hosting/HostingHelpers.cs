using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
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
            var hostingBuilder = new RegistrationBuilder();
            ConfigureHostingRegistrationBuilder(hostingBuilder);
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(IScriptEngine).Assembly, hostingBuilder)); //ScriptCS.Contracts
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ScriptServices).Assembly, hostingBuilder)); //ScriptCS.Core
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(RoslynScriptEngine).Assembly, hostingBuilder)); //CShell.Engine.Roslyn
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(HostingHelpers).Assembly, hostingBuilder)); //CShell.Hosting
        }

        public static void ConfigureHostingRegistrationBuilder(RegistrationBuilder builder)
        {
            builder.ForTypesDerivedFrom<IReplCommandWithInfo>().Export<IReplCommand>();
            builder.ForTypesDerivedFrom<ILineProcessor>().Export<ILineProcessor>();

            builder.ForType<ReplLogger>().SelectConstructor(b => b.First(c => c.GetParameters().Length == 1)).Export<ILog>();
            builder.ForType<FileSystem>().Export<IFileSystem>();
            builder.ForType<ReplScriptHostFactory>().Export<IScriptHostFactory>();
            builder.ForType<RoslynScriptEngine>().Export<IScriptEngine>();
            builder.ForType<FilePreProcessor>().Export<IFilePreProcessor>();

            builder.ForType<PackageContainer>().Export<IPackageContainer>();
            builder.ForType<PackageAssemblyResolver>().Export<IPackageAssemblyResolver>();
            builder.ForType<NugetInstallationProvider>().Export<IInstallationProvider>();
            builder.ForType<PackageInstaller>().Export<IPackageInstaller>();

            builder.ForType<ScriptPackResolver>().Export<IScriptPackResolver>();
            builder.ForType<AssemblyUtility>().Export<IAssemblyUtility>();
            builder.ForType<AssemblyResolver>().Export<IAssemblyResolver>();
            builder.ForType<ObjectSerializer>().Export<IObjectSerializer>();
            builder.ForType<MockConsole>().Export<IConsole>();

            builder.ForType<ScriptExecutor>().Export<IScriptExecutor>();
            builder.ForType<ScriptServices>().Export<ScriptServices>();

            builder.ForType<ReplExecutorFactory>().Export<IReplExecutorFactory>();
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
