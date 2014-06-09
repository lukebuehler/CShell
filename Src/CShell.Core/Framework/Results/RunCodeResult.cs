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
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace CShell.Framework.Results
{
    public class RunCodeResult : ResultBase
    {
        [Import]
        public Workspace Workspace { get; set; }

        private readonly string code;
        private readonly string sourceFile;

        public RunCodeResult(string code)
        {
            this.code = code;
        }

        public RunCodeResult(string code, string sourceFile)
        {
            this.code = code;
            this.sourceFile = sourceFile;
        }

        public override void Execute(CoroutineExecutionContext context)
        {
            if(Workspace != null && Workspace.ReplExecutor != null)
            {
                Workspace.ReplExecutor.Execute(code, sourceFile);
                OnCompleted(null);
            }
        }
    }
}
