using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using CShell.Framework.Results;
using Microsoft.Win32;

namespace CShell.Modules.Workspace.ViewModels
{
    public class FolderPackagesViewModel : FolderViewModel
    {
        protected FolderPackagesViewModel(string path, CShell.Workspace workspace) : base(path, workspace)
        {
        }

        public FolderPackagesViewModel(DirectoryInfo info, CShell.Workspace workspace)
            : base(info, workspace)
        {
        }

        public override Uri IconSource
        {
            get
            {
                if (IsExpanded)
                    return new Uri("pack://application:,,,/CShell;component/Resources/Icons/ReferenceFolder.Open.png");
                else
                    return new Uri("pack://application:,,,/CShell;component/Resources/Icons/ReferenceFolder.Closed.png");
            }
        }

        public IEnumerable<IResult> MangePackages()
        {
            var windowSettings = new Dictionary<string, object> { { "SizeToContent", SizeToContent.Manual }, { "Width", 500.0 }, { "Height", 500.0 } };
            var dialog = new AssemblyPackagesViewModel();
            yield return Show.Dialog(dialog, windowSettings);
        }
    }
}
