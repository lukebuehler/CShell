//#region License
//// CShell, A Simple C# Scripting IDE
//// Copyright (C) 2013  Arnova Asset Management Ltd., Lukas Buhler
//// 
//// This program is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
//// 
//// This program is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//#endregion
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel.Composition.Hosting;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using CShell.Util;

//namespace CShell
//{
//    public class AssemblyReference
//    {
//        private readonly string filePath;

//        public AssemblyReference(string filePath)
//        {
//            //make sure the path is only relative
//            this.filePath = PathHelper.ToRelativePath(Environment.CurrentDirectory, filePath);
//            assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(filePath));
//            FullPath = PathHelper.ToAbsolutePath(Environment.CurrentDirectory, this.filePath);
//            Available = File.Exists(FullPath);
//        }

//        public AssemblyReference(Assembly assembly)
//        {
//            this.assembly = assembly;
//            assemblyName = assembly.GetName();
//            Available = true;
//        }

//        public AssemblyReference(AssemblyName assemblyName)
//        {
//            this.assemblyName = assemblyName;
//            Available = true;
//        }

//        public string FilePath
//        {
//            get { return filePath; }
//        }

//        public string FullPath { get; private set; }

//        public bool Available { get; private set; }

//        private bool removable = true;
//        public bool Removable
//        {
//            get { return removable; }
//            set { removable = value; }
//        }

//        private AssemblyName assemblyName;
//        public AssemblyName AssemblyName
//        {
//            get { return assemblyName; }
//        }

//        private Assembly assembly;
//        public Assembly Assembly
//        {
//            get
//            {
//                if(assembly == null && Available)
//                {
//                    try
//                    {
//                        //LoadFile, doesn't load the context
//                        assembly = AssemblyLoader.LoadAssembly(this);
//                        assemblyName = assembly.GetName();
//                    }
//                    catch (Exception)
//                    {
//                        Available = false;
//                        throw;
//                    }

//                }
//                return assembly;
//            }
//        }

//        private bool assemblyCatalogLoaded;
//        private AssemblyCatalog assemblyCatalog;
//        public AssemblyCatalog AssemblyCatalog
//        {
//            get
//            {
//                if (!assemblyCatalogLoaded)
//                {
//                    if(!Available)
//                    {
//                        assemblyCatalogLoaded = true;
//                        return null;
//                    }
//                    var a = Assembly;
//                    // is not loadable and throws a LoaderException, eventhough we are not interested in that dll
//                    // see: http://stackoverflow.com/questions/4020532/mef-unable-to-load-one-or-more-of-the-requested-types-retrieve-the-loaderexce
//                    try
//                    {
//                        assemblyCatalog = new AssemblyCatalog(a);
//                        var parts = assemblyCatalog.Parts.ToArray(); // throws ReflectionTypeLoadException, IF that single assembly cannot be inspected.
//                    }
//                    catch (Exception ex)
//                    {
//                        Debug.Print("Warning, Could not load MEF parts in assembly: " + ex);
//                        assemblyCatalog = null;
//                    }
//                    finally
//                    {
//                        assemblyCatalogLoaded = true;
//                    }
//                }
//                return assemblyCatalog;
//            }
//        }

//        public bool HasParts
//        {
//            get { return AssemblyCatalog != null && AssemblyCatalog.Parts.Any(); }
//        }

//    }
//}
