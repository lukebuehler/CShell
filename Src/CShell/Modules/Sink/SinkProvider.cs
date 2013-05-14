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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using CShell.Framework.Services;
using CShell.Modules.Sink.ViewModels;

namespace CShell.Modules.Sink
{
    [Export(typeof(ISinkProvider))]
    public class SinkProvider : ISinkProvider
    {
        public bool Handles(Uri uri)
        {
            return uri.Scheme == "sink" && uri.Host == "cshell";
        }

        /// <summary>
        /// Creates a CShell sink.
        /// The CShell sink URI is arranged like this:
        ///   sink://cshell/SinkType/SinkName
        /// for example, following uri would create a XHTML window named "Hi"
        ///   sink://cshell/xhtml/Hi
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>If the URI was correct a sink, otherwise null.</returns
        public Framework.ISink Create(Uri uri)
        {
            var pathParts = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathParts.Length > 0)
            {
                //the first part of the uri is the sink type
                var sinkType = pathParts[0].ToLower();
                if (sinkType == "xhtml")
                {
                    return new XhtmlSinkViewModel(uri);
                }
                else if (sinkType == "control")
                {
                    return new ControlSinkViewModel(uri);
                }
                else if (sinkType == "grid")
                {
                    return new GridSinkViewModel(uri);
                }
                else if (sinkType == "plot")
                {
                    return new PlotSinkViewModel(uri);
                }

            }
            return null;
        }
    }
}
