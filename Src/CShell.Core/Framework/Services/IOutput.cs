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
using System.Windows.Media;
using CShell.Framework;

namespace CShell.Framework.Services
{
	public interface IOutput
	{
        void Write(string text);
        void Write(string format, params object[] arg);
        void WriteLine();
        void WriteLine(string text);
        void WriteLine(string format, params object[] arg);

		void Clear();
        int BufferLength { get; set; }

        string Font { get; set; }
        double FontSize { get; set; }
        Color TextColor { get; set; }
        Color BackgroundColor { get; set; }
	}
}