using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace CShell.Modules.Repl.Controls
{
    internal class ConsoleStream : Stream
    {
        private readonly TextType textType;
        readonly Action<string, TextType> callback;

        public ConsoleStream(TextType textType, Action<string, TextType> cb)
        {
            this.textType = textType;
            callback = cb;
        }

        public override bool CanRead { get { return false; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return true; } }


        public override long Length { get { return 0; } }
        public override long Position { get { return 0; } set { } }
        public override void Flush() { }
        public override int Read([In, Out] byte[] buffer, int offset, int count) { return -1; }

        public override long Seek(long offset, SeekOrigin origin) { return 0; }

        public override void SetLength(long value) { }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Execute.OnUIThread(() => callback(Encoding.UTF8.GetString(buffer, offset, count), textType));
        }
    }
}
