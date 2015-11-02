using System.Windows.Controls;
using CShell.Framework.Services;
using CShell.Modules.Repl.Controls;

namespace CShell.Modules.Repl.Views
{
    /// <summary>
    /// Interaction logic for ReplView.xaml
    /// </summary>
    public partial class ReplView : UserControl, IReplView
    {
        public ReplView()
        {
            InitializeComponent();
        }

        public IReplOutput GetReplOutput()
        {
            return repl;
        }
    }
}
