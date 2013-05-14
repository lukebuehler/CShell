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
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CShell.Framework.Services
{
	[Export(typeof(IResourceManager))]
	public class ResourceManager : IResourceManager
	{
		public Stream GetStream(string relativeUri, string assemblyName)
		{
			try
			{
				var resource = Application.GetResourceStream(new Uri(assemblyName + ";component/" + relativeUri, UriKind.Relative))
					?? Application.GetResourceStream(new Uri(relativeUri, UriKind.Relative));

				return (resource != null)
					? resource.Stream
					: null;
			}
			catch
			{
				return null;
			}
		}

		public BitmapImage GetBitmap(string relativeUri, string assemblyName)
		{
			var s = GetStream(relativeUri, assemblyName);
			if (s == null) return null;

			using (s)
			{
				var bmp = new BitmapImage();
				bmp.BeginInit();
				bmp.StreamSource = s;
				bmp.EndInit();
				bmp.Freeze();
				return bmp;
			}
		}

		public BitmapImage GetBitmap(string relativeUri)
		{
			return GetBitmap(relativeUri, ExtensionMethods.GetExecutingAssemblyName());
		}
	}
}