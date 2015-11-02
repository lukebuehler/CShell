using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CShell
{
    public static class Constants
    {
        public const string CShellHelp = "https://github.com/ArnovaAssetManagement/CShell";


        public const string FileFilter = "C# Files|*.csx;*.cs;|All Files|*.*";
        public const string FileTypes = "C# Script|*.csx|C# File|*.cs|Text File|*.txt|Other|*.*";
        public const string DefaultExtension = ".csx";

        public const string AssemblyFileFilter = "Component Files|*.dll;*.exe|All Files|*.*";
        public const string AssemblyFileExtension = ".dll";

        public const string WorkspaceFileExtension = ".xml";
        public const string WorkspaceFilter = "*.xml *.dll *.cshell";

        public const string ModulesPath = "Modules";
        public const string TemplatesPath = "Templates";
        public const string DefaultWorkspacePath = "Default\\";
        public const string ReferencesFile = "references.csx";
        public const string LayoutFile = "layout.config";
        public const string ConfigFile = "config.csx";

        public const string PackagesFile = "packages.config";
        public const string NugetFile = "nuget.config";
        public const string PackagesFolder = "packages";
        public const string DefaultRepositoryUrl = "https://nuget.org/api/v2/";

        public const string BinFolder = "bin";
        public const string DllCacheFolder = "cache";

        public const string NetFrameworkName = ".NETFramework,Version=v4.5";

    }
}
