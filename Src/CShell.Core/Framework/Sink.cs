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

        public abstract void Dump(object o);

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
