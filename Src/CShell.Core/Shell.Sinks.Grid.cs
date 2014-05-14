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

namespace CShell
{
    public partial class Shell
    {
        /// <summary>
        /// Gets a grid sink based on the sink name.
        /// </summary>
        /// <param name="sinkName">Name of the sink.</param>
        /// <param name="suppressOpen">If set to <c>true</c> the grid is not opened.</param>
        private static ISink GetGridSink(string sinkName, bool suppressOpen = false)
        {
            var uri = new Uri(Constants.SinkGrid + sinkName);
            return GetSink(uri, suppressOpen);
        }

        /// <summary>
        /// Dumps data to a table.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="sinkName">Name of the sink.</param>
        public static void DumpGrid(this IEnumerable data, string sinkName = null)
        {
            var sink = GetGridSink(sinkName);
            sink.Dump(data);
        }
    }
}
