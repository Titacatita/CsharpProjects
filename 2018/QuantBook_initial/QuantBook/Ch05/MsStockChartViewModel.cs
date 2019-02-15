using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using QuantBook.Models.ChartModel;
using QuantBook.Models.DataModel;
using System.Windows.Forms.DataVisualization.Charting;

namespace QuantBook.Ch05
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MsStockChartViewModel : Screen
    {
        [ImportingConstructor]
        public MsStockChartViewModel()
        {
            DisplayName = "09. MS Stock";
            
            StockData = new BindableCollection<StockData>();
            IndexData = new BindableCollection<IndexData>();
            LineCollection = new BindableCollection<Series>();
            ScatterCollection = new BindableCollection<Series>();
            HlocCollection = new BindableCollection<Series>();
            CandleCollection = new BindableCollection<Series>();                
        }

        public BindableCollection<Series> LineCollection { get; set; }
        public BindableCollection<Series> ScatterCollection { get; set; }
        public BindableCollection<Series> HlocCollection { get; set; }
        public BindableCollection<Series> CandleCollection { get; set; }
        public BindableCollection<StockData> StockData { get; set; }
        public BindableCollection<IndexData> IndexData { get; set; }

        public void GetData()
        {
            IndexData.Clear();
            IndexData.AddRange(Dal.GetIndexData());

            StockData.Clear();
            string ticker = "GS";
            DateTime endDate = DateTime.Today;
            DateTime startDate = endDate.AddMonths(-3);
            StockData.AddRange(Dal.GetStockDataFromQuandl(ticker, startDate, endDate));
        }

        public void PlotData()
        {
            LineChart();
            ScatterChart();
            HlocChart();
            CandleStickVolumeChart();
        }

        public void LineChart()
        {
            LineCollection.Clear();
            BindableCollection<Series> series = new BindableCollection<Series>();
            List<System.Drawing.Color> my_colors = MSChartHelper.GetColors();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Name = "HY";
            ds.Color = my_colors[0];
            ds.XValueType = ChartValueType.Date;
            ds.XValueType = ChartValueType.Date;
            ds.XValueMember = "Date";
            ds.YValueMembers = "HYSpread";
            LineCollection.Add(ds);

            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Name = "VIX";
            ds.Color = my_colors[1];
            ds.XValueType = ChartValueType.Date;
            ds.XValueType = ChartValueType.Date;
            ds.XValueMember = "Date";
            ds.YValueMembers = "VIX";
            LineCollection.Add(ds);

            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Name = "VIX2Y";
            ds.Color = my_colors[2];
            ds.XValueType = ChartValueType.Date;
            ds.XValueType = ChartValueType.Date;
            ds.XValueMember = "Date";
            ds.YValueMembers = "VIX";
            ds.YAxisType = AxisType.Secondary;
            LineCollection.Add(ds);
        }

        private void ScatterChart()
        {
            ScatterCollection.Clear();
            Series ds = new Series();
            ds = new Series();
            ds.ChartType = SeriesChartType.Point;
            ds.MarkerSize = 6;
            ds.MarkerStyle = MarkerStyle.Diamond;
            ds.MarkerBorderColor = System.Drawing.Color.Black;
            ds.MarkerColor = System.Drawing.Color.White;
            ds.XValueMember = "VIX";
            ds.YValueMembers = "HYSpread";
            ScatterCollection.Add(ds);
        }

        private void HlocChart()
        {
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Stock;
            ds.IsXValueIndexed = true;
            ds["OpenCloseStyle"] = "Triangle";
            ds["ShowOpenClose"] = "Both";
            ds["PointWidth"] = "0.6";
            ds["PriceUpColor"] = "Green";
            ds["PriceDownColor"] = "Red";
            ds.XValueType = ChartValueType.Date;
            ds.XValueMember = "Date";
            ds.YValueMembers = "High, Low, Open, Close";
            ds.ToolTip = "Date: #VALX \nOpen: #VALY3{0.00} \nHigh:#VALY1{0.00} \nLow: #VALY2{0.00} \nClose: #VALY4{0.00}";
            HlocCollection.Clear();
            HlocCollection.Add(ds);
        }

        private void CandleStickVolumeChart()
        {
            CandleCollection.Clear();
            List<System.Drawing.Color> my_colors = MSChartHelper.GetColors();

            Series ds = new Series();
            ds.ChartType = SeriesChartType.Candlestick;
            ds.IsXValueIndexed = true;
            ds.XValueType = ChartValueType.Date;
            ds["OpenCloseStyle"] = "Triangle";
            ds["ShowOpenClose"] = "Both";
            ds["PointWidth"] = "0.6";
            ds["PriceUpColor"] = "Green";
            ds["PriceDownColor"] = "Red";
            ds.XValueMember = "Date";
            ds.YValueMembers = "High, Low, Open, Close";
            ds.ToolTip = "#VALX, Min:#VALY2, Max:#VALY1";
            ds.Tag = 1;
            CandleCollection.Add(ds);

            ds = new Series();
            ds.ChartType = SeriesChartType.Column;
            ds.IsXValueIndexed = true;
            ds.XValueType = ChartValueType.Date;
            ds.Color = my_colors[0];
            ds["PointWidth"] = "0.6";
            ds.XValueMember = "Date";
            ds.YValueMembers = "Volume";
            ds.ToolTip = "#VALX, #VALY";
            ds.Tag = 2;
            CandleCollection.Add(ds);
        }
    }
}
