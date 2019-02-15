using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System;
using System.Linq;
using System.Collections.Generic;
using QuantBook.Models.Strategy;
using QuantBook.Models;

namespace QuantBook.Ch11
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SingleNameViewModel : Screen
    {
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public SingleNameViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "01. Single Name";
            LineSeriesCollection1 = new BindableCollection<Series>();
            LineSeriesCollection2 = new BindableCollection<Series>();
            SignalCollection = new BindableCollection<SignalEntity>();
            PnLCollection = new BindableCollection<PnLEntity>();
            StartDate = new DateTime(2010, 1, 3);
            EndDate = new DateTime(2015, 12, 31);
        }

        public BindableCollection<Series> LineSeriesCollection1 { get; set; }
        public BindableCollection<Series> LineSeriesCollection2 { get; set; }
        public BindableCollection<SignalEntity> SignalCollection { get; set; }
        public BindableCollection<PnLEntity> PnLCollection { get; set; }

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

        private int movingWindow = 30;
        public int MovingWindow
        {
            get { return movingWindow; }
            set
            {
                movingWindow = value;
                NotifyOfPropertyChange(() => MovingWindow);
            }
        }

        private IEnumerable<PriceTypeEnum> priceType;
        public IEnumerable<PriceTypeEnum> PriceType
        {
            get
            {
                return Enum.GetValues(typeof(PriceTypeEnum)).
                    Cast<PriceTypeEnum>();
            }
            set
            {
                priceType = value;
                NotifyOfPropertyChange(() => PriceType);
            }
        }

        private PriceTypeEnum selectedPriceType;
        public PriceTypeEnum SelectedPriceType
        {
            get { return selectedPriceType; }
            set
            {
                selectedPriceType = value;
                NotifyOfPropertyChange(() => SelectedPriceType);
            }
        }

        private IEnumerable<SignalTypeEnum> signalType;
        public IEnumerable<SignalTypeEnum> SignalType
        {
            get
            {
                return Enum.GetValues(typeof(SignalTypeEnum)).
                    Cast<SignalTypeEnum>();
            }
            set
            {
                signalType = value;
                NotifyOfPropertyChange(() => SignalType);
            }
        }

        private SignalTypeEnum selectedSignalType;
        public SignalTypeEnum SelectedSignalType
        {
            get { return selectedSignalType; }
            set
            {
                selectedSignalType = value;
                NotifyOfPropertyChange(() => SelectedSignalType);
            }
        }

        private IEnumerable<StrategyTypeEnum> strategyType;
        public IEnumerable<StrategyTypeEnum> StrategyType
        {
            get
            {
                return Enum.GetValues(typeof(StrategyTypeEnum)).
                    Cast<StrategyTypeEnum>();
            }
            set
            {
                strategyType = value;
                NotifyOfPropertyChange(() => StrategyType);
            }
        }

        private StrategyTypeEnum selectedStrategyType;
        public StrategyTypeEnum SelectedStrategyType
        {
            get { return selectedStrategyType; }
            set
            {
                selectedStrategyType = value;
                NotifyOfPropertyChange(() => SelectedStrategyType);
            }
        }


        private double notional = 10000;
        public double Notional
        {
            get { return notional; }
            set
            {
                notional = value;
                NotifyOfPropertyChange(() => Notional);
            }
        }

        private double signalIn = 2.0;
        public double SignalIn
        {
            get { return signalIn; }
            set
            {
                signalIn = value;
                NotifyOfPropertyChange(() => SignalIn);
            }
        }

        private double signalOut = 0;
        public double SignalOut
        {
            get { return signalOut; }
            set
            {
                signalOut = value;
                NotifyOfPropertyChange(() => SignalOut);
            }
        }

        private bool isReinvest = false;
        public bool IsReinvest
        {
            get { return isReinvest; }
            set
            {
                isReinvest = value;
                NotifyOfPropertyChange(() => IsReinvest);
            }
        }

        private string title1 = string.Empty;
        public string Title1
        {
            get { return title1; }
            set
            {
                title1 = value;
                NotifyOfPropertyChange(() => Title1);
            }
        }

        private string title2 = string.Empty;
        public string Title2
        {
            get { return title2; }
            set
            {
                title2 = value;
                NotifyOfPropertyChange(() => Title2);
            }
        }

        private string ylabel1 = string.Empty;
        public string YLabel1
        {
            get { return ylabel1; }
            set
            {
                ylabel1 = value;
                NotifyOfPropertyChange(() => YLabel1);
            }
        }

        private string ylabel2 = string.Empty;
        public string YLabel2
        {
            get { return ylabel2; }
            set
            {
                ylabel2 = value;
                NotifyOfPropertyChange(() => YLabel2);
            }
        }

        private DataTable yearlyPnLTable = null;
        public DataTable YearlyPnLTable
        {
            get { return yearlyPnLTable; }
            set
            {
                yearlyPnLTable = value;
                NotifyOfPropertyChange(() => YearlyPnLTable);
            }
        }


        public void GetSignalData()
        {
            PnLCollection.Clear();
            LineSeriesCollection1.Clear();
            LineSeriesCollection2.Clear();
            BindableCollection<SignalEntity> data = SignalHelper.GetStockData(Ticker, StartDate, EndDate, SelectedPriceType);
            BindableCollection<SignalEntity> signal = SignalHelper.GetSignal(data, MovingWindow, SelectedSignalType);
            SignalCollection.Clear();
            SignalCollection.AddRange(signal);
            AddSignalCharts();
        }

        DataTable drawdownTable = new DataTable();
        public void ComputePnL()
        {
            BindableCollection<PnLEntity> pnl = BacktestHelper.ComputeLongShortPnL(SignalCollection, Notional, SignalIn, SignalOut, SelectedStrategyType, IsReinvest);
            PnLCollection.Clear();
            PnLCollection.AddRange(pnl);
            DataTable dt = BacktestHelper.GetYearlyPnL(PnLCollection);
            YearlyPnLTable = dt;
            drawdownTable = BacktestHelper.GetDrawDown(PnLCollection, Notional);
            AddPnLCharts();
        }

        public void DrawdownStrategy()
        {
            Drawdown(true);
        }

        public void DrawdownHold()
        {
            Drawdown(false);
        }













        private void AddSignalCharts()
        {
            Title1 = string.Format("{0}: Stock Price (Price Type = {1}, Signal Type = {2})", Ticker, SelectedPriceType, SelectedSignalType);
            Title2 = string.Format("{0}: Signal (Price Type = {1}, Signal Type = {2})", Ticker, SelectedPriceType, SelectedSignalType);
            YLabel1 = "Stock Price";
            YLabel2 = "Signal";

            LineSeriesCollection1.Clear();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.MarkerStyle = MarkerStyle.Diamond;
            ds.MarkerSize = 4;
            foreach (SignalEntity p in SignalCollection)
                ds.Points.AddXY(p.Date, p.Price);
            ds.Name = "Original Price";
            LineSeriesCollection1.Add(ds);
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Color = System.Drawing.Color.Red;
            foreach (SignalEntity p in SignalCollection)
                ds.Points.AddXY(p.Date, p.PricePredicted);
            ds.Name = "Predicted Price";
            LineSeriesCollection1.Add(ds);
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Color = System.Drawing.Color.DarkGreen;
            foreach (SignalEntity p in SignalCollection)
                ds.Points.AddXY(p.Date, p.UpperBand);
            ds.Name = "Upper Band";
            LineSeriesCollection1.Add(ds);
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Color = System.Drawing.Color.DarkGreen;
            foreach (SignalEntity p in SignalCollection)
                ds.Points.AddXY(p.Date, p.LowerBand);
            ds.Name = "Lower Band";
            LineSeriesCollection1.Add(ds);

            LineSeriesCollection2.Clear();
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            foreach (SignalEntity p in SignalCollection)
                ds.Points.AddXY(p.Date, p.Signal);
            LineSeriesCollection2.Add(ds);
        }

        private void AddPnLCharts()
        {
            double pnl = Math.Round(YearlyPnLTable.Rows[YearlyPnLTable.Rows.Count - 1]["PnL"].To<double>(), 0);
            double sharpe = Math.Round(YearlyPnLTable.Rows[YearlyPnLTable.Rows.Count - 1]["Sharpe"].To<double>(), 3);
            int numTrades = YearlyPnLTable.Rows[YearlyPnLTable.Rows.Count - 1]["NumTrades"].To<int>();
            Title1 = string.Format("{0}: P&L (Total PnL = {1}, Sharpe = {2}, NumTrades = {3})", Ticker, pnl, sharpe, numTrades);
            YLabel1 = "Cummulated P&L";

            LineSeriesCollection1.Clear();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            foreach (PnLEntity p in PnLCollection)
                ds.Points.AddXY(p.Date, p.PnlCumHold);
            ds.Name = "PnL for Holding Position";
            LineSeriesCollection1.Add(ds);
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Color = System.Drawing.Color.Red;
            foreach (PnLEntity p in PnLCollection)
                ds.Points.AddXY(p.Date, p.PnlCum);
            ds.Name = "PnL";
            LineSeriesCollection1.Add(ds);

            Drawdown(true);
        }

        private void Drawdown(bool isStrategy)
        {
            YLabel2 = "Drawdown (%)";
            Series ds = new Series();
            if(isStrategy)
            {
                Title2 = string.Format("{0}: Drawdown for Signal Type = {1}", Ticker, SelectedSignalType);
                LineSeriesCollection2.Clear();
                ds = new Series();
                ds.ChartType = SeriesChartType.Line;
                foreach (DataRow row in drawdownTable.Rows)
                    ds.Points.AddXY(row["Date"].To<DateTime>(), row["Drawdown"].To<double>());
                ds.Name = "Drawndown";
                LineSeriesCollection2.Add(ds);
                ds = new Series();
                ds.ChartType = SeriesChartType.Line;
                ds.Color = System.Drawing.Color.Red;
                foreach (DataRow row in drawdownTable.Rows)
                    ds.Points.AddXY(row["Date"].To<DateTime>(), row["MaxDrawdown"].To<double>());
                ds.Name = "MaxDrawdown";
                LineSeriesCollection2.Add(ds);
            }
            else
            {
                Title2 = string.Format("{0}: Drawdown for Holding", Ticker);
                LineSeriesCollection2.Clear();
                ds = new Series();
                ds.ChartType = SeriesChartType.Line;
                foreach (DataRow row in drawdownTable.Rows)
                    ds.Points.AddXY(row["Date"].To<DateTime>(), row["DrawdownHold"].To<double>());
                ds.Name = "DrawndownHold";
                LineSeriesCollection2.Add(ds);
                ds = new Series();
                ds.ChartType = SeriesChartType.Line;
                ds.Color = System.Drawing.Color.Red;
                foreach (DataRow row in drawdownTable.Rows)
                    ds.Points.AddXY(row["Date"].To<DateTime>(), row["MaxDrawdownHold"].To<double>());
                ds.Name = "MaxDrawdownHold";
                LineSeriesCollection2.Add(ds);
            }

        }
    }
}
