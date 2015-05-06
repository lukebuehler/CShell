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
using System.Linq;
using System.Text;
using CShell.Framework;
using CShell.Framework.Services;
using Caliburn.Micro;

namespace CShell
{
    public static partial class Shell
    {
        /// <summary>
        /// Gets a specific tool based on the URI. 
        /// If the tool URI exists or can be created the tool is opened.
        /// </summary>
        /// <param name="uri">The tool URI.</param>
        public static ITool GetTool(Uri uri)
        {
            return GetTool(uri, false);
        }

        /// <summary>
        /// Gets a specific tool based on the uri.
        /// </summary>
        /// <param name="uri">The tool URI.</param>
        /// <param name="suppressOpen">If set to <c>true</c> tool will not be opened, but just created.</param>
        /// <returns></returns>
        public static ITool GetTool(Uri uri, bool suppressOpen)
        {
            var tools = UI.Tools.ToArray();
            var tool = tools.FirstOrDefault(t => t.Uri == uri);
            if (tool == null)
            {
                tool = IoC.GetAllInstances(typeof(ITool))
                    .Cast<ITool>()
                    .FirstOrDefault(t=>t.Uri == uri);

                if (tool != null && !suppressOpen)
                    UI.ShowTool(tool);
            }
            return tool;
        }
    }
}
