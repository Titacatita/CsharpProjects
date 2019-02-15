using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;

namespace QuantBook.Ch02
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CollectionViewModel : Screen
    {
        [ImportingConstructor]
        public CollectionViewModel()
        {
            DisplayName = "05. Collection";
            DataCollection = new BindableCollection<Ch02Model2>();
            GetInitialData();
        }

        public BindableCollection<Ch02Model2> DataCollection { get; set; }

        private void GetInitialData()
        {
            Ch02Model2 model = new Ch02Model2
            {
                Ticker = "IBM",
                Date = Convert.ToDateTime("7/14/2015"),
                PriceOpen = 169.43,
                PriceHigh = 169.54,
                PriceLow = 168.24,
                PriceClose = 168.61,
                Volume = 2974900
            };
            DataCollection.Add(model);

            model = new Ch02Model2
            {
                Ticker = "MSFT",
                Date = Convert.ToDateTime("7/14/2015"),
                PriceOpen = 45.45,
                PriceHigh = 45.96,
                PriceLow = 45.31,
                PriceClose = 45.62,
                Volume = 22723700
            };
            DataCollection.Add(model);

            model = new Ch02Model2
            {
                Ticker = "INTC",
                Date = Convert.ToDateTime("7/14/2015"),
                PriceOpen = 29.66,
                PriceHigh = 30.11,
                PriceLow = 29.44,
                PriceClose = 29.65,
                Volume = 39276800
            };
            DataCollection.Add(model);

            model = new Ch02Model2
            {
                Ticker = "IBM",
                Date = Convert.ToDateTime("7/13/2015"),
                PriceOpen = 167.93,
                PriceHigh = 169.89,
                PriceLow = 167.52,
                PriceClose = 169.38,
                Volume = 4225500
            };
            DataCollection.Add(model);

            model = new Ch02Model2
            {
                Ticker = "MSFT",
                Date = Convert.ToDateTime("7/13/2015"),
                PriceOpen = 44.98,
                PriceHigh = 45.62,
                PriceLow = 44.95,
                PriceClose = 45.54,
                Volume = 24994700
            };
            DataCollection.Add(model);

            model = new Ch02Model2
            {
                Ticker = "INTC",
                Date = Convert.ToDateTime("7/13/2015"),
                PriceOpen = 29.27,
                PriceHigh = 29.82,
                PriceLow = 29.19,
                PriceClose = 29.73,
                Volume = 26335600
            };
            DataCollection.Add(model);
        }

        public void Update()
        {
            DataCollection.Add(new Ch02Model2
            {
                Ticker = "AAPL",
                Date = Convert.ToDateTime("7/14/2015"),
                PriceOpen = 126.04,
                PriceHigh = 126.37,
                PriceLow = 125.04,
                PriceClose = 125.61,
                Volume = 31535500
            });
        }
    }
}
