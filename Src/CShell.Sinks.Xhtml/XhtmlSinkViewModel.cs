using System;
using System.IO;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using CShell.Framework.Services;
using CShell.Sinks.Xhtml.XhtmlDumper;

namespace CShell.Sinks.Xhtml
{
    public class XhtmlSinkViewModel : Framework.Sink
    {
        private StringBuilder stringBuilder;
        private StringWriter stringWriter;
        private XhtmlDumper.XhtmlDumper xhtmlDumper;

        private TextWriter linqPadWriter;
        
        public XhtmlSinkViewModel(Uri uri)
        {
            Uri = uri;
            DisplayName = GetTitle(uri, "Dump");
        }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        private string text = "";
        public string Text
        {
            get { return text; }
        }

        public override void Dump(object o)
        {
            if (xhtmlDumper == null)
            {
                stringBuilder = new StringBuilder();
                stringWriter = new StringWriter(stringBuilder);
                var renderers = IoC.GetAllInstances(typeof(IXhtmlRenderer)).Cast<IXhtmlRenderer>().ToList();
                //add the basic rederer at the end
                renderers.Add(new BasicXhtmlRenderer());
                xhtmlDumper = new XhtmlDumper.XhtmlDumper(stringWriter);
            }

            xhtmlDumper.WriteObject(o, null, 3);
            text = stringBuilder.ToString();
            //append the closing HTML closing tags to the string
            text += Environment.NewLine + "</body></html>";

            NotifyOfPropertyChange(()=>Text);
        }

        public override void Clear()
        {
            if(linqPadWriter != null)
                linqPadWriter.Dispose();
            linqPadWriter = null;

            text = String.Empty;
            NotifyOfPropertyChange(() => Text);
        }

        protected override void OnDeactivate(bool close)
        {
            Clear();
            base.OnDeactivate(close);
        }

        public override bool Equals(object obj)
        {
            var other = obj as XhtmlSinkViewModel;
            return other != null && Uri == other.Uri;
        }

    }
}
