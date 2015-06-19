using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShell.Framework;

namespace CShell.Sinks.Xhtml
{
    public static class XhtmlSink
    {
        public const string XhtmlSinkUri = "sink://cshell/xhtml/";

        public static ISink GetXhtmlSink(string sinkName)
        {
            return GetXhtmlSink(sinkName, false);
        }

        public static ISink GetXhtmlSink(string sinkName, bool suppressOpen)
        {
            var uri = new Uri(XhtmlSinkUri + sinkName);
            return Shell.GetSink(uri, suppressOpen);
        }

        public static void DumpXhtml(this object o, string sinkName = null)
        {
            var sink = GetXhtmlSink(sinkName);
            sink.Dump(o);
        }
    }
}
