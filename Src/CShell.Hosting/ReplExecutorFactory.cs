using CShell.Framework.Services;
using ScriptCs;

namespace CShell.Hosting
{
    public class ReplExecutorFactory : IReplExecutorFactory
    {
        private readonly IRepl repl;
        private readonly ScriptServices scriptServices;

        public ReplExecutorFactory(ScriptServices scriptServices, IRepl repl)
        {
            this.scriptServices = scriptServices;
            this.repl = repl;
        }

        public IReplExecutor Create(string workspaceDirectory)
        {
            scriptServices.FileSystem.CurrentDirectory = workspaceDirectory;
            scriptServices.InstallationProvider.Initialize();

            var replExecutor = new ReplExecutor(
                repl, 
                scriptServices.ObjectSerializer, 
                scriptServices.FileSystem, 
                scriptServices.FilePreProcessor,
                scriptServices.Engine,
                scriptServices.Logger,
                scriptServices.ReplCommands);

            var assemblies = scriptServices.AssemblyResolver.GetAssemblyPaths(scriptServices.FileSystem.CurrentDirectory);
            var scriptPacks = scriptServices.ScriptPackResolver.GetPacks();

            replExecutor.Initialize(assemblies, scriptPacks);
            repl.Initialize(replExecutor);

            return replExecutor;
        }
    }
}
