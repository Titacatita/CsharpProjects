using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;

namespace QuantBook.Ch02
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class NamingViewModel: Screen
    {
        [ImportingConstructor]
        public NamingViewModel()
        {
            DisplayName = "04. Naming Convention";

            Ticker = "IBM";
            Date = Convert.ToDateTime("7/14/2015");
            PriceOpen = 169.43;
            PriceHigh = 169.54;
            PriceLow = 168.24;
            PriceClose = 168.61;
        }

          private string ticker;
          private DateTime date;
          private double priceOpen;
          private double priceHigh;
          private double priceLow;
          private double priceClose;
         
          public string Ticker
          {
              get { return ticker; }
              set
              {
                  ticker = value;
                  NotifyOfPropertyChange(() => Ticker);
              }
          }

          public DateTime Date
          {
              get { return date; }
              set
              {
                  date = value;
                  NotifyOfPropertyChange(() => Date);
              }
          }

          public double PriceOpen
          {
              get { return priceOpen; }
              set
              {
                  priceOpen = value;
                  NotifyOfPropertyChange(() => PriceOpen);
              }
          }

          public double PriceHigh
          {
              get { return priceHigh; }
              set
              {
                  priceHigh = value;
                  NotifyOfPropertyChange(() => PriceHigh);
              }
          }

          public double PriceLow
          {
              get { return priceLow; }
              set
              {
                  priceLow = value;
                  NotifyOfPropertyChange(() => PriceLow);
              }
          }

          public double PriceClose
          {
              get { return priceClose; }
              set
              {
                  priceClose = value;
                  NotifyOfPropertyChange(() => PriceClose);
              }
          }


          public void Update()
          {
              if (Ticker == "IBM")
              {
                  Ticker = "MSFT";
                  Date = Convert.ToDateTime("7/14/2015");
                  PriceOpen = 45.45;
                  PriceHigh = 45.96;
                  PriceLow = 45.31;
                  PriceClose = 45.62;
              }
              else
              {
                  Ticker = "IBM";
                  Date = Convert.ToDateTime("7/14/2015");
                  PriceOpen = 169.43;
                  PriceHigh = 169.54;
                  PriceLow = 168.24;
                  PriceClose = 168.61;
              }
          }
    }
}
