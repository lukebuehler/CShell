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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using CShell.Framework.Services;
using Caliburn.Micro;

namespace CShell
{
    public sealed partial class Workspace
    {
        private void SaveLayout()
        {
            var path = Path.Combine(WorkspaceDirectory, Constants.LayoutFile);
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            var windowLocation = shell.GetWindowLocation();

            var serializer = new XmlSerializer(typeof(WindowLocation));
            using (var xmlWriter = XmlWriter.Create(path, settings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("CShellLayout");
                serializer.Serialize(xmlWriter, windowLocation);
                shell.SaveLayout(xmlWriter);
                xmlWriter.WriteEndElement();
            }
        }

        private void LoadLayout()
        {
            var path = Path.Combine(WorkspaceDirectory, Constants.LayoutFile);
            if(!File.Exists(path))
                return;

            var settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            var serializer = new XmlSerializer(typeof(WindowLocation));
            using (var xmlReader = XmlReader.Create(path, settings))
            {
                xmlReader.ReadStartElement();
                var windowLocation = (WindowLocation)serializer.Deserialize(xmlReader);
                shell.RestoreWindowLocation(windowLocation);

                shell.LoadLayout(xmlReader);
            }
        }

        [Browsable(false)]
        [XmlRootAttribute("WindowLocation", IsNullable = false)]
        public class WindowLocation
        {
            public double Top { get; set; }
            public double Left { get; set; }
            public double Height { get; set; }
            public double Width { get; set; }
            public string State { get; set; }
            public int Monitor { get; set; }
        }

    }//end class Workspace

}
