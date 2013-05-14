#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2013  Arnova Asset Management Ltd., Lukas Buhler
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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using CShell.Framework.Results;
using CShell.Modules.Workspace.Results;
using Caliburn.Micro;
using Microsoft.Win32;

namespace CShell.Modules.Workspace.ViewModels
{
    public class AssemblyReferencesViewModel : TreeViewModel
    {
        private readonly AssemblyReferences assemblyReferences;

        public AssemblyReferencesViewModel(AssemblyReferences assemblyReferences)
        {
            this.assemblyReferences = assemblyReferences;
            DisplayName = "References";

            assemblyReferences.CollectionChanged += AssemblyReferencesOnCollectionChanged;
            Reload();
        }

        private void Reload()
        {
            Children.Clear();
            foreach (var assembly in assemblyReferences.OrderBy(a=>a.AssemblyName.Name))
            {
                var assemblyVM = new AssemblyReferenceViewModel(assembly, assemblyReferences);
                Children.Add(assemblyVM);
            }
        }

        private void AssemblyReferencesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            Reload();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                assemblyReferences.CollectionChanged -= AssemblyReferencesOnCollectionChanged;
            }
            base.Dispose(disposing);
        }

        public override Uri IconSource
        {
            get
            {
                if(IsExpanded)
                    return new Uri("pack://application:,,,/CShell;component/Resources/Icons/ReferenceFolder.Open.png");
                else
                    return new Uri("pack://application:,,,/CShell;component/Resources/Icons/ReferenceFolder.Closed.png");
            }
        }

        public IEnumerable<IResult> AddReferenceFromFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = CShell.Constants.AssemblyFileFilter;
            dialog.Multiselect = true;
            yield return Show.Dialog(dialog);
            yield return new AddReferencesResult(assemblyReferences, dialog.FileNames);
        }

        public IEnumerable<IResult> AddReferenceFromGac()
        {
            var windowSettings = new Dictionary<string, object> {{ "SizeToContent", SizeToContent.Manual }, { "Width", 500.0 }, { "Height", 500.0 }  };
            var dialog = new AssemblyGacViewModel();
            yield return Show.Dialog(dialog, windowSettings);
            var selectedAssemblies = dialog.SelectedAssemblies.Select(item => item.AssemblyName).ToArray();
            if(selectedAssemblies.Length <= dialog.MaxSelectedAssemblyCount)
                yield return new AddReferencesResult(assemblyReferences, dialog.SelectedAssemblies.Select(item=>item.AssemblyName));
        }

    }//end class
}
