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
using System.Windows.Input;
using CShell.Framework.Services;

namespace CShell.Framework
{
    public abstract class Sink : Document, ISink
    {
        protected Sink()
        {
            IsVisible = true;
        }

        public virtual void Dump(object o)
        {
            Dump(o, null);
        }

        public abstract void Dump(object o, string description);

        public abstract void Clear();

		public virtual Uri IconSource
		{
			get { return null; }
		}

		private bool _isVisible;
		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;
				NotifyOfPropertyChange(() => IsVisible);
			}
		}

        public virtual PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        /// <summary>
        /// Gets the display name from the uri, which is usually the second part of the absolute path.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="pathIndex">Zero based index. 0 gets the first part of the path, e.g. /mypah/hello -> "mypath" </param>
        /// <returns>The name of the path part if the path contains such a item, otherwise returns null.</returns>
        protected string GetUriPathPart(Uri uri, int pathIndex)
        {
            var pathParts = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathParts.Length > pathIndex)
                return pathParts[pathIndex];
            return null;
        }

        protected string GetTitle(Uri uri, string defaultTitle)
        {
            var title = GetUriPathPart(uri, 1);
            if (title != null)
                title = Uri.UnescapeDataString(title);
            if (String.IsNullOrEmpty(title))
                title = defaultTitle;
            return title;
        }

    }
}
