#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2012  Arnova Asset Management Ltd., Lukas Buhler
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CShell.Util
{
    public enum TextType
    {
        Output,
        Warning,
        Error,
        Repl,
        None,
    }

    internal class GuiStream : Stream
    {
        private readonly TextType textType;
        readonly Action<string, TextType> callback;

        public GuiStream(TextType textType, Action<string, TextType> cb)
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
            callback(Encoding.UTF8.GetString(buffer, offset, count), textType);
        }
    }
}
