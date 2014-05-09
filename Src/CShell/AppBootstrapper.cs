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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using CShell.Framework;
using CShell.Framework.Services;
using Caliburn.Micro;

namespace CShell
{
    public class AppBootstrapper : Bootstrapper<IShell>
    {
        static AppBootstrapper()
        {
#if DEBUG
            //setup the logger at the earliest possible moment
            LogManager.GetLog = type => new DebugLogger(type);
#else
            LogManager.GetLog = type => new NLogLogger(type);
#endif
        }

        private const string ModulesPath = @"./Modules";
        private static List<IModule> _modules; 
        private CompositionContainer _container;

        /// <summary>
        /// By default, we are configured to use MEF
        /// </summary>
        protected override void Configure()
        {
            //to start we just add the already loaded assemblies to the container & the assemblies in exe folder
            var directoryCatalog = new DirectoryCatalog(@"./");

            //use this code to look into loader exceptions, the code bellow is faster.
            //try
            //{
            //    // load the assembly or type
            //    foreach (var part in directoryCatalog.Parts)
            //    {
            //        var assembly = ReflectionModelServices.GetPartType(part).Value.Assembly;
            //        if (!AssemblySource.Instance.Contains(assembly))
            //            AssemblySource.Instance.Add(assembly);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    if (ex is System.Reflection.ReflectionTypeLoadException)
            //    {
            //        var typeLoadException = ex as ReflectionTypeLoadException;
            //        var loaderExceptions = typeLoadException.LoaderExceptions;
            //    }
            //}

            AssemblySource.Instance.AddRange(
                directoryCatalog.Parts
                    .AsParallel()
                    .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                    .ToList()
                    .Where(assembly => !AssemblySource.Instance.Contains(assembly)));
            var catalog = new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)));
            _container = new CompositionContainer(catalog);

            var batch = new CompositionBatch();
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            var eventAggregator = new EventAggregator();
            batch.AddExportedValue<IEventAggregator>(eventAggregator);
            batch.AddExportedValue(new AssemblyLoader(_container, eventAggregator));
            batch.AddExportedValue(_container);
            //batch.AddExportedValue(catalog);
            _container.Compose(batch);
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            //the order of the statements here is important

            //1. this will show the Shell UI.
            base.OnStartup(sender, e);

            //2. init all basic modules, they themselves will register in the UI
            _modules = IoC.GetAllInstances(typeof(IModule)).Cast<IModule>().ToList();
            foreach (var module in _modules.OrderBy(m => m.Order))
                module.Initialize();

            //3. & finally forward the arguments to the shell that it can open the workspace if one was specified in the arguments.
            // this is the main reason the order matters, once the workspace is opened all modules and their dlls need to be loaded.
            var shell = IoC.Get<IShell>();
            shell.Opened(e.Args);
        }


        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var log = LogManager.GetLog(typeof(AppBootstrapper));
            //get the inner exception if thre is one (often the exception is only a target invocation ex, from caliburn that has an inner ex)
            var displayException = e.Exception;
            if (displayException.InnerException != null)
                displayException = displayException.InnerException;
            log.Error(e.Exception);
          
            //really bad exception :-0, panic and exit
            string errorMessage = string.Format("An unhandled exception occurred: {0}", displayException.Message);
            errorMessage += Environment.NewLine + Environment.NewLine;
            errorMessage += "CShell will be closed. The details can be found in the logs.";
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            Application.Shutdown();
        }

        #region IoC, wrapper around MEF container
        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract);

            if (exports.Any())
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }
        #endregion
    }
}