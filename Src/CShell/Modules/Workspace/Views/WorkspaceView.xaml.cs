using System.Windows.Controls;

namespace CShell.Modules.Workspace.Views
{
	/// <summary>
    /// Interaction logic for WorkspaceView.xaml
	/// </summary>
	public partial class WorkspaceView : UserControl
	{
		public WorkspaceView()
		{
			InitializeComponent();
		}

        /// <summary>
        /// Hack to access the parents control DataContext, due to the ContextMenus separate DataContext.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ContextMenu_Opened(object sender, System.Windows.RoutedEventArgs e)
        {
            var contextMenu = sender as ContextMenu;
            if (contextMenu != null)
            {
                ////the first time set the tag to the TS VM.
                //if (contextMenu.Tag == null)
                //    contextMenu.Tag = contextMenu.DataContext;
                ////try selecting the item of the contect menu
                //var vm = DataContext as TimeSeriesExplorerViewModel;
                //var itemVM = contextMenu.Tag as ITimeSeriesDisplay;
                //if (vm != null && itemVM != null)
                //{
                //    vm.SelectedItem = itemVM;
                //}
                //contextMenu.DataContext = DataContext;
            }
        }
	}
}
