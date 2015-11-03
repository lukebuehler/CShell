namespace CShell.Hosting
{
    using CShell.Framework.Services;

    using ScriptCs;

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
            this.scriptServices.FileSystem.CurrentDirectory = workspaceDirectory;
            this.scriptServices.InstallationProvider.Initialize();

            var replExecutor = new ReplScriptExecutor(
                this.replOutput,
                this.scriptServices.ObjectSerializer,
                this.scriptServices.FileSystem,
                this.scriptServices.FilePreProcessor,
                this.scriptServices.Engine,
                this.scriptServices.Logger,
                this.scriptServices.ReplCommands,
                this.defaultReferences);

            var assemblies = this.scriptServices.AssemblyResolver.GetAssemblyPaths(this.scriptServices.FileSystem.CurrentDirectory);
            var scriptPacks = this.scriptServices.ScriptPackResolver.GetPacks();

            replExecutor.Initialize(assemblies, scriptPacks);
            this.replOutput.Initialize(replExecutor);

            return replExecutor;
        }
    }
}
