using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CShell.Sinks.Charting
{
    /// <summary>
    /// Interaction logic for ChartSinkView.xaml
    /// </summary>
    public partial class ChartSinkView : UserControl
    {
        public ChartSinkView()
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
