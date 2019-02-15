using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models.DataModel;
using QuantBook.Models.Yahoo;
using QuantBook.Models;

namespace QuantBook.Ch04
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class AddPricesViewModel : Screen
    {
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public AddPricesViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "03. Add Prices";
            StartDate = Convert.ToDateTime("1/1/2010");
            EndDate = Convert.ToDateTime("1/1/2015");
            PriceCollection = new BindableCollection<Price>();
            TickerCollection = new BindableCollection<Symbol>();
            SymbolID = 1;
        }

        public BindableCollection<Price> PriceCollection { get; set; }
        public BindableCollection<Symbol> TickerCollection {get;set;}

        private int symbolId;
        public int SymbolID
        {
            get { return symbolId; }
            set
            {
                symbolId = value;
                NotifyOfPropertyChange(() => SymbolID);
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

        public void GetPrice()
        {
            string tk = YahooHelper.IdToTicker(SymbolID);
            BindableCollection<Price> prices = YahooHelper.GetYahooHistStockData(SymbolID, tk, StartDate, EndDate);
            PriceCollection.Clear();
            PriceCollection.AddRange(prices);
            _events.PublishOnUIThread(new ModelEvents(new List<object>(new  object[] {string.Format("Get Price From Yahoo: Ticker = {0},  Count = {1}",
                tk, PriceCollection.Count) })));
        }

        public void SavePrice()
        {
            if(PriceCollection.Count>0)
            {
                YahooHelper.PriceInsert(PriceCollection);
                _events.PublishOnUIThread(new ModelEvents(new List<object>(new 
                    object[] { "Save Price: Count = " + PriceCollection.Count.ToString() })));
            }
        }

        public async void GetPrices()
        {
            await Task.Run(() =>
                {
                    TickerCollection.Clear();
                    BindableCollection<Symbol> tks = YahooHelper.GetTickers();
                    TickerCollection.AddRange(tks);

                    List<object> objs = new List<object>();
                    objs.Add("Get  data from Yahoo:");
                    objs.Add(0);
                    objs.Add(TickerCollection.Count);
                    objs.Add(0);

                    int count = 0;
                    foreach (Symbol tc in TickerCollection)
                    {
                        BindableCollection<Price> price = YahooHelper.GetYahooHistStockData(tc.SymbolID, tc.Ticker, StartDate, EndDate);
                        if (price.Count > 0)
                        {
                            YahooHelper.PriceInsert(price);
                            objs[0] = string.Format("Get data from Yahoo: Ticker = {0}, Count = {1}, Records = {2}", tc.Ticker, count, price.Count);
                            objs[3] = count;
                            _events.PublishOnUIThread(new ModelEvents(objs));
                        }
                        count++;
                    }
                });
        }
    }
}
