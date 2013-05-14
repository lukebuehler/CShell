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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CShell.Framework;
using CShell.Sinks;

namespace CShell
{
    public partial class Shell
    {
        /// <summary>
        /// Gets a XHTML sink.
        /// </summary>
        /// <param name="sinkName">Name of the sink.</param>
        /// <param name="suppressOpen">If set to <c>true</c> the sink is not opened.</param>
        /// <returns></returns>
        private static IXhtmlSink GetXhtmlSink(string sinkName, bool suppressOpen = false)
        {
            var uri = new Uri(Constants.SinkXhtml + sinkName);
            return GetSink(uri, suppressOpen) as IXhtmlSink;
        }

        /// <summary>
        /// Dumps the object to a XHTML sink.
        /// </summary>
        /// <param name="o">The object to dump.</param>
        /// <param name="description">A description of the object.</param>
        /// <param name="depth">If it's a nested type how many sub-types should be printed.</param>
        /// <param name="sinkName">Name of the sink.</param>
        public static void DumpXhtml(this object o, string description = null, int depth = 3, string sinkName = null)
        {
            var sink = GetXhtmlSink(sinkName);
            sink.Dump(o, description, depth);
        }
    }
}
