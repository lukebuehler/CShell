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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.ReflectionModel;
using System.ComponentModel.Composition.Registration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CShell.Framework;
using CShell.Framework.Services;
using Caliburn.Micro;
using CShell.Hosting;
using IModule = CShell.Framework.IModule;
using LogLevel = Common.Logging.LogLevel;
using LogManager = Caliburn.Micro.LogManager;

namespace CShell
{
    public class AppBootstrapper : Caliburn.Micro.BootstrapperBase
    {
        static AppBootstrapper()
        {

#if DEBUG
            Common.Logging.LogManager.Adapter = new Common.Logging.Simple.TraceLoggerFactoryAdapter(LogLevel.Debug, false, false, true, "HH:mm:ss", true);
#else
            Common.Logging.LogManager.Adapter = new Common.Logging.Simple.NoOpLoggerFactoryAdapter();
#endif
            LogManager.GetLog = type => new Logger(type);
        }

        public AppBootstrapper()
        {
            Initialize();
        }

        private CompositionContainer container;
        private List<IModule> modules;

        /// <summary>
        /// By default, we are configured to use MEF
        /// </summary>
        protected override void Configure()
        {
            var exeDir = Assembly.GetExecutingAssembly().Location;

            //setup catalog with core assemblies
            var aggregateCatalog = new AggregateCatalog(
                new AssemblyCatalog(GetType().Assembly), //CShell
                new AssemblyCatalog(typeof(IShell).Assembly) //CShell.Core
                );

            //load modules
            var moduleBuilder = new RegistrationBuilder();
            moduleBuilder.ForTypesDerivedFrom<IModule>().Export<IModule>();
            HostingHelpers.ConfigureModuleRegistrationBuilder(moduleBuilder);
            
            var modulesDir = Path.Combine(exeDir, Constants.CShellModulesPath);
            if (Directory.Exists(modulesDir))
            {
                var directories = Directory.GetDirectories(modulesDir).ToList();
                directories.Add(modulesDir);
                foreach (var dir in directories)
                {
                    aggregateCatalog.Catalogs.Add(new DirectoryCatalog(dir, moduleBuilder));
                }
            }

            //make module assemblies available to caliburn.micro 
            AssemblySource.Instance.AddRange(
                aggregateCatalog.Parts
                    .AsParallel()
                    .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                    .Distinct()
                    .ToList()
                    .Where(assembly => !AssemblySource.Instance.Contains(assembly)));

            //setup ScriptCS hosting
            HostingHelpers.ConfigureHostingCatalog(aggregateCatalog);

            //setup the container
            container = new CompositionContainer(aggregateCatalog);
            //add custom exports 
            var batch = new CompositionBatch();
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(container);
            container.Compose(batch);

            //configure the modules
            modules = container.GetExportedValues<IModule>().ToList();
            foreach (var module in modules.OrderBy(m => m.Order))
            {
                var moduleConfiguration = new ModuleConfiguration(new CompositionBatch());
                module.Configure(moduleConfiguration);
                //see if we should update the DI container
                if(moduleConfiguration.CompositionBatch.PartsToAdd.Count > 0 || moduleConfiguration.CompositionBatch.PartsToRemove.Count > 0)
                    container.Compose(batch);
            }
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //the order of the statements here is important

            //1. this will show the Shell UI.
            DisplayRootViewFor<IShell>();
            //base.OnStartup(sender, e);

            //2. init all basic modules, they themselves will register in the UI
            foreach (var module in modules.OrderBy(m => m.Order))
                module.Start();

            //3. & finally forward the arguments to the shell that it can open the workspace if one was specified in the arguments.
            // this is the main reason the order matters, once the workspace is opened all modules and their dlls need to be loaded.
            var shell = IoC.Get<IShell>();

            Task.Run(async () =>
            {
                await Task.Delay(100);
                await Caliburn.Micro.Execute.OnUIThreadAsync(() => shell.Opened(e.Args));
            });
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
            var exports = container.GetExportedValues<object>(contract);

            if (exports.Any())
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
        }
        #endregion
    }
}