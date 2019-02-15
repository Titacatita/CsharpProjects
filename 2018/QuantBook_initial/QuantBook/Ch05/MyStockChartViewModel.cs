using System;
using System.Linq;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Windows.Media;
using ChartControl;
using QuantBook.Models.Yahoo;
using QuantBook.Models;
using System.Data;

namespace QuantBook.Ch05
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MyStockChartViewModel : Screen
    {
                private readonly IEventAggregator _events;
        [ImportingConstructor]
        public MyStockChartViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "04. My Stock Chart";
            DataCollection = new BindableCollection<StockSeries>();
            StockPrices = new BindableCollection<ChartControl.StockPrice>();
            EndDate = DateTime.Today;
            StartDate = EndDate.AddMonths(-3);            
            Ticker = "GS";
        }

        public BindableCollection<StockSeries> DataCollection { get; set; }
        public BindableCollection<ChartControl.StockPrice> StockPrices { get; set; }

        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyOfPropertyChange(() => Title);
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

        private IEnumerable<StockChartTypeEnum> stockChartType;
        public IEnumerable<StockChartTypeEnum> StockChartType
        {
            get
            {
                return Enum.GetValues(typeof(StockChartTypeEnum)).
                    Cast<StockChartTypeEnum>();
            }
            set
            {
                stockChartType = value;
                NotifyOfPropertyChange(() => StockChartType);
            }
        }

        private StockChartTypeEnum selectedStockChartType =
            StockChartTypeEnum.LineAdj;
        public StockChartTypeEnum SelectedStockChartType
        {
            get { return selectedStockChartType; }
            set
            {
                selectedStockChartType = value;
                NotifyOfPropertyChange(() => SelectedStockChartType);
            }
        }

        public void GetStockData()
        {
            DataTable data = YahooHelper.GetYahooHistStockDataTable(Ticker, StartDate, EndDate);
            data = ModelHelper.DatatableSort(data, "Date ASC");
            StockPrices.Clear();
            foreach (DataRow row in data.Rows)
            {
                StockPrices.Add(new ChartControl.StockPrice
                {
                    Ticker = Ticker,
                    Date = row["Date"].ToString().To<DateTime>(),
                    PriceOpen = row["Open"].ToString().To<double>(),
                    PriceHigh = row["High"].ToString().To<double>(),
                    PriceLow = row["Low"].ToString().To<double>(),
                    PriceClose = row["Close"].ToString().To<double>(),
                    PriceAdj = row["Adj Close"].ToString().To<double>(),
                    Volume = row["Volume"].ToString().To<double>(),
                });
            }

            AddChart();
        }

        public void AddChart()
        {
            Title = string.Format("{0}: {1}", Ticker, SelectedStockChartType);
            DataCollection.Clear();
            StockSeries ss = new StockSeries();
            ss.StockPrices = StockPrices;
            ss.LineColor = Brushes.DarkBlue;
            if (SelectedStockChartType == StockChartTypeEnum.Candlestick)
            {
                ss.FillColor1 = Brushes.DarkBlue;
                ss.FillColor2 = Brushes.White;
            }
            DataCollection.Add(ss);
        }           
    }
}
