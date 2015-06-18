using System.Web.UI;

namespace CShell.Sinks.Xhtml.XhtmlDumper
{
    public interface IXhtmlRenderer
    {
        bool Render(object o, string description, int depth, XhtmlTextWriter writer);
    }
}
