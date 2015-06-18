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
    public class ChartSinkViewModel : Framework.Sink
    {
        public ChartSinkViewModel(Uri uri)
        {
            Uri = uri;
            DisplayName = GetTitle(uri, "Chart");
        }

        private Chart chart;
        public Chart Chart
        {
            get { return chart ?? (chart = CreateChart()); }
            set
            {
                chart = value;
                NotifyOfPropertyChange(() => Chart);
            }
        }

        public override void Dump(object o)
        {
            Execute.OnUIThreadEx(() =>
            {
                try
                {
                    var series = o as Series;
                    if (series != null)
                    {
                        Chart.Series.Add(series);
                    }

                    //see if a whole chart is being plotted, if so, replace the current one with the new chart
                    var newChart = o as Chart;
                    if (newChart != null)
                    {
                        Chart = newChart;
                    }
                }
                catch (Exception ex)
                {
                    LogManager.GetLog(typeof(ChartSinkViewModel)).Error(ex);
                }
            });
        }

        public override void Clear()
        {
            Dump(CreateChart());
        }

        public static Chart CreateChart()
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

    }
}
