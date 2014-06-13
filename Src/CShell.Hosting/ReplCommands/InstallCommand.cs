using System.Linq;
using System.Runtime.Versioning;
using Common.Logging;
using ScriptCs;
using ScriptCs.Contracts;

namespace CShell.Hosting.ReplCommands
{
    public class InstallCommand : IReplCommandWithInfo
    {
        private readonly IPackageInstaller _packageInstaller;
        private readonly IPackageAssemblyResolver _packageAssemblyResolver;
        private readonly ILog _logger;
        private readonly IInstallationProvider _installationProvider;

        public InstallCommand(
            IPackageInstaller packageInstaller,
            IPackageAssemblyResolver packageAssemblyResolver,
            ILog logger,
            IInstallationProvider installationProvider)
        {
            _packageInstaller = packageInstaller;
            _packageAssemblyResolver = packageAssemblyResolver;
            _logger = logger;
            _installationProvider = installationProvider;
        }

        public string CommandName
        {
            get { return "install"; }
        }

        public string Help
        {
            get { return "Installs a NuGet pacakge. Use: ':install PackageName [version] [pre]'"; }
        }

        public object Execute(IScriptExecutor repl, object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return null;
            }

            string version = null;
            var allowPre = false;
            if (args.Length >= 2)
            {
                version = args[1].ToString();
            }

            allowPre = args.Length >= 3 && args[2].ToString().ToUpperInvariant() == "PRE";

            _logger.InfoFormat("Installing {0}", args[0]);

            _installationProvider.Initialize();

            var packageRef = new PackageReference(
                args[0].ToString(), new FrameworkName(".NETFramework,Version=v4.0"), version);

            _packageInstaller.InstallPackages(new[] { packageRef }, allowPre);
            _packageAssemblyResolver.SavePackages();

            var dlls = _packageAssemblyResolver.GetAssemblyNames(repl.FileSystem.CurrentDirectory)
                .Except(repl.References.PathReferences).ToArray();

            if(repl is IReplExecutor)
                ((IReplExecutor)repl).AddReferencesAndNotify(dlls);
            else
                repl.AddReferences(dlls);

            foreach (var dll in dlls)
            {
                _logger.InfoFormat("Added reference to {0}", dll);
            }

            return null;
        }
    }
}
