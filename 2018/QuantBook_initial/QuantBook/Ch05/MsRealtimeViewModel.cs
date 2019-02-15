using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models;
using System.Collections.Generic;
using System.Windows;
using QuantBook.Models.DataModel;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;

namespace QuantBook.Ch05
{
    
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MsRealtimeViewModel : Screen
    {
        private readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public MsRealtimeViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "10. MS Real-Time";       
            AsyncCollection = new BindableCollection<Series>();
            TimerCollection = new BindableCollection<Series>();
            StockData = new BindableCollection<StockData>();
        }

        public BindableCollection<Series> AsyncCollection { get; set; }
        public BindableCollection<Series> TimerCollection { get; set; }
        public BindableCollection<StockData> StockData { get; set; }

        private bool isStop = false;
        public bool IsStop
        {
            get { return isStop; }
            set
            {
                isStop = value;
                NotifyOfPropertyChange(() => IsStop);
            }
        }


        public void GetData()
        {
            StockData.Clear();

            string ticker = "GS";
            DateTime endDate = DateTime.Today;
            DateTime startDate = endDate.AddYears(-5);
            StockData.AddRange(Dal.GetStockDataFromQuandl(ticker, startDate, endDate));
        }

        public async void StartAsyncChart()
        {
            IsStop = false;
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Candlestick;
            ds.IsXValueIndexed = true;
            ds.YValuesPerPoint = 4;
            ds["OpenCloseStyle"] = "Triangle";
            ds["ShowOpenClose"] = "Both";
            ds["PointWidth"] = "0.6";
            ds["PriceUpColor"] = "Green";
            ds["PriceDownColor"] = "Red";
            ds.XValueType = ChartValueType.Date;

            int num = 50;
            int totalRows = StockData.Count;
            for (int i = 0; i < num; i++)
            {
                StockData p = StockData[i];
                ds.Points.AddXY((DateTime)p.Date, (double)p.High);
                ds.Points[i].YValues[1] = (double)p.Low;
                ds.Points[i].YValues[2] = (double)p.Open;
                ds.Points[i].YValues[3] = (double)p.Close;
            }

            AsyncCollection.Clear();
            AsyncCollection.Add(ds);

            List<object> objList = new List<object>
            {
                "Ready...",
                0,
                totalRows,
                0
            };

            await Task.Run(() =>
            {            
                for (int i = num; i < totalRows; i++)
                {
                    if (IsStop)
                        break;

                    Thread.Sleep(100);
                    StockData p = StockData[i];
                    Application.Current.Dispatcher.BeginInvoke((System.Action)delegate()
                    {
                        try
                        {
                            ds.Points.AddXY((DateTime)p.Date, (double)p.High);
                            int j = ds.Points.Count - 1;
                            ds.Points[j].YValues[1] = (double)p.Low;
                            ds.Points[j].YValues[2] = (double)p.Open;
                            ds.Points[j].YValues[3] = (double)p.Close;
                            ds.Points.RemoveAt(0);
                            AsyncCollection.Clear();
                            AsyncCollection.Add(ds);
                        }
                        catch { }
                    });

                    objList[0] = string.Format("Total Runs = {0}, i={1}, Date={2}, close={3}", totalRows, i, p.Date, p.Close);
                    objList[3] = i;
                    _events.PublishOnUIThread(new ModelEvents(objList));
                }

                objList[0] = "Ready...";
                objList[1] = 0;
                objList[2] = 1;
                objList[3] = 0;
                _events.PublishOnUIThread(new ModelEvents(objList));

            });
        }


        public void StartTimerChart()
        {
            IsStop = false;
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Stock;
            ds.IsXValueIndexed = true;
            ds.YValuesPerPoint = 4;
            ds["OpenCloseStyle"] = "Triangle";
            ds["ShowOpenClose"] = "Both";
            ds["PointWidth"] = "0.6";
            ds["PriceUpColor"] = "Green";
            ds["PriceDownColor"] = "Red";
            ds.XValueType = ChartValueType.Date;

            int num = 50;
            int totalRows = StockData.Count;
            for (int i = 0; i < num; i++)
            {
                StockData p = StockData[i];
                ds.Points.AddXY((DateTime)p.Date, (double)p.High);
                ds.Points[i].YValues[1] = (double)p.Low;
                ds.Points[i].YValues[2] = (double)p.Open;
                ds.Points[i].YValues[3] = (double)p.Close;
            }

            TimerCollection.Clear();
            TimerCollection.Add(ds);

            int count = num;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Start();
            timer.Tick += (o, e) =>
            {
                try
                {
                    StockData p = StockData[count];
                    ds.Points.AddXY((DateTime)p.Date, (double)p.High);
                    int j = ds.Points.Count - 1;
                    ds.Points[j].YValues[1] = (double)p.Low;
                    ds.Points[j].YValues[2] = (double)p.Open;
                    ds.Points[j].YValues[3] = (double)p.Close;
                    ds.Points.RemoveAt(0);
                    TimerCollection.Clear();
                    TimerCollection.Add(ds);
                }
                catch { }

                if (count >= totalRows || IsStop)
                    timer.Stop();
                count++;
            };           
        }

        public void Stop()
        {
            IsStop = true;
        }







        /*private Chart myChart;
        public Chart MyChart
        {
            get { return myChart; }
            set
            {
                myChart = value;
                NotifyOfPropertyChange(() => MyChart);
            }
        }

        private void CreateChart()
        {
            MsRealtimeView view = this.GetView() as MsRealtimeView;
            ChartArea area = new ChartArea();
            MSChartHelper.ChartStyle(view.chart1.myChart, area, ChartBackgroundColor.Blue);
            double[] yValues = { 23.67, 75.45, 60.45, 34.54, 85.62, 32.43, 55.98, 67.23, 56.34, 23.14, 45.24, 67.41, 13.45, 56.36, 45.29 };

            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.Color = System.Drawing.Color.Blue;
            ds.Name = "S1";
            for (int pointIndex = 0; pointIndex < yValues.Length; pointIndex++)
            {
                ds.Points.AddXY(1990 + pointIndex, yValues[pointIndex]);
            }
            view.chart1.myChart.Series.Add(ds);

            DataSet dataSet1 = view.chart1.myChart.DataManipulator.ExportSeriesValues("S1");
            ChartData = dataSet1.Tables[0];
        }


        private void AddIndicator()
        {
            MsRealtimeView view = this.GetView() as MsRealtimeView;
            ChartArea area = new ChartArea();
            ChartArea area2 = new ChartArea();
            ChartArea area3 = new ChartArea();
            MSChartHelper.ChartStyle3(view.chart1.myChart, area, area2, area3, ChartBackgroundColor.Blue);
            List<System.Drawing.Color> my_colors = MSChartHelper.GetColors();
            //view.chart1.myChart.Series.Clear();

            view.chart1.myChart.DataSource = StockPrices;
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Stock;
            ds.XValueType = ChartValueType.Date;
            ds["OpenCloseStyle"] = "Triangle";
            ds["ShowOpenClose"] = "Both";
            ds["PointWidth"] = "0.6";
            ds["PriceUpColor"] = "Green";
            ds["PriceDownColor"] = "Red";
            ds.XValueMember = "Date";
            ds.YValueMembers = "PriceHigh, PriceLow, PriceOpen, PriceClose";            
            ds.Name = "Series1";
            view.chart1.myChart.Series.Add(ds);
            //view.chart1.myChart.DataBind();

            ds = new Series();
            ds.ChartType = SeriesChartType.Column;
            ds.XValueType = ChartValueType.Date;
            ds.XValueMember = "Date";
            ds.YValueMembers = "Volume";
            ds.Name = "Series3";
            ds.ChartArea = area3.Name;
            view.chart1.myChart.Series.Add(ds);
            view.chart1.myChart.DataBind();

            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.XValueType = ChartValueType.Date;
            ds.XValueMember = "Date";
            ds.Name = "Series2";
            ds.ChartArea = area2.Name;
            foreach (var p in StockPrices)
                ds.Points.AddXY(p.Date, 0);
            view.chart1.myChart.Series.Add(ds);

           

            int n = 15;
            view.chart1.myChart.DataManipulator.FinancialFormula(FinancialFormula.WilliamsR, n.ToString(), "Series1:Y,Series1:Y2,Series1:Y3", "Series2:Y");

            for (int i = 0; i < n - 1; i++)
            {
                view.chart1.myChart.Series["Series1"].Points.RemoveAt(0);
                view.chart1.myChart.Series["Series3"].Points.RemoveAt(0);
            }
            view.chart1.myChart.Series["Series1"].IsXValueIndexed = true;
            view.chart1.myChart.Series["Series2"].IsXValueIndexed = true;
            view.chart1.myChart.Series["Series3"].IsXValueIndexed = true;

            DataSet dataSet1 = view.chart1.myChart.DataManipulator.ExportSeriesValues("Series2");
            ChartData = dataSet1.Tables[0];            
        }

        private void AddIndicator1()
        {
            MsRealtimeView view = this.GetView() as MsRealtimeView;
            ChartData = MSChartHelper.IndicatorChart(view.chart1.myChart, StockPrices, FinancialFormula.ExponentialMovingAverage, new BindableCollection<int>(){20}, ChartBackgroundColor.Green);
        }*/
    }
}
