using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Data;
using QuantBook.Models.DataModel;

namespace QuantBook.Models.Strategy
{
    public static class SignalHelper
    {
        #region Single Name
        public static BindableCollection<SignalEntity> GetSignal(BindableCollection<SignalEntity> input, int movingWindow, SignalTypeEnum signalType)
        {
            BindableCollection<SignalEntity> res = new BindableCollection<SignalEntity>();
            switch(signalType)
            {
                case SignalTypeEnum.MovingAverage:
                    res = MovingAverage(input, movingWindow);
                    break;
                case SignalTypeEnum.LinearRegression:
                    res = LinearRegression(input, movingWindow);
                    break;
                case SignalTypeEnum.RSI:
                    res = RSINormalized(input, movingWindow);
                    break;
                case SignalTypeEnum.WilliamR:
                    res = WilliamsRNormalized(input, movingWindow);
                    break;
            }
            return res;
        }

        private static BindableCollection<SignalEntity> MovingAverage(BindableCollection<SignalEntity> signalCollection, int movingWindow)
        {
            BindableCollection<SignalEntity> res = new BindableCollection<SignalEntity>();
            for (int i = movingWindow - 1; i < signalCollection.Count; i++)
            {
                List<SignalEntity> tmp = new List<SignalEntity>();
                for (int j = i - movingWindow + 1; j <= i; j++)
                {
                    tmp.Add(signalCollection[j]);
                }
                double avg = tmp.Average(x => x.Price);
                double std = tmp.StdDev(x => x.Price);
                double price = signalCollection[i].Price;
                double zscore = (price - avg) / std;
                res.Add(new SignalEntity
                {
                    Ticker = signalCollection[i].Ticker,
                    Date = signalCollection[i].Date,
                    Price = signalCollection[i].Price,
                    PricePredicted = avg,
                    UpperBand = avg + 2.0 * std,
                    LowerBand = avg - 2.0 * std,
                    Signal = zscore
                });
            }
            return res;
        }

        private static BindableCollection<SignalEntity> LinearRegression(BindableCollection<SignalEntity> signalCollection, int movingWindow)
        {
            BindableCollection<SignalEntity> res = new BindableCollection<SignalEntity>();
            for (int i = movingWindow - 1; i < signalCollection.Count; i++)
            {
                List<double> xa = new List<double>();
                List<double> ya = new List<double>();
                List<SignalEntity> tmp = new List<SignalEntity>();
                for (int j = i - movingWindow + 1; j <= i; j++)
                {
                    xa.Add(1.0*j);
                    ya.Add(signalCollection[j].Price);
                    tmp.Add(signalCollection[j]);
                }

                AnalysisModel.SimpleLinearResult lr = AnalysisModel.LinearAnalysisHelper.GetSimpleRegression(xa, ya);
                double price = signalCollection[i].Price;
                double priceLR = lr.Alpha + lr.Beta * xa[xa.Count - 1];
                double std = tmp.StdDev(x => x.Price);
                double zscore = (price - priceLR) / std;
                res.Add(new SignalEntity
                {
                    Ticker = signalCollection[i].Ticker,
                    Date = signalCollection[i].Date,
                    Price = signalCollection[i].Price,
                    PricePredicted = priceLR,
                    UpperBand = priceLR + 2.0 * std,
                    LowerBand = priceLR - 2.0 * std,
                    Signal = zscore
                });
            }
            return res;
        }

        private static BindableCollection<SignalEntity> MACDNormalized(BindableCollection<SignalEntity> signalCollection, int movingWindow)
        {
            BindableCollection<SignalEntity> signalCollection1 = new BindableCollection<SignalEntity>();
            foreach (SignalEntity p in signalCollection)
                signalCollection1.Add(p);
            
            int window1 = (int)Math.Floor(0.5 * movingWindow);
            List<double> d1 = new List<double>();
            List<double> d2 = new List<double>();
          
            double[] xa1 = new double[window1];
            double[] xa2 = new double[movingWindow];
            for (int i = 0; i < window1; i++)
                xa1[i] = signalCollection1[i].Price;
            for (int i = 0; i < movingWindow; i++)
                xa2[i] = signalCollection1[i].Price;
            double avg1 = xa1.Average();
            double avg2 = xa2.Average();           
            for (int i = window1 - 1; i < signalCollection1.Count; i++)
            {
                if (i == window1 - 1)
                    d1.Add(avg1);
                else
                {
                    double ema0 = d1[d1.Count - 1];
                    double ema = signalCollection1[i].Price * 2.0 / (window1 + 1.0) + ema0 * (1.0 - 2.0 / (window1 + 1.0));
                    d1.Add(ema);
                }
            }            
            for (int i = movingWindow - 1; i < signalCollection1.Count; i++)
            {
                if (i == movingWindow - 1)
                    d2.Add(avg2);
                else
                {
                    double ema0 = d2[d2.Count - 1];
                    double ema = signalCollection1[i].Price * 2.0 / (movingWindow + 1.0) + ema0 * (1.0 - 2.0 / (movingWindow + 1.0));
                    d2.Add(ema);
                }
            }
            for (int i = movingWindow - 1; i < signalCollection1.Count; i++)
            {
                signalCollection1[i].UpperBand = d1[i - movingWindow + 1 + window1];
                signalCollection1[i].LowerBand = d2[i - movingWindow + 1];
            }

            IOrderedEnumerable<SignalEntity> qry = (from q in signalCollection1 where Math.Abs(q.LowerBand) > 0 select q).OrderBy(x => x.Date);
            List<SignalEntity> res1 = new List<SignalEntity>();
            foreach(SignalEntity p in qry)
            {
                res1.Add(p);
            }

            BindableCollection<SignalEntity> res = new BindableCollection<SignalEntity>();

            for (int i = movingWindow - 1; i < res1.Count; i++)
            {

                List<double> ema = new List<double>();
                for (int j = i - movingWindow + 1; j <= i; j++)
                {
                    ema.Add(res1[j].LowerBand);
                }
                double[] avgStd = ModelHelper.GetAvgStd(ema);
                double signal = (res1[i].UpperBand - res1[i].LowerBand) / avgStd[1];
                res.Add(new SignalEntity
                {
                    Ticker =res1[i].Ticker,
                    Date = res1[i].Date,
                    Price = res1[i].Price,
                    PricePredicted = res1[i].Price,
                    UpperBand = res1[i].UpperBand,
                    LowerBand = res1[i].LowerBand,
                    Signal = signal,
                });
            }
            return res;
        }

        private static BindableCollection<SignalEntity> RSINormalized(BindableCollection<SignalEntity> signalCollection, int movingWindow)
        {
            BindableCollection<SignalEntity> res = new BindableCollection<SignalEntity>();
            for (int i = movingWindow - 1; i < signalCollection.Count; i++)
            {
                List<double> tmp = new List<double>();
                for (int j = i - movingWindow + 1; j <= i; j++)
                {
                    tmp.Add(signalCollection[j].Price);
                }
                double rsi = CalculateRSI(tmp);
                res.Add(new SignalEntity
                {
                    Ticker = signalCollection[i].Ticker,
                    Date = signalCollection[i].Date,
                    Price = signalCollection[i].Price,
                    PricePredicted = signalCollection[i].Price,
                    UpperBand = signalCollection[i].Price,
                    LowerBand = signalCollection[i].Price,
                    Signal = rsi,
                });
            }
            return res;
        }

        private static double CalculateRSI(List<double> prices)
        {
            double sumGain = 0;
            double sumLoss = 0;
            for (int i = 1; i < prices.Count; i++)
            {
                double diff = prices[i] - prices[i - 1];
                if (diff >= 0)
                    sumGain += diff;
                else
                    sumLoss -= diff;
            }
            double rsi = 0;
            if (sumGain == 0)
                rsi = 0;
            else if (Math.Abs(sumLoss) < 1.0e-15)
                rsi = 100.0;
            else
                rsi = sumGain / sumLoss;
            rsi = 100.0 - 100.0 / (1.0 + rsi);
            return (rsi - 50.0) / 25.0;
        }

        private static BindableCollection<SignalEntity> WilliamsRNormalized(BindableCollection<SignalEntity> signalCollection, int movingWindow)
        {
            BindableCollection<SignalEntity> res = new BindableCollection<SignalEntity>();
            for (int i = movingWindow - 1; i < signalCollection.Count; i++)
            {
                List<SignalEntity> tmp = new List<SignalEntity>();
                for (int j = i - movingWindow + 1; j <= i; j++)
                {
                    tmp.Add(signalCollection[j]);
                }
                double price = signalCollection[i].Price;
                double high = tmp.Max(x => x.Price);
                double low = tmp.Min(x => x.Price);
                double wr = -4.0 * (high - price) / (high - low) + 2.0;
                
                res.Add(new SignalEntity
                {
                    Ticker = signalCollection[i].Ticker,
                    Date = signalCollection[i].Date,
                    Price = signalCollection[i].Price,
                    PricePredicted = signalCollection[i].Price,
                    UpperBand = signalCollection[i].Price,
                    LowerBand = signalCollection[i].Price,
                    Signal = wr,
                });
            }
            return res;
        }










        public static BindableCollection<SignalEntity> GetStockData(string ticker, DateTime startDate, DateTime endDate, PriceTypeEnum priceType)
        {
            BindableCollection<SignalEntity> res = new BindableCollection<SignalEntity>();
            res = GetStockDataQuandl(ticker, startDate, endDate, priceType);
            if (res.Count < 1)
                res = GetStockDataYahoo(ticker, startDate, endDate, priceType);
            if (res.Count < 1)
                res = GetStockDataGoogle(ticker, startDate, endDate, priceType);
            return res;
        }

        private static BindableCollection<SignalEntity>  GetStockDataQuandl(string ticker, DateTime startDate, DateTime endDate, PriceTypeEnum priceType)
        {
            BindableCollection<StockData> data = Dal.GetStockData(ticker, startDate, endDate, DataSourceEnum.Quandl);
            BindableCollection<SignalEntity> res = new BindableCollection<SignalEntity>();

            foreach (StockData p in data)
            {
                double price = (double)p.AdjClose;
                switch (priceType)
                {
                    case PriceTypeEnum.Close:
                        price = (double)p.AdjClose;
                        break;
                    case PriceTypeEnum.Average:
                        price =(double)(p.AdjOpen + p.AdjHigh + p.AdjLow + p.AdjClose) / 4.0;
                        break;
                    case PriceTypeEnum.TypicalPrice:
                        price = (double)(p.AdjHigh + p.AdjLow + p.AdjClose) / 3.0;
                        break;
                }
                res.Add(new SignalEntity
                {
                    Ticker = p.Ticker,
                    Date = (DateTime)p.Date,
                    Price = price,
                });
            }
            return res;
        }

        private static BindableCollection<SignalEntity> GetStockDataYahoo(string ticker, DateTime startDate, DateTime endDate, PriceTypeEnum priceType)
        {
            BindableCollection<StockData> data = Dal.GetStockData(ticker, startDate, endDate, DataSourceEnum.Yahoo);
            BindableCollection<SignalEntity> res = new BindableCollection<SignalEntity>();

            foreach (StockData p in data)
            {
                double price = (double)p.AdjClose;
                switch (priceType)
                {
                    case PriceTypeEnum.Close:
                        price = (double)p.AdjClose;
                        break;
                    case PriceTypeEnum.Average:
                        price = (double)(p.Open + p.High + p.Low + p.Close) / 4.0;
                        break;
                    case PriceTypeEnum.TypicalPrice:
                        price = (double)(p.High + p.Low + p.Close) / 3.0;
                        break;
                }
                res.Add(new SignalEntity
                {
                    Ticker = p.Ticker,
                    Date = (DateTime)p.Date,
                    Price = price,
                });
            }
            return res;
        }

        private static BindableCollection<SignalEntity> GetStockDataGoogle(string ticker, DateTime startDate, DateTime endDate, PriceTypeEnum priceType)
        {
            BindableCollection<StockData> data = Dal.GetStockData(ticker, startDate, endDate, DataSourceEnum.Google);
            BindableCollection<SignalEntity> res = new BindableCollection<SignalEntity>();

            foreach (StockData p in data)
            {
                double price = (double)p.Close;
                switch (priceType)
                {
                    case PriceTypeEnum.Close:
                        price = (double)p.Close;
                        break;
                    case PriceTypeEnum.Average:
                        price = (double)(p.Open + p.High + p.Low + p.Close) / 4.0;
                        break;
                    case PriceTypeEnum.TypicalPrice:
                        price = (double)(p.High + p.Low + p.Close) / 3.0;
                        break;
                }
                res.Add(new SignalEntity
                {
                    Ticker = p.Ticker,
                    Date = (DateTime)p.Date,
                    Price = price,
                });
            }
            return res;
        }

        public static BindableCollection<SignalEntity> GetStockDataFile(string fileName, DateTime startDate, DateTime endDate, PriceTypeEnum priceType)
        {
            string ticker = fileName.Substring(0, 6);
            BindableCollection<StockData> data = Dal.GetStockDataFromFile(fileName, startDate, endDate);
            BindableCollection<SignalEntity> res = new BindableCollection<SignalEntity>();

            foreach (StockData p in data)
            {
                double price = (double)p.Close;
                switch (priceType)
                {
                    case PriceTypeEnum.Close:
                        price = (double)p.Close;
                        break;
                    case PriceTypeEnum.Average:
                        price = (double)(p.Open + p.High + p.Low + p.Close) / 4.0;
                        break;
                    case PriceTypeEnum.TypicalPrice:
                        price = (double)(p.High + p.Low + p.Close) / 3.0;
                        break;
                }
                res.Add(new SignalEntity
                {
                    Ticker = p.Ticker,
                    Date = (DateTime)p.Date,
                    Price = price,
                });
            }
            return res;
        }

        #endregion Single Name


       



        #region Pair Trading

        public static BindableCollection<PairSignalEntity> GetPairSignal(BindableCollection<PairSignalEntity> data, int movingWindow, PairTypeEnum pairType)
        {
            BindableCollection<PairSignalEntity> res = new BindableCollection<PairSignalEntity>();
            if (pairType == PairTypeEnum.PriceRatio)
                res = GetPairPriceRatioSignal(data, movingWindow);
            else if (pairType == PairTypeEnum.Spread)
                res = GetPairSpreadSignal(data, movingWindow);
            return res;
        }

       
        private static BindableCollection<PairSignalEntity> GetPairSpreadSignal(BindableCollection<PairSignalEntity> data, int movingWindow)
        {
            BindableCollection<PairSignalEntity> res = new BindableCollection<PairSignalEntity>();
            if (data.Count < 1)
                return res;

            for (int i = movingWindow - 1; i < data.Count; i++)
            {
                List<double> xa = new List<double>();
                List<double> ya = new List<double>();
                for (int j = i - movingWindow + 1; j <= i; j++)
                {
                    xa.Add(data[j].Price1);
                    ya.Add(data[j].Price2);
                }

                AnalysisModel.SimpleLinearResult lr = AnalysisModel.LinearAnalysisHelper.GetSimpleRegression(ya, xa);
                double price1 = xa[xa.Count - 1];
                double spread = price1 - lr.Beta * ya[ya.Count - 1];
                double[] spreads = new double[xa.Count];
                for (int ii = 0; ii < xa.Count; ii++)
                    spreads[ii] = xa[ii] - lr.Beta * ya[ii];
                double[] avgStd = ModelHelper.GetAvgStd(spreads);
                double zscore = (spread - avgStd[0]) / avgStd[1];
                res.Add(new PairSignalEntity
                {
                    Ticker1 = data[i].Ticker1,
                    Ticker2 = data[i].Ticker2,
                    Date = (DateTime)data[i].Date,
                    Price1 = xa[xa.Count - 1],
                    Price2 = ya[ya.Count - 1],
                    Correlation = data[i].Correlation,
                    Beta = data[i].Beta,
                    Spread = spread,
                    Signal = zscore,
                });
            }
            return res;
        }


        private static BindableCollection<PairSignalEntity> GetPairPriceRatioSignal(BindableCollection<PairSignalEntity> data, int movingWindow)
        {
            foreach (PairSignalEntity p in data)
                p.Spread = p.Price1 / p.Price2;
            BindableCollection<PairSignalEntity> res = new BindableCollection<PairSignalEntity>();
            if (data.Count < 1)
                return res;

            for (int i = movingWindow - 1; i < data.Count; i++)
            {
                List<PairSignalEntity> tmp = new List<PairSignalEntity>();
                for (int j = i - movingWindow + 1; j <= i; j++)
                {
                    tmp.Add(data[j]);
                }

                double avg = tmp.Average(x => x.Spread);
                double std = tmp.StdDev(x => x.Spread);
                double zscore = (data[i].Spread - avg) / std;
                
                res.Add(new PairSignalEntity
                {
                    Ticker1 = data[i].Ticker1,
                    Ticker2 = data[i].Ticker2,
                    Date = (DateTime)data[i].Date,
                    Price1 = data[i].Price1,
                    Price2 = data[i].Price2,
                    Spread = data[i].Spread,
                    Correlation = data[i].Correlation,
                    Beta = data[i].Beta,
                    Signal = zscore,
                });
            }
            return res;
        }


        public static BindableCollection<PairSignalEntity> GetPairCorrelation(string ticker1, string ticker2, DateTime startDate, DateTime endDate, int correlWindow, out double[] betas)
        {
            betas = new double[4];
            BindableCollection<PairStockData> data = Dal.GetPairStockData(ticker1, ticker2, startDate.AddDays(-2 * correlWindow), endDate, "AdjClose", DataSourceEnum.Yahoo);
            if (data.Count < 1)
                data = Dal.GetPairStockData(ticker1, ticker2, startDate.AddDays(-2 * correlWindow), endDate, "Close", DataSourceEnum.Google);
            BindableCollection<PairSignalEntity> res = new BindableCollection<PairSignalEntity>();
            if (data.Count < 1)
                return res;

            for (int i = correlWindow - 1; i < data.Count; i++)
            {
                List<double> xa = new List<double>();
                List<double> ya = new List<double>();
                for (int j = i - correlWindow + 1; j <= i; j++)
                {
                    xa.Add((double)data[j].Price1);
                    ya.Add((double)data[j].Price2);
                }

                double correl = ModelHelper.GetCorrelation(xa.ToArray(), ya.ToArray());
                AnalysisModel.SimpleLinearResult lr = AnalysisModel.LinearAnalysisHelper.GetSimpleRegression(ya.ToArray(), xa.ToArray());
                double price1 = (double)data[i].Price1;
                double price2 = (double)data[i].Price2;
                DateTime date = (DateTime)data[i].Date;

                if (date >= startDate && date <= endDate)
                {
                    res.Add(new PairSignalEntity
                    {
                        Ticker1 = ticker1,
                        Ticker2 = ticker2,
                        Date = date,
                        Price1 = price1,
                        Price2 = price2,
                        Correlation = correl,
                        Beta = lr.Beta,
                    });
                }
            }

            betas[0] = res.Average(x => x.Beta);
            betas[2] = res.Average(x => x.Correlation);
            double[] xx = new double[res.Count];
            double[] yy = new double[res.Count];
            for (int i = 0; i < res.Count; i++)
            {
                xx[i] = res[i].Price1;
                yy[i] = res[i].Price2;
            }
            betas[1] = AnalysisModel.LinearAnalysisHelper.GetSimpleRegression(yy, xx).Beta;
            betas[3] = ModelHelper.GetCorrelation(xx, yy);

            return res;
        }

       
        #endregion Pair Trading
    }




















    public class SignalEntity
    {
        public string Ticker { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }       
        public double PricePredicted { get; set; }
        public double UpperBand { get; set; }
        public double LowerBand { get; set; }
        public double Signal { get; set; }
    }

    public class PairSignalEntity
    {
        public string Ticker1 { get; set; }
        public string Ticker2 { get; set; }
        public DateTime Date { get; set; }
        public double Price1 { get; set; }
        public double Price2 { get; set; }
        public double Correlation { get; set; }
        public double Spread { get; set; }
        public double Beta { get; set; }
        public double Signal { get; set; }
    }


    public enum SignalTypeEnum
    {
        MovingAverage = 0,
        LinearRegression = 1,
        RSI = 2,
        WilliamR = 3,
    }

    public enum PriceTypeEnum
    {
        Close = 0,
        Average = 1,
        TypicalPrice,
    }

    public enum PairTypeEnum
    {
        PriceRatio = 0,
        Spread = 1,
    }
}
