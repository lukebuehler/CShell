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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CShell.Sinks.Charting
{
    /// <summary>
    /// Interaction logic for PlotSinkView.xaml
    /// </summary>
    public partial class PlotSinkView : UserControl
    {
        public PlotSinkView()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.OldValue as ChartSinkViewModel;
            if(vm != null)
            {
                vm.PropertyChanged -= VmOnPropertyChanged;
            }
            vm = e.NewValue as ChartSinkViewModel;
            if(vm != null)
            {
                vm.PropertyChanged += VmOnPropertyChanged;
                FormHost.Child = vm.Chart;
            }
        }

        private void VmOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var vm = sender as ChartSinkViewModel;
            if (vm != null)
            {
                FormHost.Child = vm.Chart;
            }
        }
    }
}
