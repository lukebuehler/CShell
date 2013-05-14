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
using System.Reflection;
using System.Text;
using CShell.Framework.Results;
using Microsoft.Win32;

namespace CShell.Modules.Workspace.Results
{
    public class AddReferencesResult : ResultBase
    {
        private readonly AssemblyReferences references;
        private readonly IEnumerable<string> files;
        private readonly IEnumerable<AssemblyName> assemblyNames;

        public AddReferencesResult(AssemblyReferences references, string file)
        {
            this.references = references;
            this.files = new [] {file};
        }

        public AddReferencesResult(AssemblyReferences references, IEnumerable<string> files)
        {
            this.references = references;
            this.files = files;
        }

        public AddReferencesResult(AssemblyReferences references, AssemblyName assemblyName)
        {
            this.references = references;
            this.assemblyNames = new []{assemblyName};
        }

        public AddReferencesResult(AssemblyReferences references, IEnumerable<AssemblyName> assemblyNames)
        {
            this.references = references;
            this.assemblyNames = assemblyNames;
        }


        public override void Execute(Caliburn.Micro.ActionExecutionContext context)
        {
            try
            {
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        references.Add(file);
                    }
                }
                if (assemblyNames != null)
                {
                    foreach (var assemblyName in assemblyNames)
                    {
                        references.Add(new AssemblyReference(assemblyName));
                    }
                }
                OnCompleted(null);
            }
            catch (Exception ex)
            {
                OnCompleted(ex);
            }
        }

    }//end class
}
