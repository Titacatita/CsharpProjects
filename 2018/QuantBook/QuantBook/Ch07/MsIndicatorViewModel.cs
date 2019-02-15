using System;
using System.Linq;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Data;
using QuantBook.Models.DataModel;
using System.Windows.Forms.DataVisualization.Charting;

namespace QuantBook.Ch07
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MsIndicatorViewModel : Screen
    {
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public MsIndicatorViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "01. Indicators";
            StockPrices = new BindableCollection<StockPrice>();
            Periods = new BindableCollection<int>();
            OutputTable = new DataTable();

            Ticker = "GS";
            StartDate = DateTime.Today.AddMonths(-6);
            EndDate = DateTime.Today;
            ExportFile = SelectedFinancialFormula.ToString() + ".csv";
        }

        private string exportFile = string.Empty;
        public string ExportFile
        {
            get { return exportFile; }
            set
            {
                exportFile = value;
                NotifyOfPropertyChange(() => ExportFile);
            }
        }

        private string ticker = string.Empty;
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

        private int period1 = 0;
        public int Period1
        {
            get { return period1; }
            set
            {
                period1 = value;
                NotifyOfPropertyChange(() => Period1);
            }
        }

        private int period2 = 0;
        public int Period2
        {
            get { return period2; }
            set
            {
                period2 = value;
                NotifyOfPropertyChange(() => Period2);
            }
        }

        public BindableCollection<StockPrice> StockPrices { get; set; }
        public BindableCollection<int> Periods { get; set; }

        private DataTable outputTable;
        public DataTable OutputTable
        {
            get { return outputTable; }
            set
            {
                outputTable = value;
                NotifyOfPropertyChange(() => OutputTable);
            }
        }

        private bool isStartIndicator = false;
        public bool IsStartIndicator
        {
            get { return isStartIndicator; }
            set
            {
                isStartIndicator = value;
                NotifyOfPropertyChange(() => IsStartIndicator);
            }
        }

        private IEnumerable<FinancialFormula> financialFormula;
        public IEnumerable<FinancialFormula> FinancialFormula
        {
            get { return Enum.GetValues(typeof(FinancialFormula)).Cast<FinancialFormula>(); }
            set
            {
                financialFormula = value;
                NotifyOfPropertyChange(() => FinancialFormula);
            }
        }

        private FinancialFormula selectedFinancialFormula;
        public FinancialFormula SelectedFinancialFormula
        {
            get { return selectedFinancialFormula; }
            set
            {
                selectedFinancialFormula = value;
                NotifyOfPropertyChange(() => SelectedFinancialFormula);
            }
        }

        public void FormulaChanged()
        {
            try
            {
                ExportFile = SelectedFinancialFormula.ToString() + ".csv";

                switch(SelectedFinancialFormula)
                {
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.AverageTrueRange:
                        Period1 = 15;
                        Period2 = 0;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.BollingerBands:
                        Period1 = 20;
                        Period2 = 2;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.ChaikinOscillator:
                        Period1 = 5;
                        Period2 = 12;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.CommodityChannelIndex:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.RelativeStrengthIndex:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.StandardDeviation:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.WilliamsR:
                        Period1 = 15;
                        Period2 = 0;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.DetrendedPriceOscillator:
                        Period1 = 10;
                        Period2 = 0;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.EaseOfMovement:
                        Period1 = 14;
                        Period2 = 0;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.Envelopes:
                        Period1 = 20;
                        Period2 = 7;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.ExponentialMovingAverage:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.MovingAverage:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.TriangularMovingAverage:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.TripleExponentialMovingAverage:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.WeightedMovingAverage:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.MoneyFlow:
                        Period1 = 20;
                        Period2 = 0;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.MassIndex:
                        Period1 = 30;
                        Period2 = 12;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.MedianPrice:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.TypicalPrice:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.WeightedClose:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.AccumulationDistribution:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.OnBalanceVolume:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.PriceVolumeTrend:
                        Period1 = 0;
                        Period2 = 0;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.MovingAverageConvergenceDivergence:
                        Period1 = 15;
                        Period2 = 30;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.NegativeVolumeIndex:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.PositiveVolumeIndex:
                        Period1 = 100;
                        Period2 = 0;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.RateOfChange:
                        Period1 = 5;
                        Period2 = 0;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.StochasticIndicator:
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.VolatilityChaikins:
                        Period1 = 15;
                        Period2 = 15;
                        break;
                    case System.Windows.Forms.DataVisualization.Charting.FinancialFormula.VolumeOscillator:
                        Period1 = 10;
                        Period2 = 30;
                        break;
                }                
            }
            catch { }
        }


        public void GetData()
        {
            StockPrices.Clear();
            BindableCollection<StockData> data = Dal.GetStockData(Ticker, StartDate, EndDate, DataSourceEnum.Yahoo);
            foreach(StockData p in data)
            {
                StockPrices.Add(new StockPrice
                {
                    Ticker = p.Ticker,
                    Date = (DateTime)p.Date,
                    PriceOpen = (double)p.Open,
                    PriceHigh = (double)p.High,
                    PriceLow = (double)p.Low,
                    PriceClose =(double)p.Close,
                    Volume = (double)p.Volume,
                    PriceAdj = (double)p.AdjClose
                });
            }
        }

        public void PlotData()
        {
            if (StockPrices.Count < 1)
                GetData();
            if (StockPrices.Count < 1)
                return;
            IsStartIndicator = false;
            Periods.Clear();
            Periods.Add(Period1);
            Periods.Add(Period2);
            IsStartIndicator = true;
        }

        public void Export()
        {
            Models.ModelHelper.DatatableToCsv(OutputTable, ExportFile);
        }

    }
}
