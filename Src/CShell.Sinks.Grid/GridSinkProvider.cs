using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShell.Framework.Services;

namespace CShell.Sinks.Grid
{
    [Export(typeof(ISinkProvider))]
    public class GridSinkProvider : ISinkProvider
    {
        public bool Handles(Uri uri)
        {
            return uri.Scheme == "sink" && uri.Host == "cshell";
        }

        /// <summary>
        /// Creates a CShell sink.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>If the URI was correct a sink, otherwise null.</returns>
        public Framework.ISink Create(Uri uri)
        {
            var pathParts = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathParts.Length > 0)
            {
                //the first part of the uri is the sink type
                var sinkType = pathParts[0].ToLower();

                if (sinkType == "grid")
                {
                    return new GridSinkViewModel(uri);
                }

            }
            return null;
        }
    }
}
