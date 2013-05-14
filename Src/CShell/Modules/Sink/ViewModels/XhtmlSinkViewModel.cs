#region License
// CShell, A Simple C# Scripting IDE
// Copyright (C) 2013  Arnova Asset Management Ltd., Lukas Buhler
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CShell.Framework.Services;
using CShell.Sinks;
using Caliburn.Micro;
using XhtmlDumper;

namespace CShell.Modules.Sink.ViewModels
{
    public class XhtmlSinkViewModel : Framework.Sink, IXhtmlSink
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

        public override void Dump(object o, string description)
        {
            Dump(o, description, 3);
        }

        public void Dump(object o, string description, int depth)
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

            xhtmlDumper.WriteObject(o, description, depth);
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
