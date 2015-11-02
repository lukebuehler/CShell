using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.Primitives;

namespace CShell.Sinks.Grid
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class GridSinkView : UserControl
    {
        private Style rightAlignStyle;
        private GridSinkViewModel gridSink;
        private Type itemType;

        public GridSinkView()
        {
            InitializeComponent();

            rightAlignStyle = new Style();
            rightAlignStyle.Setters.Add(new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Right));
        }

        /// <summary>
        /// Generates the colums for the data grid depending on the time series data.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Data_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (gridSink != null)
                gridSink.PropertyChanged -= vm_PropertyChanged;

            gridSink = this.DataContext as GridSinkViewModel;
            if(gridSink != null)
            {
                gridSink.PropertyChanged += vm_PropertyChanged;
                if(gridSink.Data != null)
                    InitializeColumns();
            }
        }

        void vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Data")
            {
                if(itemType == null || itemType != gridSink.ItemType)
                    InitializeColumns();
            }
        }

        private void CheckComboBox_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            var propertyInfo = e.Item as PropertyInfo;
            if(e.IsSelected)
                AddColumn(propertyInfo);
            else
                RemoveColumn(propertyInfo);
            e.Handled = true;
        }

        private void InitializeColumns()
        {
            Data.Columns.Clear();
            if (gridSink.ItemType != null && gridSink.SelectedProperties != null)
            {
                itemType = gridSink.ItemType;
                foreach (var property in gridSink.SelectedProperties)
                    AddColumn(property);
            }
        }

        public void AddColumn(PropertyInfo propertyInfo)
        {
            var binding = new Binding(string.Format("{0}", propertyInfo.Name));
            binding.Mode = BindingMode.OneTime;

            var column = new DataGridTextColumn
            {
                Header = propertyInfo.Name,
                Binding = binding,
                IsReadOnly = true
            };

            if (propertyInfo.PropertyType == typeof(int) ||
                propertyInfo.PropertyType == typeof(uint) ||
                propertyInfo.PropertyType == typeof(double) ||
                propertyInfo.PropertyType == typeof(decimal) ||
                propertyInfo.PropertyType == typeof(float))
            {
                column.Binding.StringFormat = "{0:0.00}";
                //if it's a percent property use % formatting.
                if (propertyInfo.Name.ToLower().Contains("percent") || propertyInfo.Name.ToLower().Contains("prc"))
                    column.Binding.StringFormat = "{0:0.00%}";
                column.ElementStyle = rightAlignStyle;
            }

            if (propertyInfo.PropertyType == typeof(int) ||
                propertyInfo.PropertyType == typeof(uint))
            {
                column.Binding.StringFormat = "{0:0}";
            }

            var columnIndex = GetColumnIndex(propertyInfo);
            Data.Columns.Insert(columnIndex, column);
        }

        private int GetColumnIndex(PropertyInfo propertyInfo)
        {
            return gridSink.Properties
                .TakeWhile(itemProperty => propertyInfo != itemProperty)
                .Count(itemProperty => gridSink.SelectedProperties.Contains(itemProperty));
        }

        private void RemoveColumn(PropertyInfo propertyInfo)
        {
            var columnToRemove = Data.Columns.FirstOrDefault(c => (string)c.Header == propertyInfo.Name);
            if(columnToRemove != null)
                Data.Columns.Remove(columnToRemove);
        }

       
    }//end class
}
