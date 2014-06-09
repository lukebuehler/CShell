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
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CShell.Framework;
using CShell.Framework.Results;
using CShell.Framework.Services;
using Microsoft.Win32;

namespace CShell.Modules.Workspace.Results
{
    public class CopyReferencesResult : ResultBase
    {
        private readonly IEnumerable<string> assemblyPaths;

        [Import]
        public IShell Shell { get; set; }

        [Import]
        public CShell.Workspace Workspace { get; set; }

        public CopyReferencesResult(string file)
        {
            this.assemblyPaths = new[] { file };
        }

        public CopyReferencesResult(IEnumerable<string> files)
        {
            this.assemblyPaths = files;
        }

        public override void Execute(Caliburn.Micro.CoroutineExecutionContext context)
        {
            try
            {
                var binDir = Path.Combine(Workspace.WorkspaceDirectory, Constants.BinFolder);
                if (!Directory.Exists(binDir))
                    Directory.CreateDirectory(binDir);
                var copiedReferences = new List<string>();
                foreach (var assemblyPath in assemblyPaths)
                {
                    if (File.Exists(assemblyPath))
                    {
                        var destFileName = Path.Combine(binDir, Path.GetFileName(assemblyPath));
                        File.Copy(assemblyPath, destFileName, true);
                        copiedReferences.Add(destFileName);
                    }
                }

                //make sure they are available for code completion
                Workspace.ReplExecutor.AddReferencesAndNotify(copiedReferences.ToArray());

                OnCompleted(null);
            }
            catch (Exception ex)
            {
                OnCompleted(ex);
            }
        }

      

    }//end class
}
