using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CShell.Framework;

namespace CShell.Sinks.Grid
{
    public static class GridSink
    {
        public const string GridSinkUri = "sink://cshell/grid/";

        public static ISink GetGridSink(string sinkName)
        {
            return GetGridSink(sinkName, false);
        }

        public static ISink GetGridSink(string sinkName, bool suppressOpen)
        {
            var uri = new Uri(GridSinkUri + sinkName);
            return Shell.GetSink(uri, suppressOpen);
        }

        public static void DumpGrid(this object o, string sinkName = null)
        {
            var sink = GetGridSink(sinkName);
            sink.Dump(o);
        }
    }
}
