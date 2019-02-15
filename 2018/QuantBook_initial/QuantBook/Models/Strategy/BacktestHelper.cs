using System;
using System.Data;
using System.Linq;
using Caliburn.Micro;
using System.ComponentModel;

namespace QuantBook.Models.Strategy
{
    public static class BacktestHelper
    {
        public static BindableCollection<PnLEntity> ComputeLongShortPnL(BindableCollection<SignalEntity> signalCollection, 
            double notional, double signalIn, double signalOut, StrategyTypeEnum strategyType, bool isReinvest)
        {
            BindableCollection<PnLEntity> pnlCollection = new BindableCollection<PnLEntity>();
            foreach (SignalEntity p in signalCollection)
            {
                double signal = p.Signal;
                if (strategyType == StrategyTypeEnum.Momentum)
                    signal = -p.Signal;
                pnlCollection.Add(new PnLEntity
                {
                    Ticker = p.Ticker,
                    Date = p.Date,
                    Price = p.Price,
                    Signal = signal,
                    TradeType = string.Empty,
                    DateIn = null,
                    PriceIn = null,
                    NumTrades = 0,
                    PnLPerTrade = 0,
                    PnLDaily = 0,
                    PnlCum = 0
                });
            }

            int numTrades =0;
            double pnlCum=0.0;
            double pnlCumHold = 0.0;
            string tradeType = string.Empty;
            double? priceIn = null;
            DateTime? dateIn = null; ;
            int ilong = 0;
            int ishort = 0;
            double shares = 0;
            double notionalIn = notional;

            for (int i = 1; i < pnlCollection.Count; i++)
            {
                bool isTrade = false;

                PnLEntity item0 = pnlCollection[i - 1];
                PnLEntity item1 = pnlCollection[i];

                double pnlDaily = 0.0;
                double pnlPerTrade = 0.0;

                //If in long position, compute daily PnL:
                if (tradeType == "LONG" && ilong > 0)
                {
                    pnlDaily = shares * (item1.Price - item0.Price);
                    pnlCum += pnlDaily;
                    item1.TradeType = "LONG";
                    item1.DateIn = dateIn;
                    item1.PriceIn = priceIn;
                    isTrade = true;
                }

                // Enter Long Position:
                if (String.IsNullOrEmpty(tradeType) && item0.Signal < -signalIn && !isTrade)
                {
                    tradeType = "LONG";
                    numTrades++;
                    dateIn = item1.Date;
                    priceIn = item1.Price;
                    shares = notional / item1.Price;
                    if (isReinvest)
                        shares = (notional + item0.PnlCum) / item1.Price;                    
                    ilong++;
                    item1.TradeType = "LONG";
                    item1.DateIn = dateIn;
                    item1.PriceIn = priceIn;
                    isTrade = true;
                }

                //Exit Long Position:
                if (tradeType == "LONG" && item0.Signal > -signalOut)
                {
                    pnlPerTrade = shares * (item1.Price - (double)priceIn);
                    numTrades++;                   
                    item1.TradeType = "LONG";
                    item1.DateIn = dateIn;
                    item1.PriceIn = priceIn;                    
                    tradeType = null;
                    priceIn = null;
                    shares = 0.0;
                    ilong = 0;
                    dateIn = null;
                    isTrade = true;
                }

                // If in Short position, compute daily PnL:
                if (tradeType == "SHORT" && ishort > 0)
                {
                    pnlDaily = -shares * (item1.Price - item0.Price);
                    pnlCum += pnlDaily;
                    item1.TradeType = "SHORT";
                    item1.DateIn = dateIn;
                    item1.PriceIn = priceIn;
                    isTrade = true;
                }

                // Enter Short Position:
                if (String.IsNullOrEmpty(tradeType) && item0.Signal > signalIn && !isTrade)
                {
                    tradeType = "SHORT";
                    numTrades++;
                    dateIn = item1.Date;
                    priceIn = item1.Price;
                    shares = notional / item1.Price;
                    if(isReinvest)
                        shares = (notional + item0.PnlCum) / item1.Price;
                    
                    ishort++;
                    item1.TradeType = "SHORT";
                    item1.DateIn = dateIn;
                    item1.PriceIn = priceIn;
                    isTrade = true;
                }

                //Exit Short Position:
                if (tradeType == "SHORT" && item0.Signal < signalOut)
                {
                    pnlPerTrade = -shares * (item1.Price - (double)priceIn);
                    numTrades++;                   
                    item1.TradeType = "SHORT";
                    item1.DateIn = dateIn;
                    item1.PriceIn = priceIn;
                    tradeType = null;
                    priceIn = null;
                    shares = 0.0;
                    ishort = 0;
                    dateIn = null;
                    isTrade = true;
                }

                // Compute pnl for holding position:
                double pnlDailyHold = notional * (item1.Price - item0.Price) / pnlCollection[0].Price;
                pnlCumHold = notional * (item1.Price - pnlCollection[0].Price) / pnlCollection[0].Price;

                item1.NumTrades = numTrades;
                item1.PnlCum = pnlCum;
                item1.PnLDaily = pnlDaily;
                item1.PnLPerTrade = pnlPerTrade;
                item1.PnLDailyHold = pnlDailyHold;
                item1.PnlCumHold = pnlCumHold;
            }

            if(strategyType == StrategyTypeEnum.Momentum)
            {
                foreach (PnLEntity p in pnlCollection)
                    p.Signal = -p.Signal;
            }

            return pnlCollection;
        }        


        public static DataTable GetYearlyPnL(BindableCollection<PnLEntity> pnl)
        {
            int n = pnl.Count;
            DateTime date0 = pnl[0].Date;
            DateTime date1 = pnl[n - 1].Date;
            DataTable res = new DataTable();
            res.Columns.Add("Ticker", typeof(string));
            res.Columns.Add("Period", typeof(string));
            res.Columns.Add("NumTrades", typeof(int));
            res.Columns.Add("PnL", typeof(double));
            res.Columns.Add("Sharpe", typeof(double));
            res.Columns.Add("PnLHold", typeof(double));
            res.Columns.Add("SharpeHold", typeof(double));
            

            DateTime date = new DateTime(date0.Year, 1, 1);
            while (date <= date1)
            {
                try
                {
                    DateTime firstDay = new DateTime(date.Year, 1, 1);
                    DateTime lastDay = new DateTime(date.Year, 12, 31);

                    System.Collections.Generic.List<PnLEntity> tp = (from p in pnl where p.Date >= firstDay && p.Date <= lastDay orderby p.Date select p).ToList();
                    BindableCollection<PnLEntity> tmp = new BindableCollection<PnLEntity>();
                    tmp.AddRange(tp);
                    int nn = tmp.Count;
                    if (nn > 0)
                    {
                        double pnl1 = tmp[nn - 1].PnlCum - tmp[0].PnlCum + tmp[0].PnLDaily;
                        double pnl2 = tmp[nn - 1].PnlCumHold - tmp[0].PnlCumHold + tmp[0].PnLDailyHold;
                        int numTrades = tmp[nn - 1].NumTrades - tmp[0].NumTrades;
                        System.Collections.Generic.List<PnLEntity> tmp1 = (from p in tmp where p.PnLDaily != 0 orderby p.Date select p).ToList();
                        if (tmp1.Count > 0)
                        {
                            double[] sp = GetSharpe(tmp);
                            res.Rows.Add(pnl[0].Ticker, date.Year.ToString(), numTrades, Math.Round(pnl1, 0), sp[0], Math.Round(pnl2, 0), sp[1]);
                        }
                    }
                    date = date.AddYears(1);
                }
                catch { }
            }
            double[] sp1 = GetSharpe(pnl);
            double sum = Math.Round(pnl[n - 1].PnlCum, 0);
            double sum1 = Math.Round(pnl[n - 1].PnlCumHold, 0);
            res.Rows.Add(pnl[0].Ticker, "Total", pnl[n - 1].NumTrades, sum, sp1[0],sum1,sp1[1] );
            return res;
        }

        public static double[] GetSharpe(BindableCollection<PnLEntity> pnl)
        {
            double[] res = new double[2];
            try
            {
                double avg = pnl.Average(x => x.PnLDaily);
                double std = pnl.StdDev(x => x.PnLDaily);
                double avg1 = pnl.Average(x => x.PnLDailyHold);
                double std1 = pnl.StdDev(x => x.PnLDailyHold);

                double sp = Math.Round(Math.Sqrt(252.0) * avg / std, 4);
                double sp1 = Math.Round(Math.Sqrt(252.0) * avg1 / std1, 4);
                res = new double[] { sp, sp1 };
            }
            catch { }
            return res;
        }

        public static DataTable GetDrawDown(BindableCollection<PnLEntity> pnlInput, double notional)
        {
            DataTable res = new DataTable();
            res.Columns.Add("Date", typeof(DateTime));
            res.Columns.Add("PnL", typeof(double));
            res.Columns.Add("Drawdown", typeof(double));
            res.Columns.Add("MaxDrawdown", typeof(double));
            res.Columns.Add("Drawup", typeof(double));
            res.Columns.Add("MaxDrawup", typeof(double));
            res.Columns.Add("PnLHold", typeof(double));
            res.Columns.Add("DrawdownHold", typeof(double));
            res.Columns.Add("MaxDrawdownHold", typeof(double));
            res.Columns.Add("DrawupHold", typeof(double));
            res.Columns.Add("MaxDrawupHold", typeof(double));

            for (int i = 0; i < pnlInput.Count - 2; i++)
                res.Rows.Add(pnlInput[i].Date, pnlInput[i].PnlCum + notional, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, pnlInput[i].PnlCumHold + notional);

            double max = 0;
            double min = 2.0 * notional;
            double maxa = 0;
            double mina = 2.0 * notional;
            foreach (DataRow row in res.Rows)
            {
                double pnl = Convert.ToDouble(row["PnL"]);
                double pnla = Convert.ToDouble(row["PnLHold"]);
                max = Math.Max(max, pnl);
                min = Math.Min(min, pnl);
                maxa = Math.Max(maxa, pnla);
                mina = Math.Min(mina, pnla);
                row["Drawdown"] = 100.0 * (max - pnl) / max;
                row["DrawdownHold"] = 100.0 * (maxa - pnla) / maxa;
                row["Drawup"] = 100.0 * (pnl - min) / pnl;
                row["DrawupHold"] = 100.0 * (pnla - mina) / pnla;
            }

            max = 0;
            maxa = 0;
            foreach (DataRow row in res.Rows)
            {
                double dd = Convert.ToDouble(row["Drawdown"]);
                double dda = Convert.ToDouble(row["DrawdownHold"]);
                max = Math.Max(max, dd);
                maxa = Math.Max(maxa, dda);
                row["MaxDrawdown"] = max;
                row["MaxDrawdownHold"] = maxa;
            }

            max = 0;
            maxa = 0;
            foreach (DataRow row in res.Rows)
            {
                double du = Convert.ToDouble(row["Drawup"]);
                double dua = Convert.ToDouble(row["DrawupHold"]);
                max = Math.Max(max, du);
                maxa = Math.Max(maxa, dua);
                row["MaxDrawup"] = max;
                row["MaxDrawupHold"] = maxa;
            }

            return res;
        }


        public static BindableCollection<PairPnLEntity> ComputePnLPair(BindableCollection<PairSignalEntity> signalCollection, double notional, double signalIn, double signalOut, 
            double hedgeRatio, out BindableCollection<PnLEntity> pnlEntity1)
        {
            double notional0 = notional / (1.0 + hedgeRatio);
            pnlEntity1 = new BindableCollection<PnLEntity>();
            BindableCollection<SignalEntity> signal1 = new BindableCollection<SignalEntity>();
            BindableCollection<SignalEntity> signal2 = new BindableCollection<SignalEntity>();
            foreach (PairSignalEntity p in signalCollection)
            {
                signal1.Add(new SignalEntity
                {
                    Ticker = p.Ticker1,
                    Date = p.Date,
                    Price = p.Price1,
                    Signal = p.Signal,
                });

                signal2.Add(new SignalEntity
                {
                    Ticker = p.Ticker2,
                    Date = p.Date,
                    Price = p.Price2,
                    Signal = -p.Signal,
                });
            }

            BindableCollection<PnLEntity> pnl1 = ComputeLongShortPnL(signal1, notional0, signalIn, signalOut, StrategyTypeEnum.MeanReversion, false);
            BindableCollection<PnLEntity> pnl2 = ComputeLongShortPnL(signal2, hedgeRatio * notional0, signalIn, signalOut, StrategyTypeEnum.MeanReversion, false);
            BindableCollection<PairPnLEntity> pnl = new BindableCollection<PairPnLEntity>();
            for (int i = 0; i < pnl1.Count; i++)
            {
                pnl.Add(new PairPnLEntity
                {
                    Ticker1 = signalCollection[0].Ticker1,
                    Ticker2 = signalCollection[0].Ticker2,
                    Date = pnl1[i].Date,
                    Price1 = pnl1[i].Price,
                    Price2 = pnl2[i].Price,
                    Signal = pnl1[i].Signal,
                    TradeType1 = pnl1[i].TradeType,
                    TradeType2 = pnl2[i].TradeType,
                    NumTrades = pnl1[i].NumTrades,
                    PnLDaily1 = pnl1[i].PnLDaily,
                    PnlCum1 = pnl1[i].PnlCum,
                    PnLDaily2 = pnl2[i].PnLDaily,
                    PnlCum2 = pnl2[i].PnlCum,
                    PnLPerTrade = pnl1[i].PnLPerTrade + pnl2[i].PnLPerTrade,
                    PnLDaily = pnl1[i].PnLDaily + pnl2[i].PnLDaily,
                    PnlCum = pnl1[i].PnlCum + pnl2[i].PnlCum,
                });
                pnlEntity1.Add(new PnLEntity
                {
                    Ticker = signalCollection[0].Ticker1 + "," + signalCollection[0].Ticker2,
                    Date = pnl1[i].Date,
                    Signal = pnl1[i].Signal,
                    NumTrades = pnl1[i].NumTrades,
                    PnLPerTrade = pnl1[i].PnLPerTrade + pnl2[i].PnLPerTrade,
                    PnLDaily = pnl1[i].PnLDaily + pnl2[i].PnLDaily,
                    PnlCum = pnl1[i].PnlCum + pnl2[i].PnlCum,
                    PnLDailyHold = pnl1[i].PnLDailyHold - pnl2[i].PnLDailyHold,
                    PnlCumHold = pnl1[i].PnlCumHold - pnl2[i].PnlCumHold,
                });
            }

            return pnl;
        }

    }













   

    public class PnLEntity
    {
        public string Ticker { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public double Signal { get; set; }
        public string TradeType { get; set; }
        public DateTime? DateIn { get; set; }
        public double? PriceIn { get; set; }
        public int NumTrades { get; set; }
        public double PnLPerTrade { get; set; }
        public double PnLDaily { get; set; }
        public double PnlCum { get; set; }
        public double PnLDailyHold { get; set; }
        public double PnlCumHold { get; set; }
    }

    public class PairPnLEntity
    {
        public string Ticker1 { get; set; }
        public string Ticker2 { get; set; }
        public DateTime Date { get; set; }
        public double Price1 { get; set; }
        public double Price2 { get; set; }
        public double Signal { get; set; }
        public string TradeType1 { get; set; }
        public string TradeType2 { get; set; }
        public int NumTrades { get; set; }
        public double PnLDaily1 { get; set; }
        public double PnlCum1 { get; set; }
        public double PnLDaily2 { get; set; }
        public double PnlCum2 { get; set; }
        public double PnLPerTrade { get; set; }
        public double PnLDaily { get; set; }
        public double PnlCum { get; set; }
    }
    

    public enum StrategyTypeEnum
    {
        MeanReversion = 0,
        Momentum = 1,
    }
}
