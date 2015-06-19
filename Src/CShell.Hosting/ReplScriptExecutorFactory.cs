using CShell.Framework.Services;
using ScriptCs;

namespace CShell.Hosting
{
    public class ReplScriptExecutorFactory : IReplScriptExecutorFactory
    {
        private readonly IReplOutput replOutput;
        private readonly IDefaultReferences defaultReferences;
        private readonly ScriptServices scriptServices;

        public ReplScriptExecutorFactory(ScriptServices scriptServices, IReplOutput replOutput, IDefaultReferences defaultReferences)
        {
            this.scriptServices = scriptServices;
            this.replOutput = replOutput;
            this.defaultReferences = defaultReferences;
        }

        public IReplScriptExecutor Create(string workspaceDirectory)
        {
            scriptServices.FileSystem.CurrentDirectory = workspaceDirectory;
            scriptServices.InstallationProvider.Initialize();

            var replExecutor = new ReplScriptExecutor(
                replOutput, 
                scriptServices.ObjectSerializer, 
                scriptServices.FileSystem, 
                scriptServices.FilePreProcessor,
                scriptServices.Engine,
                scriptServices.Logger,
                scriptServices.ReplCommands,
                defaultReferences
                );

            var assemblies = scriptServices.AssemblyResolver.GetAssemblyPaths(scriptServices.FileSystem.CurrentDirectory);
            var scriptPacks = scriptServices.ScriptPackResolver.GetPacks();

            replExecutor.Initialize(assemblies, scriptPacks);
            replOutput.Initialize(replExecutor);

            return replExecutor;
        }
    }
}
