using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Caliburn.Micro;
using CShell.Framework.Services;
using CShell.Util;

namespace CShell
{
    [Export]
    public sealed partial class Workspace : PropertyChangedBase
    {
        private readonly IShell shell;
        private readonly IReplScriptExecutorFactory replExecutorFactory;

        private IReplScriptExecutor replExecutor;

        [ImportingConstructor]
        public Workspace(IShell shell, IReplScriptExecutorFactory replExecutorFactory)
        {
            this.shell = shell;
            this.replExecutorFactory = replExecutorFactory;
        }

        public IReplScriptExecutor ReplExecutor
        {
            get { return replExecutor; }
        }

        public string WorkspaceDirectory { get; private set; }


        public void SetWorkspaceDirectory(string dir)
        {
            if (!String.IsNullOrEmpty(WorkspaceDirectory))
            {
                //run teardown script
                //try to save the layout
                SaveLayout();
            }

            if(replExecutor != null)
                replExecutor.Reset();

            replExecutor = null;
            WorkspaceDirectory = dir;

            if (!String.IsNullOrEmpty(WorkspaceDirectory))
            {
                WorkspaceDirectory = Path.GetFullPath(WorkspaceDirectory);
                //create executor
                replExecutor = replExecutorFactory.Create(WorkspaceDirectory);
                //restore layout
                LoadLayout();

                //load libs

                //run startup scripts
            }

            NotifyOfPropertyChange(() => WorkspaceDirectory);
            NotifyOfPropertyChange(() => ReplExecutor);
        }

       

        #region Helpers

        public static void CreateEmptyFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            var templateFile = Path.Combine(exeDir, Constants.TemplatesPath, "Empty" + extension);
            var emptyFileText = "";
            if (File.Exists(templateFile))
                emptyFileText = File.ReadAllText(templateFile);
            File.WriteAllText(filePath, emptyFileText);
        }
        #endregion

    }//end class
}
