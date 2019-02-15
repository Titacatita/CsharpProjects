using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Windows.Threading;
using QuantBook.Models.Yahoo;
using System.Data;

namespace QuantBook.Ch04
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class YahooStockViewModel : Screen
    {
        private readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public YahooStockViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "01. Yahoo Stock";
            MyQuotes = new BindableCollection<StockQuote>();
            MyPrices = new DataTable();
            Ticker = "IBM";
            StartDate = DateTime.Today.AddYears(-5);
            EndDate = DateTime.Today;
        }

        public BindableCollection<StockQuote> MyQuotes { get; set; }
        private DataTable myPrices;
        public DataTable MyPrices
        {
            get { return myPrices; }
            set
            {
                myPrices = value;
                NotifyOfPropertyChange(() => MyPrices);
            }
        }

        private string ticker;
        public string Ticker
        {
            get { return ticker; }
            set
            {
                ticker = value;
                NotifyOfPropertyChange(() => Ticker);
            }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }

        public void HistPrices()
        {  
            MyPrices = YahooHelper.GetYahooHistStockDataTable(Ticker, StartDate, EndDate);
        }

        public void StockQuotes()
        {
            MyQuotes.Clear();
            MyQuotes.Add(new StockQuote("^IXIC"));
            MyQuotes.Add(new StockQuote("^GSPC"));
            MyQuotes.Add(new StockQuote("MSFT"));
            MyQuotes.Add(new StockQuote("INTC"));
            MyQuotes.Add(new StockQuote("IBM"));
            MyQuotes.Add(new StockQuote("AMZN"));
            MyQuotes.Add(new StockQuote("BIDU"));
            MyQuotes.Add(new StockQuote("SINA"));
            MyQuotes.Add(new StockQuote("NVDA"));
            MyQuotes.Add(new StockQuote("AMD"));
            MyQuotes.Add(new StockQuote("WMT"));
            MyQuotes.Add(new StockQuote("GLD"));
            MyQuotes.Add(new StockQuote("SLV"));
            MyQuotes.Add(new StockQuote("GS"));
            MyQuotes.Add(new StockQuote("JPM"));
            MyQuotes.Add(new StockQuote("MCD"));

            YahooHelper.GetQuotes(MyQuotes);

            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Tick += (o, e) =>
            {
                YahooHelper.GetQuotes(MyQuotes);
            };

            timer.Start();
        }
    }
}
