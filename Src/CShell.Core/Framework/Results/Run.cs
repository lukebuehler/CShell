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
using System.Linq;
using System.Text;

namespace CShell.Framework.Results
{
    public static class Run
    {
        public static RunCodeResult Code(string code, string sourceFile)
        {
            return new RunCodeResult(code, sourceFile);
        }

        public static RunCodeResult Code(string code)
        {
            return new RunCodeResult(code);
        }

        public static RunCodeResult File(string file)
        {
            var fileText =System.IO.File.ReadAllText(file);
            return new RunCodeResult(fileText, file);
        }
    }
}
