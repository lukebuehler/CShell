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
