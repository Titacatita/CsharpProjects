using System;
using Caliburn.Micro;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuantBook_v1
{
    /// <summary>
    /// Interaction logic for WrongWayViewModel.xaml
    /// The first 3 tags (lines) bellow belong to the Caliburn Micro MVVM toolkit
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class WrongWayViewModel : Screen
    {
        [ImportingConstructor] // MVVM CM
        public WrongWayViewModel() 
        {
            DisplayName = "01. Wrong Way";


            Model = new Ch02Model
            {
                Ticker = "IBM",
                Date = Convert.ToDateTime("14/7/2015"),
                PriceOpen = 169.43,
                PriceHigh = 169.54,
                PriceLow = 168.24,
                PriceClose = 168.61,
            };
        }

        public Ch02Model Model{ get; set; }

        public void Update()
        {
            if (Model.Ticker == "IBM")
            {
                Model.Ticker = "MSFT";
                Model.Date = Convert.ToDateTime("14/7/2015");
                Model.PriceOpen = 45.45;
                Model.PriceHigh = 45.96;
                Model.PriceLow = 45.31;
                Model.PriceClose = 45.62;

            }
            else
            {
                Model.Ticker = "IBM";
                Model.Date = Convert.ToDateTime("14/7/2015");
                Model.PriceOpen = 169.43;
                Model.PriceHigh = 169.54;
                Model.PriceLow = 168.24;
                Model.PriceClose = 168.61;
            }
        }
    }
}
