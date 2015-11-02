using CShell.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CShell.Modules.Workspace.ViewModels
{
    public class AssemblyReferenceViewModel : TreeViewModel
    {
        private readonly string filePath;
        private readonly IReplScriptExecutor replExecutor;
        private readonly Assembly assembly;

        public AssemblyReferenceViewModel(string filePath, IReplScriptExecutor replExecutor)
        {
            if (filePath.EndsWith(".dll") || filePath.EndsWith(".exe"))
            {
                this.filePath = filePath; //PathHelper.ToRelativePath(replExecutor.WorkspaceDirectory, filePath);
                assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(filePath));
                FullPath = PathHelper.ToAbsolutePath(Environment.CurrentDirectory, this.filePath);
                Available = File.Exists(FullPath);
            }
            else
            {
                this.filePath = filePath;
                assemblyName = new AssemblyName(filePath);
                Available = true;
            }

            this.replExecutor = replExecutor;
        }

        public AssemblyReferenceViewModel(Assembly assembly, IReplScriptExecutor replExecutor)
        {
            this.assembly = assembly;
            assemblyName = assembly.GetName();
            Available = true;
            this.replExecutor = replExecutor;
        }

        public override string DisplayName
        {
            get { return assemblyName.Name; }
            set
            { }
        }

        public string FilePath
        {
            get { return filePath; }
        }

        public string FullPath { get; private set; }

        public bool Available { get; private set; }

        private bool removable = true;
        public bool Removable
        {
            get { return removable; }
            set { removable = value; }
        }

        private AssemblyName assemblyName;
        public AssemblyName AssemblyName
        {
            get { return assemblyName; }
        }

        public override Uri IconSource
        {
            get
            {
                //if(!assemblyReference.HasParts)
                    return new Uri("pack://application:,,,/CShell;component/Resources/Icons/Icons.16x16.Reference.png");
                //else
                //    return new Uri("pack://application:,,,/CShell;component/Resources/Icons/Icons.16x16.ReferenceModule.png");
            }
        }

      

        public string ToolTip
        {
            get { return filePath ; }
        }

        public void Remove()
        {
            if(assembly != null)
                replExecutor.RemoveReferences(assembly);
            else
                replExecutor.RemoveReferences(filePath);
        }
    }
}
