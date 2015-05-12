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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CShell.Sinks;
using Caliburn.Micro;
using Execute = CShell.Framework.Services.Execute;

namespace CShell.Sinks.Charting
{
    public class PlotSinkViewModel : Framework.Sink, IPlotSink
    {
        public PlotSinkViewModel(Uri uri)
        {
            Uri = uri;
            DisplayName = GetTitle(uri, "Plot");
        }

        private Chart chart;
        public Chart Chart
        {
            get { return chart ?? (chart = InitChart()); }
            set
            {
                chart = value;
                NotifyOfPropertyChange(() => Chart);
            }
        }

        private Chart InitChart()
        {
            var ch = new Chart();
            var chartArea = new ChartArea("Plots");
            chartArea.AxisY.IsStartedFromZero = false;
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(255, 185, 185, 185);
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(255, 185, 185, 185);
            chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            ch.ChartAreas.Add(chartArea);
            return ch;
        }

        private static bool CheckDataType(object values)
        {
            var isNested = false;
            return CheckDataType(values, out isNested);
        }

        private static bool CheckDataType(object values, out bool isNested)
        {
            // Only values of these types can be used in a Series: 
            // Double, Decimal, Single, int, long, uint, ulong, String, DateTime, short, ushort.
            if(values is IEnumerable<double> ||
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
            else if(values is IEnumerable<IEnumerable>)
            {
                isNested = true;
                var enumerable = (IEnumerable<IEnumerable>) values;
                var first = enumerable.FirstOrDefault();
                if (first != null)
                    return CheckDataType(first);
            }
            isNested = false;
            return false;
        }

        private const string SupportedTypes = "Supported types are: Double, Decimal, Single, int, long, uint, ulong, String, DateTime, short, ushort, and IEnumerable<IEnumerable> containing the previous types.";

        #region ISink Implementation
        public override void Dump(object o, string description)
        {
            var series = o as Series;

            // Only values of these types can be used in a Series: 
            // Double, Decimal, Single, int, long, uint, ulong, String, DateTime, short, ushort.
            if (series == null && CheckDataType(o))
            {
                var data = (IEnumerable)o;
                Plot(data, description);
            }

            if (series != null)
            {
                Plot(series);
                return;
            }

            //see if a whole chart is being plotted, if so, replace the current one with the new chart
            var newChart = o as Chart;
            if (newChart != null)
                Plot(newChart);
        }

        public override void Clear()
        {
            Plot(InitChart());
        }
        #endregion

        #region IPlotSink Implementation
        public void Plot(Series series)
        {
            Execute.OnUIThreadEx(() =>
            {
                try
                {
                    Chart.Series.Add(series);
                    Chart.ChartAreas[0].RecalculateAxesScale();
                }
                catch (Exception ex)
                {
                    LogManager.GetLog(typeof(PlotSinkViewModel)).Error(ex);
                }

            });
        }

        public void Plot(Chart newChart)
        {
            Execute.OnUIThreadEx(() => Chart = newChart);
        }

        public void Plot(IEnumerable ys, string seriesName = null, string chartType = "Line")
        {
            var isNested = false;
            if (!CheckDataType(ys, out isNested))
                throw new NotSupportedException("The provided Y values type is not supported. "+SupportedTypes);

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
                    var yValues = ((IEnumerable) value).Cast<object>().ToArray();
                    point.SetValueY(yValues);
                }
                series.Points.Add(point);
            }
            series.ChartTypeName = chartType;
            Plot(series);
        }

        public void Plot(IEnumerable ys, IEnumerable xs, string seriesName = null, string chartType = "Line")
        {
            var isNested = false;
            if (!CheckDataType(ys, out isNested))
                throw new NotSupportedException("The provided Y values type is not supported. "+SupportedTypes);
            if (!CheckDataType(xs))
                throw new NotSupportedException("The provided X values type is not supported. "+SupportedTypes);

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
            Plot(series);
        }

        public void PlotFunction(Func<double, double> f, string seriesName = null, string chartType = "Line", double from = -3.0, double to = 3.0, int points = 100)
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
            Plot(ys, xs, seriesName, chartType);
        }

        public void PlotHistogram(IEnumerable ys, string seriesName = null, string histogramName = null)
        {
            if (!CheckDataType(ys))
                throw new NotSupportedException("The provided Y values type is not supported. "+SupportedTypes);

            seriesName = String.IsNullOrEmpty(seriesName) ? "Data" : seriesName;
            histogramName = String.IsNullOrEmpty(histogramName) ? "Histogram" : histogramName;

            var series = new Series(seriesName);
            foreach (var value in ys)
                series.Points.AddY(value);

            var ch = InitChart();
            ch.Series.Add(series);

            var hist = new HistogramChartHelper();
            hist.CreateHistogram(ch, seriesName, histogramName);
            Plot(ch);
        }
        #endregion


        #region HistogramChartHelper
        /// <summary>
        /// Helper class that creates a histogram chart. Histogram is a data
        /// distribution chart which shows how many values, from the data series,
        /// are inside each segment interval.  
        /// 
        /// You can define how many intervals you want to have using the SegmentIntervalNumber
        /// field or the exact length of the interval using the SegmentIntervalWidth
        /// field. Actual segment interval number can be slightly different due
        /// to the automatic interval rounding.
        /// </summary>
        private class HistogramChartHelper
        {
            #region Fields

            /// <summary>
            /// Number of class intervals the data range is devided in.
            /// This property only has affect when "SegmentIntervalWidth" is 
            /// set to double.NaN.
            /// </summary>
            public int SegmentIntervalNumber = 20;

            /// <summary>
            /// Histogram class interval width. Setting this value to "double.NaN"
            /// will result in automatic width calculation based on the data range
            /// and number of required interval specified in "SegmentIntervalNumber".
            /// </summary>
            public double SegmentIntervalWidth = double.NaN;

            /// <summary>
            /// Indicates that percent frequency should be shown on the right axis
            /// </summary>
            public bool ShowPercentOnSecondaryYAxis = true;

            #endregion // Fields

            #region Methods

            /// <summary>
            /// Creates a histogram chart.
            /// </summary>
            /// <param name="chartControl">Chart control reference.</param>
            /// <param name="dataSeriesName">Name of the series which stores the original data.</param>
            /// <param name="histogramSeriesName">Name of the histogram series.</param>
            public void CreateHistogram(
                Chart chartControl,
                string dataSeriesName,
                string histogramSeriesName)
            {
                // Validate input
                if (chartControl == null)
                {
                    throw (new ArgumentNullException("chartControl"));
                }
                if (chartControl.Series.IndexOf(dataSeriesName) < 0)
                {
                    throw (new ArgumentException("Series with name'" + dataSeriesName + "' was not found.", "dataSeriesName"));
                }

                // Make data series invisible
                chartControl.Series[dataSeriesName].Enabled = false;

                // Check if histogram series exsists
                Series histogramSeries = null;
                if (chartControl.Series.IndexOf(histogramSeriesName) < 0)
                {
                    // Add new series
                    histogramSeries = chartControl.Series.Add(histogramSeriesName);

                    // Set new series chart type and other attributes
                    histogramSeries.ChartType = SeriesChartType.Column;
                    histogramSeries.BorderColor = Color.Black;
                    histogramSeries.BorderWidth = 1;
                    histogramSeries.BorderDashStyle = ChartDashStyle.Solid;
                }
                else
                {
                    histogramSeries = chartControl.Series[histogramSeriesName];
                    histogramSeries.Points.Clear();
                }

                // Get data series minimum and maximum values
                double minValue = double.MaxValue;
                double maxValue = double.MinValue;
                int pointCount = 0;
                foreach (DataPoint dataPoint in chartControl.Series[dataSeriesName].Points)
                {
                    // Process only non-empty data points
                    if (!dataPoint.IsEmpty)
                    {
                        if (dataPoint.YValues[0] > maxValue)
                        {
                            maxValue = dataPoint.YValues[0];
                        }
                        if (dataPoint.YValues[0] < minValue)
                        {
                            minValue = dataPoint.YValues[0];
                        }
                        ++pointCount;
                    }
                }

                // Calculate interval width if it's not set
                if (double.IsNaN(this.SegmentIntervalWidth))
                {
                    this.SegmentIntervalWidth = (maxValue - minValue) / SegmentIntervalNumber;
                    this.SegmentIntervalWidth = RoundInterval(this.SegmentIntervalWidth);
                }

                // Round minimum and maximum values
                minValue = Math.Floor(minValue / this.SegmentIntervalWidth) * this.SegmentIntervalWidth;
                maxValue = Math.Ceiling(maxValue / this.SegmentIntervalWidth) * this.SegmentIntervalWidth;

                // Create histogram series points
                double currentPosition = minValue;
                for (currentPosition = minValue; currentPosition <= maxValue; currentPosition += this.SegmentIntervalWidth)
                {
                    // Count all points from data series that are in current interval
                    int count = 0;
                    foreach (DataPoint dataPoint in chartControl.Series[dataSeriesName].Points)
                    {
                        if (!dataPoint.IsEmpty)
                        {
                            double endPosition = currentPosition + this.SegmentIntervalWidth;
                            if (dataPoint.YValues[0] >= currentPosition &&
                                dataPoint.YValues[0] < endPosition)
                            {
                                ++count;
                            }

                            // Last segment includes point values on both segment boundaries
                            else if (endPosition >= maxValue)
                            {
                                if (dataPoint.YValues[0] >= currentPosition &&
                                    dataPoint.YValues[0] <= endPosition)
                                {
                                    ++count;
                                }
                            }
                        }
                    }


                    // Add data point into the histogram series
                    histogramSeries.Points.AddXY(currentPosition + this.SegmentIntervalWidth / 2.0, count);
                }

                // Adjust series attributes
                histogramSeries["PointWidth"] = "1";

                // Adjust chart area
                ChartArea chartArea = chartControl.ChartAreas[histogramSeries.ChartArea];
                chartArea.AxisY.Title = "Frequency";
                chartArea.AxisX.Minimum = minValue;
                chartArea.AxisX.Maximum = maxValue;

                // Set axis interval based on the histogram class interval
                // and do not allow more than 10 labels on the axis.
                double axisInterval = this.SegmentIntervalWidth;
                while ((maxValue - minValue) / axisInterval > 10.0)
                {
                    axisInterval *= 2.0;
                }
                chartArea.AxisX.Interval = axisInterval;

                // Set chart area secondary Y axis
                chartArea.AxisY2.Enabled = AxisEnabled.Auto;
                if (this.ShowPercentOnSecondaryYAxis)
                {
                    chartArea.RecalculateAxesScale();

                    chartArea.AxisY2.Enabled = AxisEnabled.True;
                    chartArea.AxisY2.LabelStyle.Format = "P0";
                    chartArea.AxisY2.MajorGrid.Enabled = false;
                    chartArea.AxisY2.Title = "Percent of Total";

                    chartArea.AxisY2.Minimum = 0;
                    chartArea.AxisY2.Maximum = chartArea.AxisY.Maximum / (pointCount / 100.0);
                    double minStep = (chartArea.AxisY2.Maximum > 20.0) ? 5.0 : 1.0;
                    chartArea.AxisY2.Interval = Math.Ceiling((chartArea.AxisY2.Maximum / 5.0 / minStep)) * minStep;

                }
            }

            /// <summary>
            /// Helper method which rounds specified axsi interval.
            /// </summary>
            /// <param name="interval">Calculated axis interval.</param>
            /// <returns>Rounded axis interval.</returns>
            internal double RoundInterval(double interval)
            {
                // If the interval is zero return error
                if (interval == 0.0)
                {
                    throw (new ArgumentOutOfRangeException("interval", "Interval can not be zero."));
                }

                // If the real interval is > 1.0
                double step = -1;
                double tempValue = interval;
                while (tempValue > 1.0)
                {
                    step++;
                    tempValue = tempValue / 10.0;
                    if (step > 1000)
                    {
                        throw (new InvalidOperationException("Auto interval error due to invalid point values or axis minimum/maximum."));
                    }
                }

                // If the real interval is < 1.0
                tempValue = interval;
                if (tempValue < 1.0)
                {
                    step = 0;
                }

                while (tempValue < 1.0)
                {
                    step--;
                    tempValue = tempValue * 10.0;
                    if (step < -1000)
                    {
                        throw (new InvalidOperationException("Auto interval error due to invalid point values or axis minimum/maximum."));
                    }
                }

                double tempDiff = interval / Math.Pow(10.0, step);
                if (tempDiff < 3.0)
                {
                    tempDiff = 2.0;
                }
                else if (tempDiff < 7.0)
                {
                    tempDiff = 5.0;
                }
                else
                {
                    tempDiff = 10.0;
                }

                // Make a correction of the real interval
                return tempDiff * Math.Pow(10.0, step);
            }

            #endregion // Methods
        }//end HistogramChartHelper
        #endregion


        
    }
}
