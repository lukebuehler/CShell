#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2013  Arnova Asset Management Ltd., Lukas Buhler
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
using System.IO;
using System.Linq;
using System.Text;

namespace CShell.Util
{
    public static class PathHelper
    {
        public static String ToAbsolutePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(toPath))
                return null;
            if (String.IsNullOrEmpty(fromPath))
                return toPath;

            Uri path2 = new Uri(toPath, UriKind.RelativeOrAbsolute);
            if (path2.IsAbsoluteUri)
                return toPath;
            Uri basePath = new Uri(fromPath + "/", UriKind.Absolute);
            Uri absPath = new Uri(basePath, toPath);
            return absPath.LocalPath;
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static String ToRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(toPath))
                return null;
            if (String.IsNullOrEmpty(fromPath))
                return toPath;

            Uri uri1 = new Uri(toPath, UriKind.RelativeOrAbsolute);
            if (uri1.IsAbsoluteUri)
            {
                Uri uri2 = new Uri(fromPath + "/", UriKind.Absolute);
                Uri relativeUri = uri2.MakeRelativeUri(uri1);
                return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar);
            }
            // else it is already a relative path
            return toPath;
        }
    }
}
