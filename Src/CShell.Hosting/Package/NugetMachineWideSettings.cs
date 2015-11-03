namespace CShell.Hosting.Package
{
    using System;
    using System.Collections.Generic;

    using NuGet;

    internal class NugetMachineWideSettings : IMachineWideSettings
    {
        private readonly Lazy<IEnumerable<Settings>> settings;

        public NugetMachineWideSettings()
        {
            var baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            this.settings = new Lazy<IEnumerable<Settings>>(() => NuGet.Settings.LoadMachineWideSettings(new PhysicalFileSystem(baseDirectory)));
        }

        public IEnumerable<Settings> Settings
        {
            get
            {
                return this.settings.Value;
            }
        }
    }
}