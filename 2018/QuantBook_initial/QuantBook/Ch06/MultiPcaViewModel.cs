using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models;
using System.Data;
using QuantBook.Models.AnalysisModel;
using System.Windows.Forms.DataVisualization.Charting;
using QuantBook.Models.ChartModel;
using System.Collections.Generic;

namespace QuantBook.Ch06
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MultiPcaViewModel : Screen
    {
        [ImportingConstructor]
        public MultiPcaViewModel()
        {
            DisplayName = "05. Multi-PCA";
            Tickers = "MS,GS,C,XLF";
            tks = Tickers.Split(',');
            StartDate = "21/09/2004".To<DateTime>();
            EndDate = "15/05/2014".To<DateTime>();            
            OrigData = new DataTable();
            ScatterCollection1 = new BindableCollection<Series>();
            ScatterCollection2 = new BindableCollection<Series>();
        }

        public BindableCollection<Series> ScatterCollection1 { get; set; }
        public BindableCollection<Series> ScatterCollection2 { get; set; }
        private List<SimpleLinearResult> linearResults = new List<SimpleLinearResult>();

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

        public void IndexPca()
        {
            tks = new string[] { "HY", "SPX", "VIX" };
            DataTable dt = MultiLinearHelper.GetMultiLinearDataIndex(StartDate, EndDate);
            linearResults.Clear();
            linearResults.AddRange(MultiLinearHelper.GetMultiPca(dt, tks));
            OrigData = dt;
            AddCharts();
        }

        public void StockPca()
        {
            tks = Tickers.Split(',');
            DataTable dt = MultiLinearHelper.GetMultiLienarDataStock(tks, StartDate, EndDate);
            linearResults.Clear();
            linearResults.AddRange(MultiLinearHelper.GetMultiPca(dt, tks));
            OrigData = dt;
            AddCharts();
        }

        private void AddCharts()
        {
            string colx = "PcaComponent";
            string coly = tks[0];

            //Scatter chart 1:
            Title1 = string.Format("{0} ~ PCA Component", coly);
            YLabel1 = coly;
            ScatterCollection1.Clear();
            ScatterCollection1.AddRange(MSChartHelper.ScatterChart(OrigData, colx, coly, RegressionType.SimpleLinear));

            //Scatter chart 2:
            ScatterCollection2.Clear();
            coly = tks[0] + "1";
            Title2 = string.Format("{0} ~ PCA Component, R2_PCA = {1}, R2_SLR = {2}", coly, Math.Round(linearResults[0].RSquared, 4), Math.Round(linearResults[1].RSquared, 4));
            YLabel2 = coly;
            ScatterCollection2.AddRange(MSChartHelper.ScatterChart(OrigData, colx, coly, RegressionType.Both));
        }
    }
}
