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
    //public sealed partial class Workspace
    //{
    //    public void Save(string path)
    //    {
    //        Save(path, false);
    //    }

    //    public void Save(string path, bool saveShellLayout)
    //    {
    //        var ws = new WorkspaceContainer();
    //        //add the assemblies that have a file name
    //        //foreach (var assemblyFile in this.Assemblies.Where(a=>a.FullPath != null))
    //        //{
    //        //    ws.AssemblyFileReferences.Add(assemblyFile.FilePath);
    //        //}
    //        ////add the assemblies that have no file name, also only the ones that are removable, the nonremovable assemblies are from CShell
    //        //foreach (var assemblyFile in this.Assemblies.Where(a => a.FullPath == null && a.Removable))
    //        //{
    //        //    ws.AssemblyNameReferences.Add(assemblyFile.AssemblyName.FullName);
    //        //}

    //        //save the file settings
    //        ws.RootFolder = RootFolder;
    //        ws.Filter = Filter;

    //        var serializer = new XmlSerializer(typeof(WorkspaceContainer));
    //        //using (var textWriter = new StreamWriter(path))
    //        var settings = new XmlWriterSettings();
    //        settings.Indent = true;
    //        //settings.OmitXmlDeclaration = true;
    //        using (var xmlWriter = XmlWriter.Create(path, settings))
    //        {
    //            xmlWriter.WriteStartDocument();
    //            xmlWriter.WriteStartElement("CShell");
    //            serializer.Serialize(xmlWriter, ws);

    //            if(saveShellLayout)
    //            {
    //                //todo: better way of getting the current shell
    //                var shell = IoC.Get<IShell>();
    //                if(shell != null)
    //                    shell.SaveLayout(xmlWriter);
    //            }

    //            xmlWriter.WriteEndElement();
    //        }
    //    }

    //    public bool Load(string path)
    //    {
    //        return Load(path, false);
    //    }

    //    public bool Load(string path, bool loadShellLayout)
    //    {
    //        if (!File.Exists(path))
    //            return false;
    //        var serializer = new XmlSerializer(typeof(WorkspaceContainer));
    //        WorkspaceContainer ws = null;

    //        var settings = new XmlReaderSettings();
    //        settings.IgnoreWhitespace = true;
    //        //using (var reader = new StreamReader(path))
    //        using (var xmlReader = XmlReader.Create(path, settings))
    //        {
    //            xmlReader.ReadStartElement();
    //            ws = (WorkspaceContainer) serializer.Deserialize(xmlReader);

    //            //foreach (var assemblyFile in ws.AssemblyFileReferences)
    //            //{
    //            //    this.Assemblies.Add(assemblyFile);
    //            //}
    //            //foreach (var assemblyName in ws.AssemblyNameReferences)
    //            //{
    //            //    this.Assemblies.Add(new AssemblyReference(new AssemblyName(assemblyName)));
    //            //}

    //            //restore file settings
    //            RootFolder = ws.RootFolder ?? "";
    //            Filter = ws.Filter ?? "";

    //            //var es = xmlReader.ReadElementContentAsString();
    //            if (loadShellLayout)
    //            {
    //                var shell = IoC.Get<IShell>();
    //                if (shell != null)
    //                    shell.LoadLayout(xmlReader);

    //            }
    //        }
    //        return true;
    //    }

    //    [Browsable(false)]
    //    [XmlRootAttribute("Workspace", IsNullable = false)]
    //    public class WorkspaceContainer
    //    {
    //        private Collection<string> assemblyFileReferences = new Collection<string>();
    //        [XmlArrayItem("File")]
    //        public Collection<string> AssemblyFileReferences
    //        {
    //            get { return assemblyFileReferences; }
    //        }

    //        private Collection<string> assemblyNameReferences = new Collection<string>();
    //        [XmlArrayItem("AssemblyName")]
    //        public Collection<string> AssemblyNameReferences
    //        {
    //            get { return assemblyNameReferences; }
    //        }

    //        public string RootFolder { get; set; }
    //        public string Filter { get; set; }
    //    }

       
    //}//end class Workspace

}
