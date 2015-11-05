namespace CShell.Hosting
{
    using ScriptCs;
    using ScriptCs.Contracts;

    public class ReplScriptHost : ScriptHost
    {
        public ReplScriptHost(IScriptPackManager scriptPackManager, ScriptEnvironment environment)
            : base(scriptPackManager, environment)
        {
        }
    }

    public class ReplScriptHostFactory : IScriptHostFactory
    {
        public IScriptHost CreateScriptHost(IScriptPackManager scriptPackManager, string[] scriptArgs)
        {
            return new ReplScriptHost(scriptPackManager, new ScriptEnvironment(scriptArgs));
        }
    }
}
