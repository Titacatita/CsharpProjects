using System;
using System.Collections.Generic;
using Caliburn.Micro;
using System.Data;

namespace QuantBook.Models.Strategy
{
    public static class OptimHelper
    {
        public static DataTable OptimSingleName(BindableCollection<SignalEntity> input, SignalTypeEnum signalType, StrategyTypeEnum strategyType, bool isReinvest, IEventAggregator events)
        {
            double notional = 10000.0;
            DataTable res = new DataTable();
            res.Columns.Add("Ticker", typeof(string));
            res.Columns.Add("MovingWindow", typeof(int));
            res.Columns.Add("SignalIn", typeof(double));
            res.Columns.Add("SignalOut", typeof(double));
            res.Columns.Add("NumTrades", typeof(int));
            res.Columns.Add("PnL", typeof(double));
            res.Columns.Add("Sharpe", typeof(double));
            int[] bars = new int[] { 5, 10, 15, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 150, 200, 250, 300 };
            double[] zin = new double[] { 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.2, 1.4, 1.6, 1.8, 2.0, 2.2, 2.4, 2.6 };
            if(signalType== SignalTypeEnum.RSI || signalType == SignalTypeEnum.WilliamR)
                zin = new double[] { 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.2, 1.4, 1.6, 1.8, 1.9};
            double[] zout = new double[] { 0, 0.1, 0.2, 0.3, 0.4 };

            List<object> objList = new List<object>();
            objList.Add("Starting...");
            objList.Add(0);
            objList.Add(bars.Length);
            objList.Add(0);
            events.PublishOnUIThread(new QuantBook.Models.ModelEvents(objList));

            for (int i = 0; i < bars.Length; i++)
            {
                BindableCollection<SignalEntity> signalCollection = SignalHelper.GetSignal(input, bars[i], signalType);
                for (int j = 0; j < zin.Length; j++)
                {
                    for (int k = 0; k < zout.Length; k++)
                    {
                        BindableCollection<PnLEntity> pnl = BacktestHelper.ComputeLongShortPnL(signalCollection, notional, zin[j], zout[k], strategyType, isReinvest);
                        if (pnl.Count > 0)
                        {
                            double[] sharpe = BacktestHelper.GetSharpe(pnl);
                            PnLEntity item = pnl[pnl.Count - 1];
                            if (Math.Abs(item.PnlCum) > 0)
                                res.Rows.Add(pnl[0].Ticker, bars[i], zin[j], zout[k], item.NumTrades, item.PnlCum, sharpe[0]);
                        }
                    }
                }

                objList[0] = string.Format("Total Runs = {0}, i = {1}", bars.Length, i + 1);
                objList[3] = i;
                events.PublishOnUIThread(new QuantBook.Models.ModelEvents(objList));
            }

            objList[0] = "Ready...";
            objList[1] = 0;
            objList[2] = 1;
            objList[3] = 0;
            events.PublishOnUIThread(new QuantBook.Models.ModelEvents(objList));

            res = ModelHelper.DatatableSort(res, "PnL DESC");
            return res;
        }


        public static DataTable OptimPairsTrading(string ticker1, string ticker2, DateTime startDate, DateTime endDate, double hedgeRatio, PairTypeEnum pairType, IEventAggregator events)
        {
            double notional = 10000.0;
            DataTable res = new DataTable();
            res.Columns.Add("Ticker1", typeof(string));
            res.Columns.Add("Ticker2", typeof(string));
            res.Columns.Add("MovingWindow", typeof(int));
            res.Columns.Add("SignalIn", typeof(double));
            res.Columns.Add("SignalOut", typeof(double));
            res.Columns.Add("NumTrades", typeof(int));
            res.Columns.Add("PnL", typeof(double));
            res.Columns.Add("Sharpe", typeof(double));
            
            int[] bars = new int[] { 5, 10, 15, 20, 30, 40, 50, 60, 70, 80, 90, 100, 120, 150, 200 };
            double[] zin = new double[] { 0.5, 0.6, 0.7, 0.8, 0.9, 1.0, 1.2, 1.4, 1.6, 1.8, 2.0, 2.2, 2.4, 2.6 };
            double[] zout = new double[] { 0, 0.1, 0.2, 0.3, 0.4 };

            List<object> objList = new List<object>();
            objList.Add("Starting...");
            objList.Add(0);
            objList.Add(bars.Length);
            objList.Add(0);
            events.PublishOnUIThread(new QuantBook.Models.ModelEvents(objList));
            double[] betas;
            BindableCollection<PairSignalEntity> data = SignalHelper.GetPairCorrelation(ticker1, ticker2, startDate, endDate, 100, out betas);

            for (int i = 0; i < bars.Length; i++)
            {
                BindableCollection<PairSignalEntity> dtPair = SignalHelper.GetPairSignal(data, bars[i], pairType);
              
                for (int j = 0; j < zin.Length; j++)
                {
                    for (int k = 0; k < zout.Length; k++)
                    {
                        BindableCollection<PnLEntity> pnl1;
                        BindableCollection<PairPnLEntity> pnl = BacktestHelper.ComputePnLPair(dtPair, notional, zin[j], zout[k], hedgeRatio, out pnl1);
                        double[]sp = BacktestHelper.GetSharpe(pnl1);
                        PnLEntity item = pnl1[pnl1.Count - 1];
                       
                        if (!double.IsNaN(sp[0]))
                            res.Rows.Add(ticker1, ticker2, bars[i], zin[j], zout[k], item.NumTrades, item.PnlCum, sp[0]);
                    }
                }

                objList[0] = string.Format("Total Runs = {0}, i = {1}", bars.Length, i + 1);
                objList[3] = i;
                events.PublishOnUIThread(new QuantBook.Models.ModelEvents(objList));
            }

            objList[0] = "Ready...";
            objList[1] = 0;
            objList[2] = 1;
            objList[3] = 0;
            events.PublishOnUIThread(new QuantBook.Models.ModelEvents(objList));

            res = ModelHelper.DatatableSort(res, "PnL DESC");
            return res;
        }
    }
}
