using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caliburn.Micro;
using Application = System.Windows.Application;

namespace CShell.Framework.Results
{
    public class CloseShellResult : ResultBase
    {

        /// <summary>
        /// Closes the current workspace.
        /// </summary>
        public CloseShellResult()
        {}

        public override void Execute(CoroutineExecutionContext context)
        {
            try
            {
                Caliburn.Micro.Execute.OnUIThread(() => Application.Current.MainWindow.Close());
            }
            catch (Exception ex)
            {
                OnCompleted(ex);
            }
        }
    }
}
