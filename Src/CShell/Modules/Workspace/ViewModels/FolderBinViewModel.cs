using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using CShell.Framework.Results;
using CShell.Modules.Workspace.Results;
using Microsoft.Win32;

namespace CShell.Modules.Workspace.ViewModels
{
    public class FolderBinViewModel : FolderViewModel
    {
        protected FolderBinViewModel(string path, CShell.Workspace workspace) : base(path, workspace)
        {
        }

        public FolderBinViewModel(DirectoryInfo info, CShell.Workspace workspace) : base(info, workspace)
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

        public IEnumerable<IResult> CopyReferences()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = CShell.Constants.AssemblyFileFilter;
            dialog.Multiselect = true;
            yield return Show.Dialog(dialog);
            if (dialog.FileNames != null && dialog.FileNames.Length > 0)
            {
                yield return new CopyReferencesResult(dialog.FileNames);
            }
        }
    }
}
