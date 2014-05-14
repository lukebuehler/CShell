using ScriptCs;

namespace CShell.ScriptCs
{
    public class ReplExecutorFactory : IReplExecutorFactory
    {
        private readonly ScriptServices scriptServices;

        public ReplExecutorFactory(ScriptServicesBuilder builder)
        {
            scriptServices = builder.Build();
        }

        public IReplExecutor Create(CShell.Framework.Services.IRepl repl)
        {
            return new ReplExecutor(
                repl, 
                scriptServices.ObjectSerializer, 
                scriptServices.FileSystem, 
                scriptServices.FilePreProcessor, 
                scriptServices.Engine, 
                scriptServices.Logger);

            //todo: register the replExecutor with the repl
        }
    }
}
