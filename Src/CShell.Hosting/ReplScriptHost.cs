using ScriptCs;
using ScriptCs.Contracts;

namespace CShell.Hosting
{
    public class ReplScriptHost : ScriptHost
    {
        public ReplScriptHost(IScriptPackManager scriptPackManager, ScriptEnvironment environment)
            : base(scriptPackManager, environment)
        {

        }

        public static string Ha { get { return "Ha haha"; } }

    }

    public class ReplScriptHostFactory : IScriptHostFactory
    {
        public IScriptHost CreateScriptHost(IScriptPackManager scriptPackManager, string[] scriptArgs)
        {
            return new ReplScriptHost(scriptPackManager, new ScriptEnvironment(scriptArgs));
        }
    }
}
