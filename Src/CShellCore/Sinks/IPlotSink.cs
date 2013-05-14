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
using System.Windows.Forms.DataVisualization.Charting;
using CShell.Framework;

namespace CShell.Sinks
{
    public interface IPlotSink : ISink
    {
        void Plot(Series series);
        void Plot(Chart chart);
        void Plot(IEnumerable ys, string seriesName = null, string chartType = "Line");
        void Plot(IEnumerable ys, IEnumerable xs, string seriesName = null, string chartType = "Line");

        //todo:
        //void PlotMultiple(IEnumerable<IEnumerable> yss, string seriesName = null, string chartType = "Line");
        //ability to choose color

        void PlotFunction(Func<double, double> f, string seriesName = null, string chartType = "Line", double from = -3.0, double to = 3.0, int points = 100);

        void PlotHistogram(IEnumerable ys, string seriesName = null, string histogramName = null);
    }
}
