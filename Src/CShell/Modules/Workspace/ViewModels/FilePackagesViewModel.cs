using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using CShell.Framework.Results;

namespace CShell.Modules.Workspace.ViewModels
{
    public class FilePackagesViewModel : FileViewModel
    {
        public FilePackagesViewModel(string filePath) : base(filePath)
        {
        }

        public FilePackagesViewModel(FileInfo info) : base(info)
        {
        }

        public IEnumerable<IResult> MangePackages()
        {
            var windowSettings = new Dictionary<string, object> { { "SizeToContent", SizeToContent.Manual }, { "Width", 500.0 }, { "Height", 500.0 } };
            var dialog = new AssemblyPackagesViewModel();
            yield return Show.Dialog(dialog, windowSettings);
        }
    }
}
