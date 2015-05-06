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
using System.Windows.Forms.DataVisualization.Charting;
using CShell.Framework;
using CShell.Sinks;

namespace CShell
{
    public partial class Shell
    {
        /// <summary>
        /// Gets a plot sink.
        /// If the plot exists or can be created the plot is opened.
        /// </summary>
        /// <param name="sinkName">Name of the sink.</param>
        public static IPlotSink GetPlotSink(string sinkName)
        {
            return GetPlotSink(sinkName, false);
        }

        /// <summary>
        /// Gets a plot sink.
        /// </summary>
        /// <param name="sinkName">Name of the sink.</param>
        /// <param name="suppressOpen">If set to <c>true</c> the plot is not opened.</param>
        public static IPlotSink GetPlotSink(string sinkName, bool suppressOpen)
        {
            var uri = new Uri(Constants.SinkPlot + sinkName);
            return GetSink(uri, suppressOpen) as IPlotSink;
        }

        /// <summary>
        /// Plots the specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="sinkName">Name of the sink.</param>
        public static void Plot(this Series series, string sinkName = null)
        {
            var sink = GetPlotSink(sinkName);
            sink.Plot(series);
        }

        /// <summary>
        /// Plots the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="sinkName">Name of the sink.</param>
        public static void Plot(this Chart chart, string sinkName = null)
        {
            var sink = GetPlotSink(sinkName);
            sink.Plot(chart);
        }

        /// <summary>
        /// Plots the specified Y values.
        /// </summary>
        /// <param name="ys">The Y values.</param>
        /// <param name="seriesName">Name of the series.</param>
        /// <param name="chartType">The chart type to be used for the points, see the PlotChartTypes property for all available types.</param>
        /// <param name="sinkName">Name of the sink.</param>
        public static void Plot(this IEnumerable ys, string seriesName = null, string chartType = "Line", string sinkName = null)
        {
            var sink = GetPlotSink(sinkName);
            sink.Plot(ys, seriesName, chartType);
        }

        /// <summary>
        /// Plots the specified Y values at the corresponding X points.
        /// </summary>
        /// <param name="ys">The Y values.</param>
        /// <param name="xs">The X values.</param>
        /// <param name="seriesName">Name of the series.</param>
        /// <param name="chartType">The chart type to be used for the points, see the PlotChartTypes property for all available types.</param>
        /// <param name="sinkName">Name of the sink.</param>
        public static void Plot(this IEnumerable ys, IEnumerable xs, string seriesName = null, string chartType = "Line", string sinkName = null)
        {
            var sink = GetPlotSink(sinkName);
            sink.Plot(ys, xs, seriesName, chartType);
        }

        /// <summary>
        /// Plots a function that produces Y values.
        /// </summary>
        /// <param name="f">The function to plot.</param>
        /// <param name="seriesName">Name of the resulting series.</param>
        /// <param name="chartType">The chart type to be used for the points, see the PlotChartTypes property for all available types.</param>
        /// <param name="from">At what X value to start the plot.</param>
        /// <param name="to">At what X value to end the plot.</param>
        /// <param name="points">How many points to plot between start and end.</param>
        /// <param name="sinkName">Name of the sink.</param>
        public static void PlotFunction(this Func<double, double> f, string seriesName = null, string chartType = "Line", double from = -3.0, double to = 3.0, int points = 100, string sinkName = null)
        {
            var sink = GetPlotSink(sinkName);
            sink.PlotFunction(f, seriesName, chartType, from, to, points);
        }

        /// <summary>
        /// Plots the Y values as a histogram.
        /// </summary>
        /// <param name="ys">The Y values to plot.</param>
        /// <param name="seriesName">Name of the series.</param>
        /// <param name="histogramName">Name of the resulting histogram.</param>
        /// <param name="sinkName">Name of the sink.</param>
        public static void PlotHistogram(this IEnumerable ys, string seriesName = null, string histogramName = null, string sinkName = null)
        {
            var sink = GetPlotSink(sinkName);
            sink.PlotHistogram(ys, seriesName, histogramName);
        }

        /// <summary>
        /// Gets the available plot chart types.
        /// </summary>
        public static string[] PlotChartTypes
        {
            get { return Enum.GetNames(typeof(SeriesChartType)); }
        }
    }
}
