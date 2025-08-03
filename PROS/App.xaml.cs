using PROS.Views;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Navigation;

namespace PROS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
         protected override Window CreateShell()
         {
            // Return the main window of the application. 
            return Container.Resolve<PROS.Views.MainWindowView>();
         }    

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register Service.
            //containerRegistry.RegisterSingleton<IDataHandlerService, DataHandlerService>();
            //containerRegistry.RegisterDialog<CustomPopup, CustomPopupViewModel>();
            //ontainerRegistry.RegisterSingleton<IDBService,  DBService>();

            // Register View for Navigation.
            containerRegistry.RegisterForNavigation<PROS.Module.Home.Views.HomeView>();


            // Register EventAggregator.
            containerRegistry.RegisterSingleton<IEventAggregator, EventAggregator>();

        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            // Register View with ViewModel
            ViewModelLocationProvider.Register<PROS.Module.Home.Views.HomeView, PROS.Module.Home.ViewModels.HomeViewModel>();
            //ViewModelLocationProvider.Register<QLSX.Module.Products.Views.ProductsView, QLSX.Module.Products.ViewModels.ProductsViewModel>();
        }
    }
}
