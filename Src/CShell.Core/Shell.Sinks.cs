using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using CShell.Framework;
using CShell.Framework.Services;
using Caliburn.Micro;
using Execute = CShell.Framework.Services.Execute;

namespace CShell
{
    public static partial class Shell
    {
        private static Uri defaultSinkUri = new Uri("sink://cshell/xhtml/");
        /// <summary>
        /// Gets or sets the URI of the default sink.
        /// </summary>
        public static Uri DefaultSinkUri
        {
            get { return defaultSinkUri; }
            set { defaultSinkUri = value; }
        }

        /// <summary>
        /// Gets all available sinks.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ISink> GetSinks()
        {
            return UI.Documents.OfType<ISink>().ToArray();
        }

        /// <summary>
        /// Gets a specific sink based on the URI. 
        /// If the sink URI exists or can be created the sink is opened.
        /// </summary>
        /// <param name="uri">The sink URI.</param>
        public static ISink GetSink(Uri uri)
        {
            return GetSink(uri, false);
        }

        /// <summary>
        /// Gets a specific sink based on the uri.
        /// </summary>
        /// <param name="uri">The sink URI.</param>
        /// <param name="suppressOpen">If set to <c>true</c> sink will not be opened, but just created.</param>
        /// <returns></returns>
        public static ISink GetSink(Uri uri, bool suppressOpen)
        {
            if (uri == null) throw new ArgumentNullException("uri");
            var sinks = GetSinks();
            var sink = sinks.FirstOrDefault(s => s.Uri == uri);
            if (sink == null)
            {
                sink = IoC.GetAllInstances(typeof(ISinkProvider))
                    .Cast<ISinkProvider>()
                    .Where(provider => provider.Handles(uri))
                    .Select(provider => provider.Create(uri))
                    .FirstOrDefault(s => s != null);

                if (sink != null && !suppressOpen)
                    UI.ActivateDocument(sink);
            }
            return sink;
        }

        /// <summary>
        /// Gets the first sink with a certain title.
        /// </summary>
        /// <param name="sinkName">Name of the sink.</param>
        /// <returns>If the sink name cannot be found returns null.</returns>
        public static ISink GetSink(string sinkName)
        {
            if (sinkName == null) throw new ArgumentNullException("sinkName");

            var sinks = GetSinks();
            var sink = sinks.FirstOrDefault(s => s.DisplayName.Equals(sinkName));
            return sink;
        }

        /// <summary>
        /// Dumps an object to the specified sink.
        /// </summary>
        /// <param name="o">The object to dump.</param>
        /// <param name="sink">The sink URI. If no sink URI is specified the default sink is used.</param>
        public static void Dump(this object o, Uri sink = null)
        {
            if (sink == null)
                sink = DefaultSinkUri;
            var s = GetSink(sink);
            if (s == null)
                throw new NotSupportedException("The requested sink doesnt exist: " + sink + ", make sure the URI is spelled correctly and the module containing the specified sink is loaded.");

            try
            {
                s.Dump(o);
            }
            catch (Exception ex)
            {
                var log = LogManager.GetLog(s.GetType());
                log.Error(ex);
                throw;
            }
        }
    }
}
