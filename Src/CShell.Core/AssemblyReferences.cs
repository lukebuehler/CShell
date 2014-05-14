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
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CShell.Util;

namespace CShell
{
    //should the whole workspace run in a custom app domain? especially the scriptinge engine?

    //then the workspace could really be unloaded!
    // -> see how linqpad does it!
    //note: shadow copy only works if we enable a cusotm app domain!

    public class AssemblyReferences : ObservableKeyedCollection<string, AssemblyReference>, IDisposable
    {
        private readonly ScriptingEngine engine;

        public AssemblyReferences(ScriptingEngine engine)
        {
            this.engine = engine;

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        public void Dispose()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve -= CurrentDomainOnAssemblyResolve;
        }

        #region KeyedCollection
        protected override string GetKeyForItem(AssemblyReference item)
        {
            return item.AssemblyName.Name;
        }

        protected override void InsertItem(int index, AssemblyReference item)
        {
            //add the assembly to the scripting engine
            if(item.Available)
                engine.ReferenceAssembly(item.Assembly);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, AssemblyReference item)
        {
            //add the assembly to the scripting engine
            if (item.Available)
                engine.ReferenceAssembly(item.Assembly);
            base.SetItem(index, item);
        }
        #endregion

        #region Manaage Assymbly References
        public void Add(AssemblyName assemblyName)
        {
            if (assemblyName == null) throw new ArgumentNullException("assemblyName");
            if (!Contains(assemblyName.Name))
                Add(new AssemblyReference(assemblyName));
        }

        public void Add(string file)
        {
            if (file == null) throw new ArgumentNullException("file");
            var key = Path.GetFileNameWithoutExtension(file);
            if (key != null && !Contains(key))
                Add(new AssemblyReference(file));
        }

        public void AddReferences(string path)
        {
            AddReferences(path, "*.dll");
        }

        public void AddReferences(string path, string searchPattern)
        {
            if (path == null) throw new ArgumentNullException("path");
            var foundFiles = System.IO.Directory.GetFiles(path, searchPattern);
            foreach (var file in foundFiles.Where(f=>Path.GetExtension(f).ToLower() == ".dll"))
            {
                Add(file);
            }
        }
        #endregion

        #region Resolve and Load Assemblies
        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return ResolveAssembly(args.Name, args.RequestingAssembly);
        }

        public Assembly ResolveAssembly(string assemblyFullName)
        {
            return ResolveAssembly(assemblyFullName, null);
        }

        public Assembly ResolveAssembly(string assemblyFullName, Assembly requestingAssembly)
        {
            if (assemblyFullName == null) throw new ArgumentNullException("assemblyFullName");
            var requestingAssemblyFullName = requestingAssembly != null ? requestingAssembly.FullName : "";
            Debug.Print("AssemblyReferences: Trying to resolve {0}, (requesting assembly: {1})", assemblyFullName, requestingAssemblyFullName);

            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyFullName);
            if (assembly != null)
            {
                Debug.Print("AssemblyReferences: Found assembly in the AppDomain.");
                return assembly;
            }

            Debug.Print("AssemblyReferences: Assembly not found in AppDomain, looking in search paths.");
            //look in all the paths of the assemblies we already have
            var assemblyFiles = AssemblyLoader.GetAssemblySearchFiles(this);
            var assemblyName = AssemblyLoader.GetAssemblyName(assemblyFullName);
            var foundFile = assemblyFiles.FirstOrDefault(file => assemblyName.Equals(Path.GetFileNameWithoutExtension(file)));
            if (foundFile != null)
            {
                Debug.Print("AssemblyReferences: Assembly file found, loading it: " + foundFile);
                return Assembly.LoadFrom(foundFile);
            }
            else
            {
                Debug.Print("AssemblyReferences: No assembly file found in search paths.");
            }
            return null;
        }

        #endregion

    }//end class
}
