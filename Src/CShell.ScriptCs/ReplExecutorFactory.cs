using ScriptCs;
using ScriptCs.Contracts;

namespace CShell.ScriptCs
{
    public class ReplExecutorFactory : IReplExecutorFactory
    {
        private readonly ScriptServices scriptServices;

        public ReplExecutorFactory(ScriptServicesBuilder builder)
        {
            scriptServices = builder.Build();
        }

        public IReplExecutor Create(CShell.Framework.Services.IRepl repl, string workspaceDirectory)
        {
            scriptServices.FileSystem.CurrentDirectory = workspaceDirectory;

            var replExecutor = new ReplExecutor(
                repl, 
                scriptServices.ObjectSerializer, 
                scriptServices.FileSystem, 
                scriptServices.FilePreProcessor, 
                scriptServices.Engine, 
                scriptServices.Logger);

            var assemblies = scriptServices.AssemblyResolver.GetAssemblyPaths(scriptServices.FileSystem.CurrentDirectory);
            var scriptPacks = scriptServices.ScriptPackResolver.GetPacks();

            replExecutor.Initialize(assemblies, scriptPacks);
            repl.Initialize(replExecutor);

            return replExecutor;
        }
    }
}
