namespace CShell.Hosting.Package
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Versioning;

    using NuGet;

    using ScriptCs.Contracts;

    internal class PackageObject : IPackageObject
    {
        private const string Dll = ".dll";
        private const string Exe = ".exe";
        private readonly IPackage package;

        public PackageObject(IPackage package, FrameworkName frameworkName)
        {
            this.package = package;
            this.FrameworkName = frameworkName;
            this.Id = package.Id;
            this.Version = package.Version.Version;
            this.TextVersion = package.Version.ToString();
            this.FrameworkAssemblies = package.FrameworkAssemblies
                .Where(x => x.SupportedFrameworks.Any(y => y == frameworkName))
                .Select(x => x.AssemblyName);

            var dependencies = this.package.GetCompatiblePackageDependencies(frameworkName);
            if (dependencies != null)
            {
                this.Dependencies = dependencies.Select(i => new PackageObject(i.Id) { FrameworkName = frameworkName });
            }
        }

        public PackageObject(string packageId)
        {
            this.Id = packageId;
        }

        public IEnumerable<string> FrameworkAssemblies { get; private set; }

        public string Id { get; private set; }

        public string TextVersion { get; private set; }

        public Version Version { get; private set; }

        public FrameworkName FrameworkName { get; private set; }

        public IEnumerable<IPackageObject> Dependencies { get; set; }

        public string FullName
        {
            get { return this.Id + "." + this.TextVersion; }
        }

        public IEnumerable<string> GetCompatibleDlls(FrameworkName frameworkName)
        {
            var dlls = this.package.GetLibFiles().Where(i => i.EffectivePath.EndsWith(Dll) || i.EffectivePath.EndsWith(Exe));
            IEnumerable<IPackageFile> compatibleFiles;
            VersionUtility.TryGetCompatibleItems(frameworkName, dlls, out compatibleFiles);

            return compatibleFiles != null ? compatibleFiles.Select(i => i.Path) : null;
        }

        public IEnumerable<string> GetContentFiles()
        {
            return this.package.GetContentFiles().Select(file => file.Path);
        }
    }
}