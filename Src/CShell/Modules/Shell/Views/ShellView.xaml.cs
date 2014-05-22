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
using System.Windows;
using Caliburn.Micro;
using Xceed.Wpf.AvalonDock;

namespace CShell.Modules.Shell.Views
{
	/// <summary>
	/// Interaction logic for ShellView.xaml
	/// </summary>
	public partial class ShellView : Window, IShellView
	{
		public ShellView()
		{
		        InitializeComponent();
		}

        public DockingManager DockingManager
        {
            get { return this.Manager; }
        }

        #region Window Location

	    public CShell.Workspace.WindowLocation GetWindowLocation()
	    {
	        var location = new CShell.Workspace.WindowLocation();
	        Execute.OnUIThread(() =>
	        {
	            location.Top = Top;
	            location.Left = Left;
	            location.Height = Height;
	            location.Width = Width;
	            location.State = WindowState.ToString();
	        });
	        return location;
	    }

	    public void RestoreWindowLocation(CShell.Workspace.WindowLocation location)
        {
            Execute.OnUIThread(() =>
            {
                Top = location.Top;
                Left = location.Left;
                Height = location.Height;
                Width = location.Width;
                WindowState windowState;
                if (WindowState.TryParse(location.State, out windowState))
                {
                    WindowState = windowState;
                }

                //TODO: this is not working yet with multiple screens
                //Size it to fit the current screen
                //SizeToFit();
                //Move the window at least partially into view
                //MoveIntoView();
            });
        }

        public void SizeToFit()
        {
            if (Height > System.Windows.SystemParameters.VirtualScreenHeight)
            {
                Height = System.Windows.SystemParameters.VirtualScreenHeight;
            }

            if (Width > System.Windows.SystemParameters.VirtualScreenWidth)
            {
                Width = System.Windows.SystemParameters.VirtualScreenWidth;
            }
        }

        public void MoveIntoView()
        {
            if (Top + Height / 2 > System.Windows.SystemParameters.VirtualScreenHeight)
            {
                Top = System.Windows.SystemParameters.VirtualScreenHeight - Height;
            }

            if (Left + Width / 2 > System.Windows.SystemParameters.VirtualScreenWidth)
            {
                Left = System.Windows.SystemParameters.VirtualScreenWidth - Width;
            }

            if (Top < 0)
            {
                Top = 0;
            }

            if (Left < 0)
            {
                Left = 0;
            }
        }

        #endregion

    }
}
