using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Composition; // this is part of the MEF (Managed extensibility Framework
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Caliburn.Micro;
using System.Windows;
using System.Windows.Media.Imaging;


/* In traditioal UI development, a view is created using WPF or XAML, & then we write the code in background for handling events.
   Thus this brings strong coupling between UI, data logic and event handling, which means that  two developers cannot work
   simultaneusly in a project w/out the risk of one braking the other's code.... meaning that putting UI, data logic, event handling,
   and business  operations in one place is not suitable for maintainability, extensibility, and testability perspective.

    .... to address this issue we use (implement) the MVVM pattern in WPF programming style (e.g. XAML plus code-behind file)
    where MVVM stands for Model-View-View Model:
    * View (UI defined in XML)  -> should only know about "View Model"
        -> should just bind to the "View Model" and make the UI look pretty

    * Model (business rule, data access, model classes) -> it should know nothing about the "Model" or "View"
        -> It includes the interface to database access, computation process and business operations
        
    * View Model (agent between view and model): acts as an interface between both, it provides data binding  between "View" and
        "Model data", it also handles all UI actions by using commands -> should only know about "Model"
        -> delegates everything to the "Model" except for exposing the data for the "View"

    Overall,
    This way "View Model" and and "Model" are easier to test , and we achieve loose coupling , that provides benefits 
    from an extensability and maintenability perspective 
    */


/* Caliburn.Micro
    -> we create the Boostrapperclass which is a mechanism to implement the CM into our application  

*/ 

namespace QuantBook_v1.Models
{
    public class AppBootStraper : BootstrapperBase
    {
        private CompositionContainer container;

        public AppBootStraper()
        {
            Initialize();
        }

        //configuration of the container
        // here we override the config method wich sets up the Inversion of Control container & performs any other 
        // conf such as the customizing conventions
        protected override void Configure()
        {
            container = new CompositionContainer( //creating new IoC container
                new AggregateCatalog(AssemblySource.Instance.Select(
                    x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()));
            CompositionBatch batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(container);

            container.Compose(batch);
        }

        //In the next 3 override methods we tell Caliburn Micro how to use the container
        protected override object GetInstance(Type servicetType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? 
                AttributedModelServices.GetContractName(servicetType) : key;
            var exports = container.GetExportedValues<object>(contract);

            if (exports.Any())
            {
                return exports.First();
            }

            throw new Exception((string.Format("Could not locate any innstance of contract {0} ", contract)));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            container.SatisfyImportsOnce(instance);
        }


        // Here we specify the type of the "root view model" via a generic method
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainViewModel>();

        }
    }
}
