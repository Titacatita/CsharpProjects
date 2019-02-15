using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantBook_v1
{
    //This class is our Model that just defines the properties for a stock
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
            get { return ticker;}
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
            set { priceOpen = value; }
        }

        public double Volume
        {
            get { return volume; }
            set { volume = value; }
        }
    }
}
