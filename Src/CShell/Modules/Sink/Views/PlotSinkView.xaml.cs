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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CShell.Modules.Sink.ViewModels;

namespace CShell.Modules.Sink.Views
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
            var vm = e.OldValue as PlotSinkViewModel;
            if(vm != null)
            {
                vm.PropertyChanged -= VmOnPropertyChanged;
            }
            vm = e.NewValue as PlotSinkViewModel;
            if(vm != null)
            {
                vm.PropertyChanged += VmOnPropertyChanged;
                FormHost.Child = vm.Chart;
            }
        }

        private void VmOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var vm = sender as PlotSinkViewModel;
            if (vm != null)
            {
                FormHost.Child = vm.Chart;
            }
        }
    }
}
