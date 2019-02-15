using System;
using Caliburn.Micro;

namespace QuantBook.Ch02
{
    public class Ch02Model2 : PropertyChangedBase
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
    }
}
