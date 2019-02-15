using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Data;
using QuantBook.Models.FixedIncome;
using System.Windows.Forms.DataVisualization.Charting;
using System;
using System.Linq;
using System.Collections.Generic;
using QuantLib;

namespace QuantBook.Ch10
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BondOptionViewModel : Screen
    {
        [ImportingConstructor]
        public BondOptionViewModel()
        {
            DisplayName = "03. Bonds with Options";

            Table1 = new DataTable();
            Table2 = new DataTable();
            LineSeriesCollection1 = new BindableCollection<Series>();
            LineSeriesCollection2 = new BindableCollection<Series>();
        }
        public BindableCollection<Series> LineSeriesCollection1 { get; set; }
        public BindableCollection<Series> LineSeriesCollection2 { get; set; }

        private DataTable table1;
        public DataTable Table1
        {
            get { return table1; }
            set
            {
                table1 = value;
                NotifyOfPropertyChange(() => Table1);
            }
        }

        private DataTable table2;
        public DataTable Table2
        {
            get { return table2; }
            set
            {
                table2 = value;
                NotifyOfPropertyChange(() => Table2);
            }
        }

        private double sigma = 0.06;
        public double Sigma
        {
            get { return sigma; }
            set
            {
                sigma = value;
                NotifyOfPropertyChange(() => Sigma);
            }
        }

        private IEnumerable<ConvertableBondEngineType> engineType;
        public IEnumerable<ConvertableBondEngineType> EngineType
        {
            get { return Enum.GetValues(typeof(ConvertableBondEngineType)).Cast<ConvertableBondEngineType>(); }
            set
            {
                engineType = value;
                NotifyOfPropertyChange(() => EngineType);
            }
        }

        private ConvertableBondEngineType selectedEngineType;
        public ConvertableBondEngineType SelectedEngineType
        {
            get { return selectedEngineType; }
            set
            {
                selectedEngineType = value;
                NotifyOfPropertyChange(() => SelectedEngineType);
            }
        }


        public void CallableBond0()
        {
            LineSeriesCollection1.Clear();
            LineSeriesCollection2.Clear();
            Table2 = new DataTable();

            QLNet.Date evalDate = new QLNet.Date(16, 10, 2007);
            QLNet.Date issueDate = new QLNet.Date(16, 9, 2004);
            QLNet.Date callDate = new QLNet.Date(15, 9, 2006);
            QLNet.Date maturity = new QLNet.Date(15, 9, 2012);
            double a = 0.03;
            double coupon = 0.0465;
            double flatRate = 0.055;
            DataTable dt = QLNetFIHelper.CallableBondPrice(evalDate, issueDate, callDate, maturity, coupon, Sigma, a, flatRate);
            Table1 = dt;
        }

        public void CallableBond1()
        {
            QLNet.Date evalDate = new QLNet.Date(16, 10, 2007);
            QLNet.Date issueDate = new QLNet.Date(16, 9, 2004);
            QLNet.Date callDate = new QLNet.Date(15, 9, 2006);
            QLNet.Date maturity = new QLNet.Date(15, 9, 2012);
            double a = 0.03;
            double coupon = 0.0465;
            double flatRate = 0.055;

            DataTable dt1 = new DataTable();
            dt1.Columns.Add("sigma", typeof(double));
            dt1.Columns.Add("Price", typeof(double));

            for (int i = 1; i < 20; i++)
            {
                double sigma = 0.01 * i;
                DataTable dt = QLNetFIHelper.CallableBondPrice(evalDate, issueDate, callDate, maturity, coupon, sigma, 0.03, flatRate);
                foreach (DataRow r in dt.Rows)
                {
                    if (r[0].ToString() == "Clean Price")
                        dt1.Rows.Add(sigma, r["Value"]);
                }
            }
            Table1 = dt1;

            DataTable dt2 = new DataTable();
            dt2.Columns.Add("a", typeof(double));
            dt2.Columns.Add("Price", typeof(double));

            for (int i = 1; i < 20; i++)
            {
                a = 0.01 * i;
                DataTable dt = QLNetFIHelper.CallableBondPrice(evalDate, issueDate, callDate, maturity, coupon, 0.06, a, flatRate);
                foreach (DataRow r in dt.Rows)
                {
                    if (r[0].ToString() == "Clean Price")
                        dt2.Rows.Add(a, r["Value"]);
                }
            }
            Table2 = dt2;

            AddCharts();
        }


        private void AddCharts()
        {
            LineSeriesCollection1.Clear();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.MarkerBorderWidth = 1;
            ds.MarkerSize = 8;
            ds.MarkerStyle = MarkerStyle.Diamond;
            ds.XValueMember = "sigma";
            ds.YValueMembers = "Price";
            LineSeriesCollection1.Add(ds);

            LineSeriesCollection2.Clear();
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.MarkerBorderWidth = 1;
            ds.MarkerSize = 8;
            ds.MarkerStyle = MarkerStyle.Diamond;
            ds.XValueMember = "a";
            ds.YValueMembers = "Price";
            LineSeriesCollection2.Add(ds);
        }

        public void ConvertibleBond()
        {
            LineSeriesCollection1.Clear();
            LineSeriesCollection2.Clear();
            Table2 = new DataTable();
            DataTable dt = QuantLibFIHelper.ConvertibleBondPrice(SelectedEngineType);
            Table1 = dt;
        }
    }
}
