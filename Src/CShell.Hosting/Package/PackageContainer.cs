namespace CShell.Hosting.Package
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Versioning;

    using NuGet;

    using ScriptCs.Contracts;
    using ScriptCs.Logging;

    using IFileSystem = ScriptCs.Contracts.IFileSystem;
    using PackageReference = ScriptCs.PackageReference;

    public class PackageContainer : IPackageContainer
    {
        private const string DotNetFramework = ".NETFramework";

        private const string DotNetPortable = ".NETPortable";

        private readonly IFileSystem fileSystem;

        private readonly ILog logger;

        public PackageContainer(IFileSystem fileSystem, ILog logger)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
        }

        public void CreatePackageFile()
        {
            var packagesFile = Path.Combine(this.fileSystem.CurrentDirectory, this.fileSystem.PackagesFile);
            var packageReferenceFile = new PackageReferenceFile(packagesFile);

            var packagesFolder = Path.Combine(this.fileSystem.CurrentDirectory, this.fileSystem.PackagesFolder);
            var repository = new LocalPackageRepository(packagesFolder);

            var newestPackages = repository.GetPackages().GroupBy(p => p.Id)
                .Select(g => g.OrderByDescending(p => p.Version).FirstOrDefault());

            if (!newestPackages.Any())
            {
                this.logger.Info("No packages found!");
                return;
            }

            this.logger.InfoFormat("{0} {1}...", File.Exists(packagesFile) ? "Updating" : "Creating", this.fileSystem.PackagesFile);

            foreach (var package in newestPackages)
            {
                var newestFramework = GetNewestSupportedFramework(package);

                if (!packageReferenceFile.EntryExists(package.Id, package.Version))
                {
                    packageReferenceFile.AddEntry(package.Id, package.Version, package.DevelopmentDependency, newestFramework);

                    if (newestFramework == null)
                    {
                        this.logger.InfoFormat("Added {0} (v{1}) to {2}", package.Id, package.Version, this.fileSystem.PackagesFile);
                    }
                    else
                    {
                        this.logger.InfoFormat("Added {0} (v{1}, .NET {2}) to {3}", package.Id, package.Version, newestFramework.Version, this.fileSystem.PackagesFile);
                    }

                    continue;
                }

                this.logger.InfoFormat("Skipped {0} because it already exists.", package.Id);
            }

            this.logger.InfoFormat("Successfully {0} {1}.", File.Exists(packagesFile) ? "updated" : "created", this.fileSystem.PackagesFile);
        }

        public IPackageObject FindPackage(string path, IPackageReference packageRef)
        {
            var repository = new LocalPackageRepository(path);

            var package = packageRef.Version != null && !(packageRef.Version.Major == 0 && packageRef.Version.Minor == 0)
                ? repository.FindPackage(packageRef.PackageId, new SemanticVersion(packageRef.Version, packageRef.SpecialVersion), true, true)
                : repository.FindPackage(packageRef.PackageId);

            return package == null ? null : new PackageObject(package, packageRef.FrameworkName);
        }

        public IEnumerable<IPackageReference> FindReferences(string path)
        {
            var packageReferenceFile = new PackageReferenceFile(path);

            var references = packageReferenceFile.GetPackageReferences().ToList();
            if (references.Any())
            {
                foreach (var packageReference in references)
                {
                    yield return new PackageReference(
                            packageReference.Id,
                            packageReference.TargetFramework,
                            packageReference.Version.Version,
                            packageReference.Version.SpecialVersion);
                }

                yield break;
            }

            // No packages.config, check packages folder
            var packagesFolder = Path.Combine(this.fileSystem.GetWorkingDirectory(path), this.fileSystem.PackagesFolder);
            if (!this.fileSystem.DirectoryExists(packagesFolder))
            {
                yield break;
            }

            var repository = new LocalPackageRepository(packagesFolder);

            var arbitraryPackages = repository.GetPackages();
            if (!arbitraryPackages.Any())
            {
                yield break;
            }

            foreach (var arbitraryPackage in arbitraryPackages)
            {
                var newestFramework = GetNewestSupportedFramework(arbitraryPackage)
                    ?? VersionUtility.EmptyFramework;

                yield return new PackageReference(
                        arbitraryPackage.Id,
                        newestFramework,
                        arbitraryPackage.Version.Version,
                        arbitraryPackage.Version.SpecialVersion);
            }
        }

        private static FrameworkName GetNewestSupportedFramework(IPackage packageMetadata)
        {
            return packageMetadata.GetSupportedFrameworks()
                .Where(IsValidFramework)
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();
        }

        private static bool IsValidFramework(FrameworkName frameworkName)
        {
            return frameworkName.Identifier == DotNetFramework
                || (frameworkName.Identifier == DotNetPortable
                    && frameworkName.Profile.Split('+').Any(IsValidProfile));
        }

        private static bool IsValidProfile(string profile)
        {
            return profile == "net40" || profile == "net45";
        }
    }
}