using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;

namespace QuantBook.Ch02
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class WrongWayViewModel : Screen
    {
        [ImportingConstructor]
        public WrongWayViewModel()
        {
            DisplayName = "01. Wrong Way";

            Model = new Ch02Model
            {
                Ticker = "IBM",
                Date = Convert.ToDateTime("7/14/2015"),
                PriceOpen = 169.43,
                PriceHigh = 169.54,
                PriceLow = 168.24,
                PriceClose = 168.61,
            };
        }

        public Ch02Model Model { get; set; }
              
        public void Update()
        {
            if (Model.Ticker == "IBM")
            {
                Model.Ticker = "MSFT";
                Model.Date = Convert.ToDateTime("7/14/2015");
                Model.PriceOpen = 45.45;
                Model.PriceHigh = 45.96;
                Model.PriceLow = 45.31;
                Model.PriceClose = 45.62;
            }
            else
            {
                Model.Ticker = "IBM";
                Model.Date = Convert.ToDateTime("7/14/2015");
                Model.PriceOpen = 169.43;
                Model.PriceHigh = 169.54;
                Model.PriceLow = 168.24;
                Model.PriceClose = 168.61;
            }
        }
    }
}
