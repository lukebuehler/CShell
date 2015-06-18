using System;
using System.IO;
using System.Linq;
using System.Web.UI;
using CShell.Sinks.Xhtml.Properties;

namespace CShell.Sinks.Xhtml.XhtmlDumper
{
    public class XhtmlDumper : IDisposable
    {
        private readonly XhtmlTextWriter writer;
        private readonly IXhtmlRenderer[] renderers;

        public XhtmlDumper(TextWriter writer)
        {
            this.writer = new XhtmlTextWriter(writer); ;
            this.renderers = new IXhtmlRenderer[] { new ObjectXhtmlRenderer(), new BasicXhtmlRenderer() };
            InitHeader();
        }

        public XhtmlDumper(TextWriter writer, params IXhtmlRenderer[] renderers)
        {
            this.writer = new XhtmlTextWriter(writer);
            this.renderers = renderers;
            InitHeader();
        }

        private void InitHeader()
        {
            writer.WriteLineNoTabs("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Strict//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd\">");
            writer.AddAttribute("xmlns", "http://www.w3.org/1999/xhtml");
            writer.AddAttribute("xml:lang", "en");
            writer.RenderBeginTag(HtmlTextWriterTag.Html);
            writer.RenderBeginTag(HtmlTextWriterTag.Head);

            writer.RenderBeginTag(HtmlTextWriterTag.Title);
            writer.Write("XhtmlDumper");
            writer.RenderEndTag();


            writer.AddAttribute("http-equiv", "content-type");
            writer.AddAttribute(HtmlTextWriterAttribute.Content, "text/html;charset=utf-8");
            writer.RenderBeginTag(HtmlTextWriterTag.Meta);
            writer.RenderEndTag();
            writer.WriteLine();

            writer.AddAttribute(HtmlTextWriterAttribute.Name, "generator");
            writer.AddAttribute(HtmlTextWriterAttribute.Content, "XhtmlDumper");
            writer.RenderBeginTag(HtmlTextWriterTag.Meta);
            writer.RenderEndTag();
            writer.WriteLine();

            writer.AddAttribute(HtmlTextWriterAttribute.Name, "description");
            writer.AddAttribute(HtmlTextWriterAttribute.Content, "Generated on: " + DateTime.Now);
            writer.RenderBeginTag(HtmlTextWriterTag.Meta);
            writer.RenderEndTag();
            writer.WriteLine();

            writer.AddAttribute("type", "text/css");
            writer.RenderBeginTag(HtmlTextWriterTag.Style);
            writer.WriteLineNoTabs(Resources.StyleSheet);
            writer.RenderEndTag(); // style

            writer.RenderEndTag(); // Head
            writer.WriteLine();

            writer.RenderBeginTag(HtmlTextWriterTag.Body);
        }


        public void WriteObject(object o, string description, int depth)
        {
            //try to loop through all renderes to see if one will render the object,
            // otherwise fallback on textwriter
            if(!renderers.Any(xhtmlRenderer => xhtmlRenderer.Render(o, description, depth, writer)))
                writer.Write(o);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                writer.RenderEndTag(); // body
                writer.RenderEndTag(); // html
                foreach (var xhtmlRenderer in renderers)
                {
                    var disposableXhtmlRenderer = xhtmlRenderer as IDisposable;
                    if (disposableXhtmlRenderer != null)
                        disposableXhtmlRenderer.Dispose();
                }
            }
        }

    }
}
