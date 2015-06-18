using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace CShell.Sinks.Charting
{
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
    internal class HistogramChartHelper
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
}
