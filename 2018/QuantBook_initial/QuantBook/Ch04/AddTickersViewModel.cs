using System.Collections.Generic;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Reflection;
using System.IO;
using QuantBook.Models.DataModel;
using QuantBook.Models.Yahoo;
using QuantBook.Models;

namespace QuantBook.Ch04
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddTickersViewModel : Screen
    {
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public AddTickersViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "02. Add Tickers";
           
            TickerFile = Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.FullName + @"\Models\DataModel\StockTickers.csv";
            TickerCollection = new BindableCollection<Symbol>();
        }
        
        public BindableCollection<Symbol> TickerCollection { get; set; }

        private string tickerFile;
        public string TickerFile
        {
            get { return tickerFile; }
            set
            {
                tickerFile = value;
                NotifyOfPropertyChange(() => TickerFile);
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

        private string region;
        public string Region
        {
            get { return region; }
            set
            {
                region = value;
                NotifyOfPropertyChange(() => Region);
            }
        }

        private string sector;
        public string Sector
        {
            get { return sector; }
            set
            {
                sector = value;
                NotifyOfPropertyChange(() => Sector);
            }
        }

        public void AddTicker()
        {
            Symbol symbol = new Symbol();
            symbol.Ticker = Ticker;
            symbol.Region = Region;
            symbol.Sector = Sector;

            YahooHelper.SymbolInsert(symbol);
            _events.PublishOnUIThread(new ModelEvents(new List<object>(new
                object[] { "Add single ticker to Symbol: name = " + Ticker })));
        }

        public void LoadCsv()
        {
            BindableCollection<Symbol> tickers = YahooHelper.CsvToSymbolCollection(TickerFile);
            TickerCollection.Clear();
            TickerCollection.AddRange(tickers);
            _events.PublishOnUIThread(new ModelEvents(new List<object>(new
                object[] { "From CSV file loading: Count = " + 
                TickerCollection.Count.ToString() })));
        }

        public void AddTickers()
        {
            YahooHelper.SymbolInsert(TickerCollection);
            _events.PublishOnUIThread(new ModelEvents(new List<object>(new
                object[] { "Add Tickers to Symbol: Count = " + 
                TickerCollection.Count.ToString() })));
        }

    }
}
