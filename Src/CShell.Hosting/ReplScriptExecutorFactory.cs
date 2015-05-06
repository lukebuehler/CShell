using CShell.Framework.Services;
using ScriptCs;

namespace CShell.Hosting
{
    public class ReplScriptExecutorFactory : IReplScriptExecutorFactory
    {
        private readonly IReplOutput repl;
        private readonly ScriptServices scriptServices;

        public ReplScriptExecutorFactory(ScriptServices scriptServices, IReplOutput repl)
        {
            this.scriptServices = scriptServices;
            this.repl = repl;
        }

        public IReplScriptExecutor Create(string workspaceDirectory)
        {
            scriptServices.FileSystem.CurrentDirectory = workspaceDirectory;
            scriptServices.InstallationProvider.Initialize();

            var replExecutor = new ReplScriptExecutor(
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
