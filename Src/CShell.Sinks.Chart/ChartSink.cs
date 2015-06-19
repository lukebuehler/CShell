using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using CShell.Framework;

namespace CShell.Sinks.Charting
{
    //todo:
    //void PlotMultiple(IEnumerable<IEnumerable> yss, string seriesName = null, string chartType = "Line");
    //ability to choose color


    /// <summary>
    /// Static API and extension methods for charting.
    /// </summary>
    public static class ChartSink
    {
        public const string ChartSinkUri = "sink://cshell/chart/";
        private const string SupportedTypes = "Supported types are: Double, Decimal, Single, int, long, uint, ulong, String, DateTime, short, ushort, and IEnumerable<IEnumerable> containing the previous types.";

        /// <summary>
        /// Gets the available plot chart types.
        /// </summary>
        public static string[] SeriesChartTypes
        {
            get { return Enum.GetNames(typeof(SeriesChartType)); }
        }

        /// <summary>
        /// Gets a chart sink.
        /// If the chart exists or can be created the plot is opened.
        /// </summary>
        /// <param name="sinkName">Name of the sink.</param>
        public static ISink GetChartSink(string sinkName)
        {
            return GetChartSink(sinkName, false);
        }

        /// <summary>
        /// Gets a chart sink.
        /// </summary>
        /// <param name="sinkName">Name of the chart.</param>
        /// <param name="suppressOpen">If set to <c>true</c> the chart is not opened.</param>
        public static ISink GetChartSink(string sinkName, bool suppressOpen)
        {
            var uri = new Uri(ChartSinkUri + sinkName);
            return Shell.GetSink(uri, suppressOpen);
        }

        #region Plot
        /// <summary>
        /// Plots the specified series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="sinkName">Name of the sink.</param>
        public static void Plot(this Series series, string sinkName = null)
        {
            var sink = GetChartSink(sinkName);
            sink.Dump(series);
        }

        /// <summary>
        /// Plots the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="sinkName">Name of the sink.</param>
        public static void Plot(this Chart chart, string sinkName = null)
        {
            var sink = GetChartSink(sinkName);
            sink.Dump(chart);
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
            var isNested = false;
            if (!CheckDataType(ys, out isNested))
                throw new NotSupportedException("The provided Y values type is not supported. " + SupportedTypes);

            var series = seriesName != null ? new Series(seriesName) : new Series();
            foreach (var value in ys)
            {
                var point = new DataPoint();
                if (!isNested)
                {
                    point.SetValueY(value);
                }
                else
                {
                    var yValues = ((IEnumerable)value).Cast<object>().ToArray();
                    point.SetValueY(yValues);
                }
                series.Points.Add(point);
            }
            series.ChartTypeName = chartType;

            GetChartSink(sinkName).Dump(series);
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
            var isNested = false;
            if (!CheckDataType(ys, out isNested))
                throw new NotSupportedException("The provided Y values type is not supported. " + SupportedTypes);
            if (!CheckDataType(xs))
                throw new NotSupportedException("The provided X values type is not supported. " + SupportedTypes);

            var series = seriesName != null ? new Series(seriesName) : new Series();
            var xsObjects = xs.Cast<object>();
            var ysObjects = ys.Cast<object>();
            var xys = xsObjects.Zip(ysObjects, Tuple.Create);
            foreach (var xy in xys)
            {
                var point = new DataPoint();
                if (!isNested)
                {
                    point.SetValueXY(xy.Item1, xy.Item2);
                }
                else
                {
                    var yValues = ((IEnumerable)xy.Item2).Cast<object>().ToArray();
                    point.SetValueXY(xy.Item1, yValues);
                }
                series.Points.Add(point);
            }
            series.ChartTypeName = chartType;

            GetChartSink(sinkName).Dump(series);
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
            if (from >= to)
                throw new ArgumentException("The 'from' parameter cannot be greater or the same as the 'to' parameter.");
            if (points < 1)
                throw new ArgumentException("The plot has to have at least 1 point.");

            var distance = Math.Abs(to - from);
            var step = distance / points;
            var ys = new double[points + 1];
            var xs = new double[points + 1];
            for (int i = 0; i <= points; i++)
            {
                var x = from + i * step;
                var y = f(x);
                xs[i] = x;
                ys[i] = y;
            }

            Plot(ys, xs, seriesName, chartType, sinkName);
        }

        /// <summary>
        /// Plots the Y values as a histogram.
        /// </summary>
        /// <param name="ys">The Y values to plot.</param>
        /// <param name="intervalNumber">Number of class intervals the data range is devided in. This property only has affect when "SegmentIntervalWidth" is set to double.NaN.</param>
        /// <param name="intervalWidth">Histogram class interval width. Setting this value to "double.NaN" will result in automatic width calculation based on the data range and number of required interval specified in "SegmentIntervalNumber".</param>
        /// <param name="showPercentOnSecondaryYAxis">Indicates that percent frequency should be shown on the right axis</param>
        /// <param name="sinkName">Name of the sink.</param>
        public static void Histogram(this IEnumerable ys, int intervalNumber = 20, double intervalWidth = double.NaN, bool showPercentOnSecondaryYAxis = true, string sinkName = null)
        {
            if (!CheckDataType(ys))
                throw new NotSupportedException("The provided Y values type is not supported. " + SupportedTypes);

            var seriesName = "Data";
            var histogramName = "Histogram";

            var series = new Series(seriesName);
            foreach (var value in ys)
                series.Points.AddY(value);

            var ch = ChartSinkViewModel.CreateChart();
            ch.Series.Add(series);

            var hist = new HistogramChartHelper();
            hist.SegmentIntervalNumber = intervalNumber;
            hist.SegmentIntervalWidth = intervalWidth;
            hist.ShowPercentOnSecondaryYAxis = showPercentOnSecondaryYAxis;
            hist.CreateHistogram(ch, seriesName, histogramName);

            GetChartSink(sinkName).Dump(ch);
        }
        #endregion

        #region Create
        public static Chart CreateChart(string sinkName = null)
        {
            var sink = GetChartSink(sinkName);
            var chart = ChartSinkViewModel.CreateChart();
            sink.Dump(chart);
            return chart;
        }

        public static Series CreateSeries(string seriesName = null, string sinkName = null)
        {
            var sink = GetChartSink(sinkName);
            var series = seriesName != null ? new Series(seriesName) : new Series();
            sink.Dump(series);
            return series;
        }
        #endregion


        #region Check Data Types
        private static bool CheckDataType(object values)
        {
            var isNested = false;
            return CheckDataType(values, out isNested);
        }

        private static bool CheckDataType(object values, out bool isNested)
        {
            // Only values of these types can be used in a Series: 
            // Double, Decimal, Single, int, long, uint, ulong, String, DateTime, short, ushort.
            if (values is IEnumerable<double> ||
                values is IEnumerable<decimal> ||
                values is IEnumerable<Single> ||
                values is IEnumerable<int> ||
                values is IEnumerable<long> ||
                values is IEnumerable<uint> ||
                values is IEnumerable<ulong> ||
                values is IEnumerable<short> ||
                values is IEnumerable<ushort> ||
                values is IEnumerable<String> ||
                values is IEnumerable<DateTime>)
            {
                isNested = false;
                return true;
            }
            else if (values is IEnumerable<IEnumerable>)
            {
                isNested = true;
                var enumerable = (IEnumerable<IEnumerable>)values;
                var first = enumerable.FirstOrDefault();
                if (first != null)
                    return CheckDataType(first);
            }
            isNested = false;
            return false;
        }
        #endregion
    }
}
