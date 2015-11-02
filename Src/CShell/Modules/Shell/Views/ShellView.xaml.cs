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
