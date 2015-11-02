using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caliburn.Micro;
using CShell.Framework.Services;

namespace CShell.Framework.Results
{
    public class ChangeWorkspaceResult : ResultBase
    {
        private string workspaceDirectory;

        [Import]
        private Workspace workspace;

        /// <summary>
        /// Creates a result that will open a workspace.
        /// </summary>
        public ChangeWorkspaceResult(string workspaceDirectory)
        {
            this.workspaceDirectory = workspaceDirectory;
        }

        public override void Execute(CoroutineExecutionContext context)
        {
            
            try
            {
                //some of this is synchronous which can mess up the UI (especially on startup), so we execute it on a seperate thread
                Task.Factory.StartNew(() => workspace.SetWorkspaceDirectory(workspaceDirectory))
                    .ContinueWith(t2 => OnCompleted(t2.Exception));
            }
            catch (Exception ex)
            {
                OnCompleted(ex);
            }
        }
    }
}
