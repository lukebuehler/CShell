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
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CShell.Framework;
using CShell.Util;
using Caliburn.Micro;

namespace CShell
{
    /// <summary>
    /// Static Helper classes to manage the assemblies.
    /// The instance is subscribed to the workpsace events and the <see cref="AssemblyReferences"/> to load it's modules to the container.
    /// </summary>
    public class AssemblyLoader : IHandle<CShell.WorkspaceOpeningEventArgs>
    {
        #region Instance - Manage and load modules
        private readonly CompositionContainer container;
        private readonly AggregateCatalog catalog;
        private readonly ILog log = LogManager.GetLog(typeof (AssemblyLoader));
        private readonly List<IModule> loadedModules = new List<IModule>();
        private readonly List<AssemblyReference> loadedAssemblies = new List<AssemblyReference>();

        public AssemblyLoader(CompositionContainer container, IEventAggregator eventAggregator)
        {
            this.container = container;
            this.catalog = container.Catalog as AggregateCatalog;
            if(catalog == null)
                throw new ArgumentException("The containers catalog needs to be an AggregateCatalog.");

            eventAggregator.Subscribe(this);
        }

        public void Handle(WorkspaceOpeningEventArgs message)
        {
            loadedModules.Clear();
            loadedAssemblies.Clear();
            //message.Workspace.Assemblies.CollectionChanged += AssembliesOnCollectionChanged;
        }

        //public void Handle(WorkspaceClosingEventArgs message)
        //{
        //    message.Workspace.Assemblies.CollectionChanged -=AssembliesOnCollectionChanged;

        //    //we cannot just remove all assemblies when the project closes since some of them are loaded at startup
        //    // so we maintain a seperate assembly list that is cleared when the project closes.
        //    foreach (var assemblyReference in loadedAssemblies.ToList())
        //    {
        //        AssemblyRemoved(assemblyReference);
        //    }
        //    loadedAssemblies.Clear();
        //    loadedModules.Clear();
        //}

        private void AssembliesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
            {
                foreach (var item in args.OldItems)
                {
                    var assemblyReference = item as AssemblyReference;
                    if (assemblyReference != null)
                        AssemblyRemoved(assemblyReference);
                }
            }

            if (args.NewItems != null)
            {
                foreach (var item in args.NewItems)
                {
                    var assemblyReference = item as AssemblyReference;
                    if (assemblyReference != null)
                        AssemblyAdded(assemblyReference);
                }
            }
        }

        private void AssemblyAdded(AssemblyReference assemblyReference)
        {
            //the only good way to compare part definitions is by comparing the result to ToString()
            // see: http://stackoverflow.com/questions/3488893/how-can-i-avoid-redundant-imports-in-mef
            var existingCatalogLookup = catalog.Parts.ToLookup(part => part.ToString());

            if (!assemblyReference.HasParts)
            {
                log.Info("Loaded assembly contains no MEF parts: "+assemblyReference.AssemblyName.FullName);
                return;
            }
            var assemblyCatalog = assemblyReference.AssemblyCatalog;
            var assembly = assemblyReference.Assembly;

            log.Info("Adding assembly parts ({0}) to MEF container: {1}", assemblyCatalog.Parts.Count(), assembly.FullName);

            foreach (var part in assemblyCatalog.Parts)
            {
                if (existingCatalogLookup.Contains(part.ToString()))
                {
                    log.Warn("The part '{0}' is already loaded: {1}", part, assembly.GetName().Name);
                    return;
                }
            }

            if (AssemblySource.Instance.Contains(assembly))
            {
                log.Warn("This assembly has already been loaded by Caliburn: {0}", assembly.GetName().Name);
                return;
            }

            //besides updating the MEF container we also need to add the assembly to the Caliburn AssemblySources, from where the VMs and Views are loaded.
            AssemblySource.Instance.Add(assembly);

            //now we can add the assembly catalog
            catalog.Catalogs.Add(assemblyCatalog);


            //find the modules
            var modules = new List<IModule>();
            foreach (var part in assemblyCatalog.Parts)
            {
                try
                {
                    var exportDefinition = part.ExportDefinitions.FirstOrDefault(def => def.ContractName == "CShell.Framework.IModule");
                    if (exportDefinition != null)
                    {
                        var module = part.CreatePart().GetExportedValue(exportDefinition) as IModule;
                        if (module != null)
                            modules.Add(module);
                    }
                }
                catch (Exception ex)
                {
                    log.Warn("Could not load module: " +ex.Message);
                    log.Error(ex);
                }
            }

            //now initialize the modules in order
            foreach (var module in modules.OrderBy(m=>m.Order))
            {
                try
                {
                    container.SatisfyImportsOnce(module);
                    module.Initialize();
                    loadedModules.Add(module);
                }
                catch (Exception ex)
                {
                    log.Warn("Could not initialize module: " + ex.Message);
                    log.Error(ex);
                }
            }

            //add this to the laoded assemblies
            loadedAssemblies.Add(assemblyReference);
        }

        private void AssemblyRemoved(AssemblyReference assemblyReference)
        {
            if(assemblyReference.Assembly == null)
                return;

            loadedAssemblies.Remove(assemblyReference);
            AssemblySource.Instance.Remove(assemblyReference.Assembly);

            if (assemblyReference.AssemblyCatalog != null)
            {
                //deinitialize all modules that belong to this assembly
                var modulesToRemove = loadedModules.Where(m => m.GetType().Assembly == assemblyReference.Assembly).ToList();
                foreach (var module in modulesToRemove)
                {
                    try
                    {
                        module.Dispose();
                        loadedModules.Remove(module);
                    }
                    catch (Exception ex)
                    {
                        log.Warn("Could not dispose module: " + ex.Message);
                        log.Error(ex);
                    }
                }
                //at last remove it from the MEF catalog so that the IoC doessnt find it anymore.
                // note: the assembly is still loaded in the AppDomain but that's fine, if the assembly is requested later on it will be loaded from the AppDomain again (see AssemblyLoader)
                catalog.Catalogs.Remove(assemblyReference.AssemblyCatalog);
            }
        }

        #endregion

        #region Static - General Assembly Helpers
        public static string GetAssemblyName(string fullName)
        {
            return new AssemblyName(fullName).Name;
        }
        #endregion

        #region Static - Load and Search Assembly References
        public static string[] GetAssemblySearchFiles(IEnumerable<AssemblyReference> references)
        {
            return GetAssemblySearchPaths(references)
                .SelectMany(p => Directory.GetFiles(p, "*.dll"))
                .ToArray();
        }

        public static string[] GetAssemblySearchPaths(IEnumerable<AssemblyReference> references)
        {
            var paths = references
                .Where(a => a.FullPath != null)
                .Select(a => a.FullPath)
                .Select(Path.GetDirectoryName)
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .ToArray();
            return paths;
        }

        public static Assembly LoadAssembly(AssemblyReference reference)
        {
            var assemblyName = reference.AssemblyName;
            //look in the app domain if the assembly is already loaded
            Debug.Print("AssemblyReferences: Loading assembly: " + assemblyName.FullName);

            Assembly assembly = null;
            //if the assembly ref has a full name we search for that in the App Domain, otherwise we use the short name
            if(reference.AssemblyName.Name != reference.AssemblyName.FullName)
                assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyName.FullName);
            else
                assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName.Name);

            if (assembly != null)
            {
                Debug.Print("AssemblyReferences: Found assembly in the AppDomain.");
                return assembly;
            }
            else if(!String.IsNullOrEmpty(reference.FullPath))
            {
                Debug.Print("AssemblyReferences: Assembly not found in AppDomain, loading from file: " + reference.FullPath);
                return Assembly.LoadFrom(reference.FullPath);
            }
            else
            {
                Debug.Print("AssemblyReferences: Assembly not found in AppDomain, loading from name: " + reference.AssemblyName.FullName);
                return Assembly.Load(reference.AssemblyName);
            }
        }
        #endregion

        #region Static - List and Search the GAC
        private static readonly Lazy<AssemblyName[]> gacAssembyNames = new Lazy<AssemblyName[]>(() => Util.GlobalAssemblyCache.GetAssemblyList().ToArray(), true);
        private static readonly Lazy<Assembly[]> gacAssemblies = new Lazy<Assembly[]>(() =>
        {
            var assemblies = new List<Assembly>();
            var errorCount = 0;
            foreach (var assemblyName in gacAssembyNames.Value)
            {
                try
                {
                    var assembly = Assembly.ReflectionOnlyLoad(assemblyName.FullName);
                    assemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    errorCount++;
                    Debug.Print("Could not load GAC assembly '{0}': {1}",assemblyName.FullName, ex);
                }
            }
            Debug.Print("GAC error count: "+errorCount);
            return assemblies.ToArray();
        }, true);

        public static AssemblyName[] GetGacAssemblyNames()
        {
            return gacAssembyNames.Value;
        }

        /// <summary>
        /// Gets the assemblies in the GAC, the assemblies are loaded reflection only.
        /// </summary>
        /// <returns></returns>
        public static Assembly[] GetGacAssemblies()
        {
            return gacAssemblies.Value;
        }

        public static string GetGacAssemblyPath(AssemblyName assemblyName)
        {
            return GlobalAssemblyCache.FindAssemblyInNetGac(assemblyName);
        }

        #endregion
    }
}
