using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System;
using System.Linq;
using System.Collections.Generic;
using QuantBook.Models.Strategy;
using QuantBook.Models;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace QuantBook.Ch11
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class PairsOptimViewModel : Screen
    {
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public PairsOptimViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "04. Pairs Optimization";
            LineSeriesCollection1 = new BindableCollection<Series>();
            LineSeriesCollection2 = new BindableCollection<Series>();
            LineSeriesCollection3 = new BindableCollection<Series>();
            LineSeriesCollection4 = new BindableCollection<Series>();
            LineSeriesCollection5 = new BindableCollection<Series>();
            LineSeriesCollection6 = new BindableCollection<Series>();
            PairCollection = new BindableCollection<PairSignalEntity>();
            PnLCollection = new BindableCollection<PairPnLEntity>();
            StartDate = new DateTime(2010, 1, 3);
            EndDate = new DateTime(2015, 12, 31);
        }

        public BindableCollection<Series> LineSeriesCollection1 { get; set; }
        public BindableCollection<Series> LineSeriesCollection2 { get; set; }
        public BindableCollection<Series> LineSeriesCollection3 { get; set; }
        public BindableCollection<Series> LineSeriesCollection4 { get; set; }
        public BindableCollection<Series> LineSeriesCollection5 { get; set; }
        public BindableCollection<Series> LineSeriesCollection6 { get; set; }

        public BindableCollection<PairSignalEntity> PairCollection { get; set; }
        public BindableCollection<PairPnLEntity> PnLCollection { get; set; }

        private string ticker1 = "QQQ";
        public string Ticker1
        {
            get { return ticker1; }
            set
            {
                ticker1 = value;
                NotifyOfPropertyChange(() => Ticker1);
            }
        }

        private string ticker2 = "SPY";
        public string Ticker2
        {
            get { return ticker2; }
            set
            {
                ticker2 = value;
                NotifyOfPropertyChange(() => Ticker2);
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

        private int correlWindow = 100;
        public int CorrelWindow
        {
            get { return correlWindow; }
            set
            {
                correlWindow = value;
                NotifyOfPropertyChange(() => CorrelWindow);
            }
        }

        private IEnumerable<PairTypeEnum> pairType;
        public IEnumerable<PairTypeEnum> PairType
        {
            get
            {
                return Enum.GetValues(typeof(PairTypeEnum)).
                    Cast<PairTypeEnum>();
            }
            set
            {
                pairType = value;
                NotifyOfPropertyChange(() => PairType);
            }
        }

        private PairTypeEnum selectedPairType;
        public PairTypeEnum SelectedPairType
        {
            get { return selectedPairType; }
            set
            {
                selectedPairType = value;
                NotifyOfPropertyChange(() => SelectedPairType);
            }
        }

        private double hedgeRatio = 1.0;
        public double HedgeRatio
        {
            get { return hedgeRatio; }
            set
            {
                hedgeRatio = value;
                NotifyOfPropertyChange(() => HedgeRatio);
            }
        }

        private DataTable optimTable = new DataTable();
        public DataTable OptimTable
        {
            get { return optimTable; }
            set
            {
                optimTable = value;
                NotifyOfPropertyChange(() => OptimTable);
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

        private string title3 = string.Empty;
        public string Title3
        {
            get { return title3; }
            set
            {
                title3 = value;
                NotifyOfPropertyChange(() => Title3);
            }
        }
        private string title4 = string.Empty;
        public string Title4
        {
            get { return title4; }
            set
            {
                title4 = value;
                NotifyOfPropertyChange(() => Title4);
            }
        }
        private string title5 = string.Empty;
        public string Title5
        {
            get { return title5; }
            set
            {
                title5 = value;
                NotifyOfPropertyChange(() => Title5);
            }
        }
        private string title6 = string.Empty;
        public string Title6
        {
            get { return title6; }
            set
            {
                title6 = value;
                NotifyOfPropertyChange(() => Title6);
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


        public void GetData()
        {
            PnLCollection.Clear();
            OptimTable = new DataTable();
            LineSeriesCollection1.Clear();
            LineSeriesCollection2.Clear();
            LineSeriesCollection3.Clear();
            LineSeriesCollection4.Clear();
            LineSeriesCollection5.Clear();
            LineSeriesCollection6.Clear();
            BindableCollection<PairSignalEntity> pair = SignalHelper.GetPairCorrelation(Ticker1, Ticker2, StartDate, EndDate, CorrelWindow, out betas);
            AddPriceCharts(pair);
        }

        DataTable drawdownTable = new DataTable();
        public async void StartOptim()
        {
            await Task.Run(() =>
            {
                PnLCollection.Clear();
                OptimTable = new DataTable();
                LineSeriesCollection4.Clear();
                LineSeriesCollection5.Clear();
                LineSeriesCollection6.Clear();
                DataTable dt = OptimHelper.OptimPairsTrading(Ticker1, Ticker2, StartDate, EndDate, HedgeRatio, SelectedPairType, _events);
                OptimTable = dt;
            });
        }

        double[] betas;
        public async void SelectedCellChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    IList<DataGridCellInfo> selectedCells = e.AddedCells;
                    DataGridCellInfo di = selectedCells[selectedCells.Count - 1];
                    DataRowView dvr = (DataRowView)di.Item;
                    BindableCollection<PairSignalEntity> data = SignalHelper.GetPairCorrelation(Ticker1, Ticker2, StartDate, EndDate, CorrelWindow, out betas);
                    int movingWindow = dvr["MovingWindow"].To<int>();
                    BindableCollection<PairSignalEntity> signal = SignalHelper.GetPairSignal(data, movingWindow, SelectedPairType);
                    PairCollection.Clear();
                    PairCollection.AddRange(signal);
                  
                    double signalIn = dvr["SignalIn"].To<double>();
                    double signalOut = dvr["SignalOut"].To<double>();
                    BindableCollection<PnLEntity> pnl1;
                    BindableCollection<PairPnLEntity> pnl = BacktestHelper.ComputePnLPair(signal, 10000.0, signalIn, signalOut, HedgeRatio, out pnl1);
                    PnLCollection.Clear();
                    PnLCollection.AddRange(pnl);
                    DataTable dt = BacktestHelper.GetYearlyPnL(pnl1);
                    YearlyPnLTable = dt;
                    drawdownTable = BacktestHelper.GetDrawDown(pnl1, 10000.0);
                    AddSignalChart();
                    AddPnLCharts();
                }
                catch { }
            });
        }


        private void AddPriceCharts(BindableCollection<PairSignalEntity> pair)
        {
            Title1 = string.Format("{0}: Stock Price", Ticker1 + ", " + Ticker2);
            LineSeriesCollection1.Clear();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            foreach (PairSignalEntity p in pair)
                ds.Points.AddXY(p.Date, p.Price1);
            ds.Name = Ticker1 + " Price";
            LineSeriesCollection1.Add(ds);
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Color = System.Drawing.Color.Red;
            foreach (PairSignalEntity p in pair)
                ds.Points.AddXY(p.Date, p.Price2);
            ds.Name = Ticker2 + " Price";
            ds.YAxisType = AxisType.Secondary;
            LineSeriesCollection1.Add(ds);

            Title2 = string.Format("{0}: Correlation (Correl_Avg = {1}, Correl_All = {2})", Ticker1 + ", " + Ticker2, Math.Round(betas[2], 3), Math.Round(betas[3], 3));
            LineSeriesCollection2.Clear();
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            foreach (PairSignalEntity p in pair)
                ds.Points.AddXY(p.Date, p.Correlation);
            LineSeriesCollection2.Add(ds);

            Title3 = string.Format("{0}: Beta (Beta_Avg = {1}, Beta_All = {2})", Ticker1 + ", " + Ticker2, Math.Round(betas[0], 3), Math.Round(betas[1],3));
            LineSeriesCollection3.Clear();
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            foreach (PairSignalEntity p in pair)
                ds.Points.AddXY(p.Date, p.Beta);
            LineSeriesCollection3.Add(ds);
        }

        private void AddSignalChart()
        {
            Title4 = string.Format("{0}: Signal", Ticker1 + ", " + Ticker2);
            LineSeriesCollection4.Clear();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            foreach (PairSignalEntity p in PairCollection)
                ds.Points.AddXY(p.Date, p.Signal);
            LineSeriesCollection4.Add(ds);
        }

        private void AddPnLCharts()
        {
            double pnl = Math.Round(YearlyPnLTable.Rows[YearlyPnLTable.Rows.Count - 1]["PnL"].To<double>(), 0);
            double sharpe = Math.Round(YearlyPnLTable.Rows[YearlyPnLTable.Rows.Count - 1]["Sharpe"].To<double>(), 3);
            int numTrades = YearlyPnLTable.Rows[YearlyPnLTable.Rows.Count - 1]["NumTrades"].To<int>();
            Title5 = string.Format("{0}: P&L (Total PnL = {1}, Sharpe = {2}, NumTrades = {3})", Ticker1 + "," + Ticker2, pnl, sharpe, numTrades);
            LineSeriesCollection5.Clear();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Color = System.Drawing.Color.Red;
            foreach (PairPnLEntity p in PnLCollection)
                ds.Points.AddXY(p.Date, p.PnlCum);
            ds.Name = "PnL";
            LineSeriesCollection5.Add(ds);

            Title6 = string.Format("{0}: Drawdown", Ticker1 + "," + Ticker2);
            LineSeriesCollection6.Clear();
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            foreach (DataRow row in drawdownTable.Rows)
                ds.Points.AddXY(row["Date"].To<DateTime>(), row["Drawdown"].To<double>());
            ds.Name = "Drawndown";
            LineSeriesCollection6.Add(ds);
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Color = System.Drawing.Color.Red;
            foreach (DataRow row in drawdownTable.Rows)
                ds.Points.AddXY(row["Date"].To<DateTime>(), row["MaxDrawdown"].To<double>());
            ds.Name = "MaxDrawdown";
            LineSeriesCollection6.Add(ds);
        }
    }
}
