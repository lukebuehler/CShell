namespace CShell.Hosting.Package
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ScriptCs.Contracts;
    using ScriptCs.Logging;

    public class PackageInstaller : IPackageInstaller
    {
        private readonly IInstallationProvider installer;
        private readonly ILog logger;

        public PackageInstaller(IInstallationProvider installer, ILog logger)
        {
            this.installer = installer;
            this.logger = logger;
        }

        public void InstallPackages(IEnumerable<IPackageReference> packageIds, bool allowPreRelease = false)
        {
            if (packageIds == null)
            {
                throw new ArgumentNullException("packageIds");
            }

            packageIds = packageIds.ToList();

            if (!packageIds.Any())
            {
                this.logger.Info("Nothing to install.");
                return;
            }

            var exceptions = new List<Exception>();
            foreach (var packageId in packageIds)
            {
                if (this.installer.IsInstalled(packageId, allowPreRelease))
                {
                    continue;
                }

                try
                {
                    this.installer.InstallPackage(packageId, allowPreRelease);
                }
                catch (Exception ex)
                {
                    this.logger.ErrorException(ex.Message, ex);
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
