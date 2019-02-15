using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models;
using System.Data;
using QuantBook.Models.AnalysisModel;
using Accord.Statistics.Analysis;
using System.Windows.Forms.DataVisualization.Charting;
using QuantBook.Models.ChartModel;

namespace QuantBook.Ch06
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MultiRegressionViewModel : Screen
    {
        [ImportingConstructor]
        public MultiRegressionViewModel()
        {
            DisplayName = "04. Multi-Regression";
            Tickers = "MS,GS,C";
            tks = Tickers.Split(',');
            StartDate = ("9/21/2004").To<DateTime>();
            EndDate = ("5/15/2014").To<DateTime>();            
            OrigData = new DataTable();
            ScatterCollection = new BindableCollection<Series>();
            LineCollection = new BindableCollection<Series>();
        }

        public BindableCollection<Series> ScatterCollection { get; set; }
        public BindableCollection<Series> LineCollection { get; set; }
        private MultiLinearResult multiResults = new MultiLinearResult();

        private Accord.Statistics.Testing.AnovaSourceCollection anova;
        public Accord.Statistics.Testing.AnovaSourceCollection Anova
        {
            get { return anova; }
            set
            {
                anova = value;
                NotifyOfPropertyChange(() => Anova);
            }
        }

        private LinearRegressionCoefficientCollection coeffs;
        public LinearRegressionCoefficientCollection Coeffs
        {
            get { return coeffs; }
            set
            {
                coeffs = value;
                NotifyOfPropertyChange(() => Coeffs);
            }
        }

        private string[] tks = null;
        private string tickers = string.Empty;
        public string Tickers
        {
            get { return tickers; }
            set
            {
                tickers = value;
                NotifyOfPropertyChange(() => Tickers);
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

        private DataTable origData;
        public DataTable OrigData
        {
            get { return origData; }
            set
            {
                origData = value;
                NotifyOfPropertyChange(() => OrigData);
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

        private string xlabel1 = string.Empty;
        public string Xlabel1
        {
            get { return xlabel1 ; }
            set
            {
                xlabel1 = value;
                NotifyOfPropertyChange(() => Xlabel1);
            }
        }

        private string ylabel1 = string.Empty;
        public string Ylabel1
        {
            get { return ylabel1; }
            set
            {
                ylabel1 = value;
                NotifyOfPropertyChange(() => Ylabel1);
            }
        }

        public void IndexRegression()
        {
            tks = new string[] { "HY", "SPX", "VIX" };
            DataTable dt = MultiLinearHelper.GetMultiLinearDataIndex(StartDate, EndDate);
            GetResults(dt, tks);
            AddCharts();
        }

        public void StockRegression()
        {
            tks = Tickers.Split(',');
            DataTable dt = MultiLinearHelper.GetMultiLienarDataStock(tks, StartDate, EndDate);
            GetResults(dt, tks);
            AddCharts();
        }

        private void GetResults(DataTable dt, string[] tks)
        {
            multiResults = MultiLinearHelper.GetMultiRegression(dt, tks);
            OrigData = dt;
            Coeffs = multiResults.Coefficients;
            Anova = multiResults.Anova;
            
        }

        private void AddCharts()
        {
            string colx = "MrComponent";
            string coly = tks[0];
            
            //Scatter chart:
            Title1 = string.Format("{0} ~ MR, r2 = {1}, r2Adj = {2}", coly, Math.Round(multiResults.RSquared, 4), Math.Round(multiResults.RSquaredAdj, 4));
            Xlabel1 = colx;
            Ylabel1 = coly;
            ScatterCollection.Clear();
            ScatterCollection.AddRange(MSChartHelper.ScatterChart(OrigData, colx, coly, RegressionType.SimpleLinear));

            //Line chart:
            LineCollection.Clear();
            System.Collections.Generic.List<System.Drawing.Color> colors = MSChartHelper.GetColors();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Name = "Original";
            ds.Color = colors[0];
            ds.XValueType = ChartValueType.DateTime;
            ds.XValueMember = "Date";
            ds.YValueMembers = coly;
            LineCollection.Add(ds);

            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Name = "Multi-Regression";
            ds.Color = colors[1];
            ds.XValueType = ChartValueType.DateTime;
            ds.XValueMember = "Date";
            ds.YValueMembers = colx;
            LineCollection.Add(ds);

            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Name = "Spread";
            ds.Color = colors[2];
            ds.XValueType = ChartValueType.DateTime;
            ds.XValueMember = "Date";
            ds.YValueMembers = "Spread";
            ds.YAxisType = AxisType.Secondary;
            LineCollection.Add(ds);


        }
    }
}
