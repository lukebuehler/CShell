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
using System.Linq;
using System.Text;

namespace CShell.Modules.Workspace.ViewModels
{
    public class AssemblyReferenceViewModel : TreeViewModel
    {
        private readonly AssemblyReference assemblyReference;
        private readonly AssemblyReferences references;

        public AssemblyReferenceViewModel(AssemblyReference assemblyReference, AssemblyReferences references)
        {
            this.assemblyReference = assemblyReference;
            this.references = references;
        }

        public override string DisplayName
        {
            get { return assemblyReference.AssemblyName.Name; }
            set
            { }
        }

        public override Uri IconSource
        {
            get
            {
                if(!assemblyReference.HasParts)
                    return new Uri("pack://application:,,,/CShell;component/Resources/Icons/Icons.16x16.Reference.png");
                else
                    return new Uri("pack://application:,,,/CShell;component/Resources/Icons/Icons.16x16.ReferenceModule.png");
            }
        }

        public bool Available
        {
            get { return assemblyReference.Available; }
        }

        public string ToolTip
        {
            get { return assemblyReference.FilePath; }
        }

        public void Remove()
        {
            if (references != null && assemblyReference.Removable)
                references.Remove(assemblyReference);
        }
    }
}
