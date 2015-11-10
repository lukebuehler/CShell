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
using ScriptCs;
using ScriptCs.Contracts;
using IModule = CShell.Framework.IModule;

namespace CShell
{
    public class AppBootstrapper : Caliburn.Micro.BootstrapperBase
    {
        static AppBootstrapper()
        {

#if DEBUG
            Caliburn.Micro.LogManager.GetLog = type => new CShell.Framework.Services.LoggerDebug(type);
#else
            Caliburn.Micro.LogManager.GetLog = type => new CShell.Framework.Services.LoggerNLog(type);
#endif
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
            //add CShell assemblies, this .exe assembly is already added
            AssemblySource.Instance.Add(typeof(IShell).Assembly);

            //load modules
            var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var moduleCatalog = new AggregateCatalog(); //we're using this catalog just to *find* the relevant modules
            var moduleBuilder = new RegistrationBuilder();
            moduleBuilder.ForTypesDerivedFrom<IModule>().Export<IModule>();
            HostingHelpers.ConfigureModuleRegistrationBuilder(moduleBuilder); //allow the script cs integration code to register intrefaces to search for in the assemblies.
            
            //get module dirs
            var modulesDir = Path.Combine(exeDir, Constants.ModulesPath);
            if (Directory.Exists(modulesDir))
            {
                var directories = Directory.GetDirectories(modulesDir).ToList();
                directories.Add(modulesDir);
                foreach (var dir in directories)
                {
                    moduleCatalog.Catalogs.Add(new DirectoryCatalog(dir, moduleBuilder));
                }
            }

            //make module assemblies available to caliburn.micro
            var moduleAssemblies = moduleCatalog.Parts
                .AsParallel()
                .Select(part => ReflectionModelServices.GetPartType(part).Value.Assembly)
                .Distinct()
                .ToList()
                .Where(assembly => !AssemblySource.Instance.Contains(assembly))
                .Where(assembly => assembly.GetName().Name != "CShell.Core" && assembly.GetName().Name != "CShell") //dont know why CShell.Core references are not filtered out in the line above, we force it here
                .ToList();
            AssemblySource.Instance.AddRange(moduleAssemblies);


            //Now create the final catalog only using the caliburn AssemblySource
            var aggregateCatalog = new AggregateCatalog(AssemblySource.Instance.Select(assembly => new AssemblyCatalog(assembly)));

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

            //use this for testing the MEF dependency resolution
            //HostingHelpers.TestIfAllExportsCanBeResolved(container);

            //configure the modules
            modules = container.GetExportedValues<IModule>().ToList();
          
            //order them
            var orderedModules = modules.Where(m => m.Order > 0).ToList();
            var unorderedModules = modules.Where(m => m.Order < 1).ToList();
            modules = orderedModules.OrderBy(m => m.Order).Concat(unorderedModules).ToList();
            foreach (var module in modules)
            {
                module.Configure();
            }

            //use this to debug the IoC, can the workspace be resolved?
            //var w1 = container.GetExportedValue<Workspace>();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //the order of the statements here is important

            //1. this will show the Shell UI.
            DisplayRootViewFor<IShell>();
            //base.OnStartup(sender, e);

            //2. init all basic modules, they themselves will register in the UI
            foreach (var module in modules)
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