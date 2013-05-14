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
using Caliburn.Micro;

namespace CShell.Framework.Menus
{
	public class MenuItem : StandardMenuItem
	{
		private readonly Func<IEnumerable<IResult>> _execute;

		#region Constructors

		public MenuItem(string text)
			: base(text)
		{
			
		}

		public MenuItem(string text, Func<IEnumerable<IResult>> execute)
			: base(text)
		{
			_execute = execute;
		}

		public MenuItem(string text, Func<IEnumerable<IResult>> execute, Func<bool> canExecute)
			: base(text, canExecute)
		{
			_execute = execute;
		}

		#endregion

		public IEnumerable<IResult> Execute()
		{
			return _execute != null ? _execute() : new IResult[] { };
		}
	}
}