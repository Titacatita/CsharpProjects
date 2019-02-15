using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models.ChartModel;
using QuantBook.Models.DataModel;
using QuantBook.Models;
using System.Windows.Forms.DataVisualization.Charting;
using QuantBook.Models.AnalysisModel;

namespace QuantBook.Ch06
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SimpleLinearRegressionViewModel : Screen
    {
        [ImportingConstructor]
        public SimpleLinearRegressionViewModel()
        {
            DisplayName = "01. Simple LR";
            
            StockData = new BindableCollection<PairStockData>();
            StockReturnData = new BindableCollection<PairStockData>();
            IndexData = new BindableCollection<IndexData>();
            HySpxCollection = new BindableCollection<Series>();
            HyVixCollection = new BindableCollection<Series>();
            Stock1Collection = new BindableCollection<Series>();
            Stock2Collection = new BindableCollection<Series>();
        }

        public BindableCollection<Series> HySpxCollection { get; set; }
        public BindableCollection<Series> HyVixCollection { get; set; }
        public BindableCollection<Series> Stock1Collection { get; set; }
        public BindableCollection<Series> Stock2Collection { get; set; }
        public BindableCollection<PairStockData> StockData { get; set; }
        public BindableCollection<PairStockData> StockReturnData { get; set; }
        public BindableCollection<IndexData> IndexData { get; set; }

        
        private string ticker = "GS";
        public string Ticker
        {
            get { return ticker; }
            set
            {
                ticker = value;
                NotifyOfPropertyChange(() => Ticker);
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

        public void GetData()
        {
            IndexData.Clear();
            BindableCollection<IndexData> idx = Dal.GetIndexData();
            DateTime startDate = ("5/15/2005").To<DateTime>();
            DateTime endDate = ("5/15/2014").To<DateTime>();
            IndexData.AddRange(Dal.GetIndexData(startDate, endDate));

            StockData.Clear();
            startDate = ("11/25/2013").To<DateTime>();
            endDate = ("11/25/2015").To<DateTime>();
            StockData.AddRange(Dal.GetPairStockData("^GSPC", Ticker, startDate, endDate, "Close", DataSourceEnum.Yahoo));

            StockReturnData.Clear();
            for (int i = 1; i < StockData.Count; i++)
            {
                PairStockData p0 = StockData[i - 1];
                PairStockData p1 = StockData[i];
                StockReturnData.Add(new PairStockData
                {
                    Date = p1.Date,
                    Price1 = (p1.Price1 - p0.Price1) / p0.Price1,
                    Price2 = (p1.Price2 - p0.Price2) / p0.Price2
                });
            }
        }

        public void PlotData()
        {
            IndexCharts();
            StockCharts();
        }

        private void IndexCharts()
        {
            HySpxCollection.Clear();
            SimpleLinearResult slr = LinearAnalysisHelper.GetSimpleRegression(IndexData, "SPX", "HYSpread");
            Title1 = string.Format("HY ~ SPX, a = {0}, b = {1}, R2 = {2}, R2Adj = {3} ", Math.Round(slr.Alpha, 4), Math.Round(slr.Beta, 4), Math.Round(slr.RSquared, 4), Math.Round(slr.RSquaredAdj, 4));
            HySpxCollection.AddRange(MSChartHelper.ScatterChart(IndexData, "SPX", "HYSpread", RegressionType.SimpleLinear));
            
            HyVixCollection.Clear();
            slr = LinearAnalysisHelper.GetSimpleRegression(IndexData, "VIX", "HYSpread");
            Title2 = string.Format("HY ~ VIX, a = {0}, b = {1}, R2 = {2}, R2Adj = {3}", Math.Round(slr.Alpha, 4), Math.Round(slr.Beta, 4), Math.Round(slr.RSquared, 4), Math.Round(slr.RSquaredAdj, 4));
            HyVixCollection.AddRange(MSChartHelper.ScatterChart(IndexData, "VIX", "HYSpread", RegressionType.SimpleLinear));
        }

        private void StockCharts()
        {
            Stock1Collection.Clear();
            SimpleLinearResult slr = LinearAnalysisHelper.GetSimpleRegression(StockData, "Price1", "Price2");
            Title3 = string.Format("Prices : {0} ~ SPX, a = {1}, b = {2}, R2 = {3}, R2Adj = {4}", Ticker, Math.Round(slr.Alpha, 4), Math.Round(slr.Beta, 4), Math.Round(slr.RSquared, 4), Math.Round(slr.RSquaredAdj, 4));
            Stock1Collection.AddRange(MSChartHelper.ScatterChart(StockData, "Price1", "Price2", RegressionType.SimpleLinear));

            Stock2Collection.Clear();
            slr = LinearAnalysisHelper.GetSimpleRegression(StockReturnData, "Price1", "Price2");
            Title4 = string.Format("Returns: {0} ~ SPX, a = {1}, b = {2}, R2 = {3}, R2Adj = {4} ", Ticker, Math.Round(slr.Alpha, 4), Math.Round(slr.Beta, 4), Math.Round(slr.RSquared, 4), Math.Round(slr.RSquaredAdj, 4));
            Stock2Collection.AddRange(MSChartHelper.ScatterChart(StockReturnData, "Price1", "Price2", RegressionType.SimpleLinear));
        }
    }
}
