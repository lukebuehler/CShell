using System.Windows.Media;
using CShell.Framework;

namespace CShell.Framework.Services
{
	public interface IOutput
	{
        void Write(string text);
        void Write(string format, params object[] arg);
        void WriteLine();
        void WriteLine(string text);
        void WriteLine(string format, params object[] arg);

		void Clear();
        int BufferLength { get; set; }

        string Font { get; set; }
        double FontSize { get; set; }
        Color TextColor { get; set; }
        Color BackgroundColor { get; set; }
	}
}