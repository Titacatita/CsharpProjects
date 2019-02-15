using Caliburn.Micro;
using System;
using System.Windows;

namespace QuantBook.Ch02
{
    public class Ch02Model
    {
        private string ticker;
        private DateTime date;
        private double priceOpen;
        private double priceHigh;
        private double priceLow;
        private double priceClose;
        private double volume;

        public string Ticker
        {
            get { return ticker; }
            set { ticker = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public double PriceOpen
        {
            get { return priceOpen; }
            set { priceOpen = value; }
        }

        public double PriceHigh
        {
            get { return priceHigh; }
            set { priceHigh = value; }
        }

        public double PriceLow
        {
            get { return priceLow; }
            set { priceLow = value; }
        }

        public double PriceClose
        {
            get { return priceClose; }
            set { priceClose = value; }
        }

        public double Volume
        {
            get { return volume; }
            set { volume = value; }
        }
    }


    /*public class Ch02Model2 : PropertyChangedBase
    {
        private string ticker;
        private DateTime date;
        private double priceOpen;
        private double priceHigh;
        private double priceLow;
        private double priceClose;
        private double volume;

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

        public double Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                NotifyOfPropertyChange(() => Volume);
            }
        }
    }*/



    /*public class Ch02Model3 : DependencyObject
    {
        public static DependencyProperty TickerProperty =
           DependencyProperty.Register("Ticker", typeof(string),
           typeof(Ch02Model3), new FrameworkPropertyMetadata("IBM",
           FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Ticker
        {
            get { return (string)GetValue(TickerProperty); }
            set { SetValue(TickerProperty, value); }
        }

        public static DependencyProperty DateProperty =
           DependencyProperty.Register("Date", typeof(DateTime),
           typeof(Ch02Model3), new FrameworkPropertyMetadata(new DateTime(),
           FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public static DependencyProperty  PriceOpenProperty =
           DependencyProperty.Register("PriceOpen", typeof(double),
           typeof(Ch02Model3), new FrameworkPropertyMetadata(0.0,
           FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double PriceOpen
        {
            get { return (double)GetValue(PriceOpenProperty); }
            set { SetValue(PriceOpenProperty, value); }
        }

        public static DependencyProperty PriceHighProperty =
           DependencyProperty.Register("PriceHigh", typeof(double),
           typeof(Ch02Model3), new FrameworkPropertyMetadata(0.0,
           FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double PriceHigh
        {
            get { return (double)GetValue(PriceHighProperty); }
            set { SetValue(PriceHighProperty, value); }
        }

        public static DependencyProperty PriceLowProperty =
           DependencyProperty.Register("PriceLow", typeof(double),
           typeof(Ch02Model3), new FrameworkPropertyMetadata(0.0,
           FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double PriceLow
        {
            get { return (double)GetValue(PriceLowProperty); }
            set { SetValue(PriceLowProperty, value); }
        }

        public static DependencyProperty PriceCloseProperty =
           DependencyProperty.Register("PriceClose", typeof(double),
           typeof(Ch02Model3), new FrameworkPropertyMetadata(0.0,
           FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double PriceClose
        {
            get { return (double)GetValue(PriceCloseProperty); }
            set { SetValue(PriceCloseProperty, value); }
        }

        public static DependencyProperty VolumeProperty =
           DependencyProperty.Register("Volume", typeof(double),
           typeof(Ch02Model3), new FrameworkPropertyMetadata(0.0,
           FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }
    }*/
}
