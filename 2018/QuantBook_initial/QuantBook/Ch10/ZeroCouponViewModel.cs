using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Data;
using QuantLib;
using QuantBook.Models.FixedIncome;
using System.Windows.Forms.DataVisualization.Charting;
using System;

namespace QuantBook.Ch10
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class ZeroCouponViewModel : Screen
    {
        [ImportingConstructor]
        public ZeroCouponViewModel()
        {
            DisplayName = "02. Zero Coupon";

            ZcTable1 = new DataTable();
            ZcTable2 = new DataTable();
            LineSeriesCollection1 = new BindableCollection<Series>();
            LineSeriesCollection2 = new BindableCollection<Series>();
        }

        public BindableCollection<Series> LineSeriesCollection1 { get; set; }
        public BindableCollection<Series> LineSeriesCollection2 { get; set; }

        private DataTable zcTable1;
        public DataTable ZcTable1
        {
            get { return zcTable1; }
            set
            {
                zcTable1 = value;
                NotifyOfPropertyChange(() => ZcTable1);
            }
        }

        private DataTable zcTable2;
        public DataTable ZcTable2
        {
            get { return zcTable2; }
            set
            {
                zcTable2 = value;
                NotifyOfPropertyChange(() => ZcTable2);
            }
        }

        private double zSpread = 50;
        public double ZSpread
        {
            get { return zSpread; }
            set
            {
                zSpread = value;
                NotifyOfPropertyChange(() => ZSpread);
            }
        }

        public void StartZeroCoupon0()
        {
            DataTable dt = QuantLibFIHelper.ZeroCouponDirect();
            ZcTable1 = dt;
        }

        public void StartZeroCoupon1()
        {
            double[] depositRates = new double[] { 0.044, 0.045, 0.046, 0.047, 0.049, 0.051, 0.053 };
            Period[] depositMaturities = new Period[]
            {
                new Period(1, TimeUnit.Days), 
                new Period(1, TimeUnit.Months),
                new Period(2, TimeUnit.Months),
                new Period(3, TimeUnit.Months),
                new Period(6, TimeUnit.Months),
                new Period(9, TimeUnit.Months),
                new Period(12, TimeUnit.Months),
            };

            double[] bondCoupons = new double[] { 0.05, 0.06, 0.055, 0.05 };
            double[] bondPrices = new double[] { 99.55, 100.55, 99.5, 97.6 };
            Period[] bondMaturities = new Period[]
            {
                 new Period(14, TimeUnit.Months),
                 new Period(21, TimeUnit.Months),
                 new Period(2, TimeUnit.Years),
                 new Period(3, TimeUnit.Years),
            };

            DataTable dt1 = QuantLibFIHelper.ZeroCouponBootstrap(depositRates, depositMaturities, bondPrices, bondCoupons, bondMaturities, "DataPoints");
            DataTable dt2 = QuantLibFIHelper.ZeroCouponBootstrap(depositRates, depositMaturities, bondPrices, bondCoupons, bondMaturities, "NotDataPoints");
            ZcTable1 = dt1;
            ZcTable2 = dt2;
            AddCharts();
        }

        public void StartZeroCoupon2()
        {
            double[] depositRates = new double[] {0.0525, 0.055 };
            Period[] depositMaturities = new Period[]
            {
                new Period(6, TimeUnit.Months),
                new Period(12, TimeUnit.Months),
            };
            double[] bondCoupons = new double[] { 0.0575, 0.06, 0.0625, 0.065, 0.0675, 0.068, 0.07, 0.071, 0.0715, 0.072, 0.073, 0.0735, 0.074, 0.075, 0.076, 0.076, 0.077, 0.078 };
            Period[] bondMaturities = new Period[bondCoupons.Length];
            for (int i = 0; i < bondCoupons.Length; i++)
            {
                bondMaturities[i] = new Period((i + 3) * 6, TimeUnit.Months);
            }
            double[] bondPrices = new double[bondCoupons.Length];
            for (int i = 0; i < bondPrices.Length; i++)
                bondPrices[i] = 100.0;
            
            DataTable dt1 = QuantLibFIHelper.ZeroCouponBootstrap(depositRates, depositMaturities, bondPrices, bondCoupons, bondMaturities, "DataPoints");
            DataTable dt2 = QuantLibFIHelper.ZeroCouponBootstrap(depositRates, depositMaturities, bondPrices, bondCoupons, bondMaturities, "NotDataPoints");
            ZcTable1 = dt1;
            ZcTable2 = dt2;
            AddCharts();
        }

        private void AddCharts()
        {
            LineSeriesCollection1.Clear();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.XValueMember = "TimesToMaturity";
            ds.YValueMembers = "Zero Coupon Rate: R";
            ds.Name = "R";
            LineSeriesCollection1.Add(ds);

            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.XValueMember = "TimesToMaturity";
            ds.YValueMembers = "Equivalent Rate: Rc";
            ds.Name = "Rc";
            LineSeriesCollection1.Add(ds);

            LineSeriesCollection2.Clear();
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.XValueMember = "TimesToMaturity";
            ds.YValueMembers = "Discount Rate: B";
            LineSeriesCollection2.Add(ds);
        }

        private void AddCharts1()
        {
            LineSeriesCollection1.Clear();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.XValueMember = "TimesToMaturity";
            ds.YValueMembers = "Zero Coupon Rate: R";
            ds.Name = "R";
            LineSeriesCollection1.Add(ds);

            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.XValueMember = "TimesToMaturity";
            ds.YValueMembers = "Zero Coupon Rate: R with ZSpread";
            ds.Name = "R with ZSpread";
            LineSeriesCollection1.Add(ds);

            LineSeriesCollection2.Clear();
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.XValueMember = "TimesToMaturity";
            ds.YValueMembers = "Discount Rate: B";
            ds.Name = "B";
            LineSeriesCollection2.Add(ds);

            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.XValueMember = "TimesToMaturity";
            ds.YValueMembers = "Discount Rate: B with ZSpread";
            ds.Name = "B with ZSpread";
            LineSeriesCollection2.Add(ds);
        }



        public void StartInterbank()
        {
            DateTime settlementDate = new DateTime(2015, 2, 18);
            double[] depositRates = new double[] { 0.001375, 0.001717, 0.002112, 0.002581 };
            Period[] depositeMaturities = new Period[]
            {
                new Period(1,TimeUnit.Weeks),
                new Period(1,TimeUnit.Months),
                new Period(2,TimeUnit.Months),
                new Period(3,TimeUnit.Months)
            };

            double[] futRates = new double[] { 99.725, 99.585, 99.385, 99.16, 98.93, 98.715 };
            double[] swapRates = new double[] { 0.0089268, 0.0123343, 0.0147985, 0.0165843, 0.0179191 };
            Period[] swapMaturities = new Period[]
            {
                new Period(2,TimeUnit.Years),
                new Period(3,TimeUnit.Years),
                new Period(4,TimeUnit.Years),
                new Period(5,TimeUnit.Years),
                new Period(6,TimeUnit.Years)
            };

            DataTable dt = QuantLibFIHelper.InterbankZeroCoupon(settlementDate, depositRates, depositeMaturities, futRates, swapRates, swapMaturities);
            ZcTable2 = dt;
            AddCharts();
        }

        public void StartZSpread()
        {
            double[] depositRates = new double[] { 0.0525, 0.055 };
            Period[] depositMaturities = new Period[]
            {
                new Period(6, TimeUnit.Months),
                new Period(12, TimeUnit.Months),
            };
            double[] bondCoupons = new double[] { 0.0575, 0.06, 0.0625, 0.065, 0.0675, 0.068, 0.07, 0.071, 0.0715, 0.072, 0.073, 0.0735, 0.074, 0.075, 0.076, 0.076, 0.077, 0.078 };
            Period[] bondMaturities = new Period[bondCoupons.Length];
            for (int i = 0; i < bondCoupons.Length; i++)
            {
                bondMaturities[i] = new Period((i + 3) * 6, TimeUnit.Months);
            }
            double[] bondPrices = new double[bondCoupons.Length];
            for (int i = 0; i < bondPrices.Length; i++)
                bondPrices[i] = 100.0;

            DataTable dt1 = QuantLibFIHelper.ZeroCouponBootstrapZSpread(depositRates, depositMaturities, bondPrices, bondCoupons, bondMaturities, "DataPoints", ZSpread);
            DataTable dt2 = QuantLibFIHelper.ZeroCouponBootstrapZSpread(depositRates, depositMaturities, bondPrices, bondCoupons, bondMaturities, "NotDataPoints", ZSpread);
            ZcTable1 = dt1;
            ZcTable2 = dt2;
            AddCharts1();
        }
    }
}
