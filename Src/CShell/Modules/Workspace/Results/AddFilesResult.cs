#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2012  Arnova Asset Management Ltd., Lukas Buhler
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
using System.IO;
using System.Linq;
using System.Text;
using CShell.Framework.Results;
using Microsoft.Win32;

namespace CShell.Modules.Workspace.Results
{
    public class AddFilesResult : ResultBase
    {
        private readonly IEnumerable<string> files;

        public AddFilesResult(string file)
        {
            this.files = new string[] {file};
        }

        public AddFilesResult(IEnumerable<string> files)
        {
            this.files = files;
        }

        public override void Execute(Caliburn.Micro.CoroutineExecutionContext context)
        {
            foreach (var file in files)
            {
                if(!File.Exists(file))
                {
                    CShell.Workspace.CreateEmptyFile(file);
                }
            }
            OnCompleted(null);
        }
    }
}
