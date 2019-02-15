using System;
using System.Linq;
using Caliburn.Micro;

namespace QuantBook.Models.DataModel
{
    public class Dal1
    {
        public string Ticker { get; set; }
        public string Ticker1 { get; set; }
        public string Ticker2 { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public BindableCollection<StockPrice> StockPrices = new BindableCollection<StockPrice>();
        public BindableCollection<PairStockPrice> PairStockPrices = new BindableCollection<PairStockPrice>();

        public void GetStockPrices()
        {
            StockPrices.Clear();
            using (MyDbEntities db = new MyDbEntities())     
            {                
                var query = (from s in db.Symbols
                            join p in db.Prices on s.SymbolID equals p.SymbolID
                            where s.Ticker == Ticker && p.Date >= DateStart && p.Date <= DateEnd
                            select new
                            {
                                s.Ticker,
                                p.Date,
                                p.PriceOpen,
                                p.PriceHigh,
                                p.PriceLow,
                                p.PriceClose,
                                p.PriceAdj,
                                p.Volume
                            }).OrderBy(x=>x.Date);
                            
                foreach(var p in query)
                {
                    StockPrices.Add(new StockPrice
                    {
                        Ticker = p.Ticker,
                        Date = (DateTime)p.Date,
                        PriceOpen =(double)p.PriceOpen,
                        PriceHigh = (double)p.PriceHigh,
                        PriceLow = (double)p.PriceLow,
                        PriceClose = (double)p.PriceClose,
                        PriceAdj = (double)p.PriceAdj,
                        Volume = (double)p.Volume
                    });
                }
            }
        }

        public void GetPairStockPrices()
        {
            PairStockPrices.Clear();
            using (MyDbEntities db = new MyDbEntities())
            {
                var query = (from s in db.Symbols
                             join p in db.Prices on s.SymbolID equals p.SymbolID
                             where (s.Ticker.Contains(Ticker1) || s.Ticker.Contains(Ticker2)) 
                                && p.Date >= DateStart && p.Date <= DateEnd
                             select new { s.Ticker, p.Date, p.PriceAdj }).GroupBy(x => x.Date).Select(y =>
                                     new
                                     {
                                         Date = y.Key,
                                         Price1 = y.Where(z => z.Ticker == Ticker1).Select(z => z.PriceAdj),
                                         Price2 = y.Where(z => z.Ticker == Ticker2).Select(z => z.PriceAdj)
                                     }).OrderBy(xx => xx.Date);

                foreach (var p in query)
                {
                    PairStockPrices.Add(new PairStockPrice
                    {
                        Date = (DateTime)p.Date,
                        Price1 = (double)p.Price1.First(),
                        Price2 = (double)p.Price2.First()
                    });
                }
            }
        }
    }

    public class StockPrice
    {
        public string Ticker { get; set; }
        public DateTime Date { get; set; }
        public double PriceOpen { get; set; }
        public double PriceHigh { get; set; }
        public double PriceLow { get; set; }
        public double PriceClose { get; set; }
        public double PriceAdj { get; set; }
        public double Volume { get; set; }
    }

    public class PairStockPrice
    {
        public DateTime Date { get; set; }
        public double Price1 { get; set; }
        public double Price2 { get; set; }
    }
}
