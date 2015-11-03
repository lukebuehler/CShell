namespace CShell.Hosting.Package
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using NuGet;

    using ScriptCs.Contracts;
    using ScriptCs.Logging;

    using IFileSystem = ScriptCs.Contracts.IFileSystem;

    public class NugetInstallationProvider : IInstallationProvider
    {
        private readonly IFileSystem fileSystem;
        private readonly ILog logger;
        private PackageManager manager;
        private IEnumerable<string> repositoryUrls;

        private static readonly Version EmptyVersion = new Version();

        public NugetInstallationProvider(IFileSystem fileSystem, ILog logger)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
        }

        public void Initialize()
        {
            var path = Path.Combine(this.fileSystem.CurrentDirectory, this.fileSystem.PackagesFolder);
            this.repositoryUrls = this.GetRepositorySources(path);
            var remoteRepository = new AggregateRepository(PackageRepositoryFactory.Default, this.repositoryUrls, true);
            this.manager = new PackageManager(remoteRepository, path);
        }

        public IEnumerable<string> GetRepositorySources(string path)
        {
            var configFileSystem = new PhysicalFileSystem(path);

            var localNuGetConfigFile = Path.Combine(this.fileSystem.CurrentDirectory, this.fileSystem.NugetFile);
            var settings = this.fileSystem.FileExists(localNuGetConfigFile)
                               ? Settings.LoadDefaultSettings(configFileSystem, localNuGetConfigFile, null)
                               : Settings.LoadDefaultSettings(configFileSystem, null, new NugetMachineWideSettings());

            if (settings == null)
            {
                return new[] { CShell.Constants.DefaultRepositoryUrl };
            }

            var sourceProvider = new PackageSourceProvider(settings);
            var sources = sourceProvider.LoadPackageSources().Where(i => i.IsEnabled);

            if (!sources.Any())
            {
                return new[] { CShell.Constants.DefaultRepositoryUrl };
            }

            return sources.Select(i => i.Source);
        }

        public void InstallPackage(IPackageReference packageId, bool allowPreRelease = false)
        {
            var version = GetVersion(packageId);
            var packageName = packageId.PackageId + " " + (version == null ? string.Empty : packageId.Version.ToString());
            this.manager.InstallPackage(packageId.PackageId, version, allowPrereleaseVersions: allowPreRelease, ignoreDependencies: false);
            this.logger.Info("Installed: " + packageName);
        }

        private static SemanticVersion GetVersion(IPackageReference packageReference)
        {
            return packageReference.Version == EmptyVersion ? null : new SemanticVersion(packageReference.Version, packageReference.SpecialVersion);
        }

        public bool IsInstalled(IPackageReference packageReference, bool allowPreRelease = false)
        {
            var version = GetVersion(packageReference);
            return this.manager.LocalRepository.FindPackage(packageReference.PackageId, version, allowPreRelease, allowUnlisted: false) != null;
        }
    }
}