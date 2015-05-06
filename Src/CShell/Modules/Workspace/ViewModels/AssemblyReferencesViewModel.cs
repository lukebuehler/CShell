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
using ScriptCs.Contracts;

namespace CShell.Modules.Workspace.ViewModels
{
    public class AssemblyReferencesViewModel : TreeViewModel
    {
        private readonly IReplScriptExecutor replExecutor;

        public AssemblyReferencesViewModel(IReplScriptExecutor replExecutor)
        {
            this.replExecutor = replExecutor;
            DisplayName = "Loaded References";

            replExecutor.AssemblyReferencesChanged += ReplExecutorOnAssemblyReferencesChanged;
            Reload();
        }

        private void Reload()
        {
            Children.Clear();
            var refs = new List<AssemblyReferenceViewModel>();
            refs.AddRange(replExecutor.GetReferencesAsPaths().Select(path=>new AssemblyReferenceViewModel(path, replExecutor)));
            refs = refs.OrderBy(refVm => refVm.DisplayName).ToList();
            Children.AddRange(refs);
        }

        private void ReplExecutorOnAssemblyReferencesChanged(object sender, EventArgs eventArgs)
        {
            Reload();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                replExecutor.AssemblyReferencesChanged -= ReplExecutorOnAssemblyReferencesChanged;
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

        public IEnumerable<IResult> AddFileReferences()
        {
            return Module.AddFileReferences();
        }

        public IEnumerable<IResult> AddGacReferences()
        {
            return Module.AddGacReferences();
        }

        public IEnumerable<IResult> CopyReferences()
        {
            return Module.CopyReferences();
        }

        public IEnumerable<IResult> MangePackages()
        {
            return Module.MangePackages();
        }

    }//end class
}
