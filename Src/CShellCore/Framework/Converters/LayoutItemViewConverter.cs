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
using System.Globalization;
using System.Windows.Data;
using Caliburn.Micro;

namespace CShell.Framework.Converters
{
	public class LayoutItemViewConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var view = ViewLocator.LocateForModel(value, null, null);
			Bind.SetModel(view, value);
			return view;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
		    return null;
		    //throw new NotImplementedException();
		}
	}
}