#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2012  Arnova Asset Management Ltd., Lukas Buhler
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
