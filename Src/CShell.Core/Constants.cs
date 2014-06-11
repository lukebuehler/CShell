#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2012  Arnova Asset Management Ltd., Lukas Buhler
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion
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

        public const string CShellModulesPath = "Modules";
        public const string CShellTemplatesPath = "Templates";
        public const string CShellDefaultWorkspacePath = "Default\\";
        public const string CShellEmptyFile = "Empty.cshell";
        public const string CShellFileTypes = "Workspace|*.cshell|All Files|*.*";
        public const string CShellFileExtension = ".cshell";
        public const string LayoutFile = "layout.config";
        public const string ReferencesFile = "references.csx";


        public const string SinkXhtml = "sink://cshell/xhtml/";
        public const string SinkPlot = "sink://cshell/plot/";
        public const string SinkGrid = "sink://cshell/grid/";

        public const string PackagesFile = "packages.config";
        public const string NugetFile = "nuget.config";
        public const string PackagesFolder = "packages";
        public const string BinFolder = "bin";
    }
}
