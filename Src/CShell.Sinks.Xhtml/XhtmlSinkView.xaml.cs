using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CShell.Sinks.Xhtml
{
    /// <summary>
    /// Interaction logic for XhtmlSinkView.xaml
    /// </summary>
    public partial class XhtmlSinkView : UserControl
    {
        public XhtmlSinkView()
        {
            InitializeComponent();
        }

        private void webBrowser_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as XhtmlSinkViewModel;
            if (vm != null)
            {
                vm.PropertyChanged += VMOnPropertyChanged;
                if(!String.IsNullOrEmpty(vm.Text))
                    webBrowser.NavigateToString(vm.Text);
            }
        }

        private void VMOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Text")
            {
                var vm = sender as XhtmlSinkViewModel;
                if (!String.IsNullOrEmpty(vm.Text))
                    webBrowser.NavigateToString(vm.Text);
            }
        }
    }
}
