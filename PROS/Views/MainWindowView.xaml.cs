using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PROS.Views
{    
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        IRegionManager _regionManager;

        public MainWindowView(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;

            // Register view with region.
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(PROS.Module.Home.Views.HomeView));


        }
    }
}
