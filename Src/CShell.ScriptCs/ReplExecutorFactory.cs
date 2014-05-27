using ScriptCs;
using ScriptCs.Contracts;

namespace CShell.ScriptCs
{
    public class ReplExecutorFactory : IReplExecutorFactory
    {
        private readonly ScriptServicesBuilder builder;
        private ScriptServices scriptServices;

        public ReplExecutorFactory(ScriptServicesBuilder builder)
        {
            this.builder = builder;
        }

        public IReplExecutor Create(CShell.Framework.Services.IRepl repl, string workspaceDirectory)
        {
            if (scriptServices == null)
            {
                scriptServices = builder.Build(repl);
            }

            scriptServices.FileSystem.CurrentDirectory = workspaceDirectory;
            scriptServices.InstallationProvider.Initialize();

            var replExecutor = new ReplExecutor(
                repl, 
                scriptServices.ObjectSerializer, 
                scriptServices.FileSystem, 
                scriptServices.FilePreProcessor,
                scriptServices.Engine,
                scriptServices.PackageInstaller,
                scriptServices.PackageAssemblyResolver,
                scriptServices.Logger);

            var assemblies = scriptServices.AssemblyResolver.GetAssemblyPaths(scriptServices.FileSystem.CurrentDirectory);
            var scriptPacks = scriptServices.ScriptPackResolver.GetPacks();

            replExecutor.Initialize(assemblies, scriptPacks);
            repl.Initialize(replExecutor);

            return replExecutor;
        }
    }
}
