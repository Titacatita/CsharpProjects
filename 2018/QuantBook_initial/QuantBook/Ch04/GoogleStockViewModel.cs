using System;
using System.Linq;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models;
using System.Collections.Generic;
using System.Windows.Threading;
using QuantBook.Models.Google;
using QuantBook.Models.DataModel.Google;

namespace QuantBook.Ch04
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GoogleStockViewModel : Screen
    {
        private readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public GoogleStockViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "04. Google Stock";
            MyStockHist = new BindableCollection<GoogleStockPrice>();
            MyStockBars = new BindableCollection<GoogleStockPrice>();
            MyStockQuotes = new BindableCollection<GoogleQuote>();
 
            StartDate = "1/1/2015".To<DateTime>();
            EndDate = DateTime.Today;
        }

        public BindableCollection<GoogleStockPrice> MyStockHist { get; set; }
        public BindableCollection<GoogleStockPrice> MyStockBars { get; set; }
        public BindableCollection<GoogleQuote> MyStockQuotes { get; set; }
  
        private string ticker = "IBM";
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

        private IEnumerable<GoogleDataTypeEnum> dataType;
        public IEnumerable<GoogleDataTypeEnum> DataType
        {
            get { return Enum.GetValues(typeof(GoogleDataTypeEnum)).Cast<GoogleDataTypeEnum>(); }
            set
            {
                dataType = value;
                NotifyOfPropertyChange(() => DataType);
            }
        }

        private GoogleDataTypeEnum selectedDataType;
        public GoogleDataTypeEnum SelectedDataType
        {
            get { return selectedDataType; }
            set
            {
                selectedDataType = value;
                NotifyOfPropertyChange(() => SelectedDataType);
            }
        }


        public void StockHist()
        {
            MyStockHist.Clear();
            BindableCollection<GoogleStockPrice> stock = GoogleHelper.GetGoogleStockData(Ticker, StartDate, EndDate, SelectedDataType, _events);
            MyStockHist.AddRange(stock);
        }

       
        public void StockQuotes()
        {            
            MyStockQuotes.Clear();
            MyStockQuotes.Add(new GoogleQuote("SPY"));
            MyStockQuotes.Add(new GoogleQuote("MSFT"));
            MyStockQuotes.Add(new GoogleQuote("INTC"));
            MyStockQuotes.Add(new GoogleQuote("IBM"));
            MyStockQuotes.Add(new GoogleQuote("AMZN"));
            MyStockQuotes.Add(new GoogleQuote("BIDU"));
            MyStockQuotes.Add(new GoogleQuote("SINA"));
            MyStockQuotes.Add(new GoogleQuote("NVDA"));
            MyStockQuotes.Add(new GoogleQuote("AMD"));
            MyStockQuotes.Add(new GoogleQuote("WMT"));
            MyStockQuotes.Add(new GoogleQuote("GLD"));
            MyStockQuotes.Add(new GoogleQuote("SLV"));
            MyStockQuotes.Add(new GoogleQuote("GS"));
            MyStockQuotes.Add(new GoogleQuote("JPM"));
            MyStockQuotes.Add(new GoogleQuote("MCD"));

            GoogleHelper.GetGoogleStockQuote(MyStockQuotes);

            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Tick += (o, e) =>
            {
                GoogleHelper.GetGoogleStockQuote(MyStockQuotes);
            };

            timer.Start();
        }
    }
}
