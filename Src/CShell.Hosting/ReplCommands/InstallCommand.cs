namespace CShell.Hosting.ReplCommands
{
    using System.Linq;
    using System.Runtime.Versioning;

    using ScriptCs;
    using ScriptCs.Contracts;
    using ScriptCs.Logging;

    public class InstallCommand : IReplCommand
    {
        private readonly IPackageInstaller packageInstaller;
        private readonly IPackageAssemblyResolver packageAssemblyResolver;
        private readonly ILog logger;
        private readonly IInstallationProvider installationProvider;

        public InstallCommand(
            IPackageInstaller packageInstaller,
            IPackageAssemblyResolver packageAssemblyResolver,
            ILog logger,
            IInstallationProvider installationProvider)
        {
            this.packageInstaller = packageInstaller;
            this.packageAssemblyResolver = packageAssemblyResolver;
            this.logger = logger;
            this.installationProvider = installationProvider;
        }

        public string CommandName
        {
            get { return "install"; }
        }

        public string Description
        {
            get { return "Installs a Nuget package. I.e. :install <package> <version> <\"pre\">"; }
        }

        public object Execute(IRepl repl, object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return null;
            }

            string version = null;
            if (args.Length >= 2)
            {
                version = args[1].ToString();
            }

            var allowPre = args.Length >= 3 && args[2].ToString().ToUpperInvariant() == "PRE";

            this.logger.InfoFormat("Installing {0}", args[0]);

            this.installationProvider.Initialize();

            var packageRef = new PackageReference(
                args[0].ToString(), new FrameworkName(CShell.Constants.NetFrameworkName), version);

            this.packageInstaller.InstallPackages(new[] { packageRef }, allowPre);
            this.packageAssemblyResolver.SavePackages();

            var dlls = this.packageAssemblyResolver.GetAssemblyNames(repl.FileSystem.CurrentDirectory)
                .Except(repl.References.Paths).ToArray();

            repl.AddReferences(dlls);

            foreach (var dll in dlls)
            {
                this.logger.InfoFormat("Added reference to {0}", dll);
            }

            return null;
        }
    }
}
