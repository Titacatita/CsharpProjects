using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Data;
using QuantBook.Models.FixedIncome;
using System.Windows.Forms.DataVisualization.Charting;
using System;
using QuantLib;
using QuantBook.Models;

namespace QuantBook.Ch10
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CdsViewModel : Screen
    {
        [ImportingConstructor]
        public CdsViewModel()
        {
            DisplayName = "04. CDS Pricing";

            Table1 = new DataTable();
            Table2 = new DataTable();
            LineSeriesCollection1 = new BindableCollection<Series>();
            LineSeriesCollection2 = new BindableCollection<Series>();

            //EvalDate = new DateTime(2015, 3, 20);
            //Spreads = "34.93,53.60,72.02,106.39,129.39,139.46";
            //Tenors = "1Y,2Y,3Y,5Y,7Y,10Y";
            //RecoveryRate = 0.4;            
            //Spreads = "550,550,550,550";
            //Tenors = "3M,6M,1Y,2Y";
            //RecoveryRate = 0.5;
            //CdsCoupon = 500;
            //Notional = 10000;

            InitializeCds1();
            //InitializeCds2();
        }


        private void InitializeCds1()
        {
            EvalDate = new DateTime(2009, 6, 15);
            EffectiveDate = new DateTime(2009, 3, 20);
            Maturity = new DateTime(2014, 6, 20);
            Spreads = "210";
            Tenors = "5Y";
            RecoveryRate = 0.4;
            CdsCoupon = 100;
            Notional = 10000;
            ProtectionSide = "Buyer";
        }

        private void InitializeCds2()
        {
            EvalDate = new DateTime(2015, 5, 15);
            EffectiveDate = new DateTime(2015, 3, 20);
            Maturity = new DateTime(2018, 6, 20);
            Spreads = "34.93,53.60,72.02,106.39,129.39,139.46";
            Tenors = "1Y,2Y,3Y,5Y,7Y,10Y";
            RecoveryRate = 0.4;
            CdsCoupon = 100;
            Notional = 10000;
            ProtectionSide = "Buyer";
        }





        public BindableCollection<Series> LineSeriesCollection1 { get; set; }
        public BindableCollection<Series> LineSeriesCollection2 { get; set; }

        private DateTime evalDate;
        public DateTime EvalDate
        {
            get { return evalDate; }
            set
            {
                evalDate = value;
                NotifyOfPropertyChange(() => EvalDate);
            }
        }

        private DateTime effectiveDate;
        public DateTime EffectiveDate
        {
            get { return effectiveDate; }
            set
            {
                effectiveDate = value;
                NotifyOfPropertyChange(() => EffectiveDate);
            }
        }

        private DateTime maturity;
        public DateTime Maturity
        {
            get { return maturity; }
            set
            {
                maturity = value;
                NotifyOfPropertyChange(() => Maturity);
            }
        }


        private string spreads;
        public string Spreads
        {
            get { return spreads; }
            set
            {
                spreads = value;
                NotifyOfPropertyChange(() => Spreads);
            }
        }

        private string tenors;
        public string Tenors
        {
            get { return tenors; }
            set
            {
                tenors = value;
                NotifyOfPropertyChange(() => Tenors);
            }
        }

        private string protectionSide;
        public string ProtectionSide
        {
            get { return protectionSide; }
            set
            {
                protectionSide = value;
                NotifyOfPropertyChange(() => ProtectionSide);
            }
        }

        private double recoveryRate;
        public double RecoveryRate
        {
            get { return recoveryRate; }
            set
            {
                recoveryRate = value;
                NotifyOfPropertyChange(() => RecoveryRate);
            }
        }

        private double cdsCoupon;
        public double CdsCoupon
        {
            get { return cdsCoupon; }
            set
            {
                cdsCoupon = value;
                NotifyOfPropertyChange(() => CdsCoupon);
            }
        }

        private double notional;
        public double Notional
        {
            get { return notional; }
            set
            {
                notional = value;
                NotifyOfPropertyChange(() => Notional);
            }
        }

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
        public string XLabel1
        {
            get { return xlabel1; }
            set
            {
                xlabel1 = value;
                NotifyOfPropertyChange(() => XLabel1);
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

        private string xlabel2 = string.Empty;
        public string XLabel2
        {
            get { return xlabel2; }
            set
            {
                xlabel2 = value;
                NotifyOfPropertyChange(() => XLabel2);
            }
        }

        private string ylabel2= string.Empty;
        public string YLabel2
        {
            get { return ylabel2; }
            set
            {
                ylabel2 = value;
                NotifyOfPropertyChange(() => YLabel2);
            }
        }

        private string y2label2 = string.Empty;
        public string Y2Label2
        {
            get { return y2label2; }
            set
            {
                y2label2 = value;
                NotifyOfPropertyChange(() => Y2Label2);
            }
        }
        

        public void HazardRate()
        {
            string[] ss = Spreads.Replace(" ", "").Split(new char[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);
            double[] mySpreads = new double[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                mySpreads[i] = ss[i].To<double>();
            string[] myTenors = Tenors.Replace(" ", "").Split(new char[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);
            DataTable dt1 = QuantLibFIHelper.CdsHazardRate(EvalDate.To<Date>(), RecoveryRate, mySpreads, myTenors, true);
            DataTable dt2 = QuantLibFIHelper.CdsHazardRate(EvalDate.To<Date>(), RecoveryRate, mySpreads, myTenors, false);
            Table1 = dt1;
            Table2 = dt2;

            AddCharts();
        }

        private void AddCharts()
        {
            Title1 = "Hazard Rate";
            XLabel1 = "Maturity (Years)";
            YLabel1 = "Hazard Rate (%)";
            Title2 = "Survival Probability";
            XLabel2 = "Maturity (Years)";
            YLabel2 = "Survival Probability (%)";
            Y2Label2 = "Default Probability (%)";

            LineSeriesCollection1.Clear();
            Series ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.XValueMember = "TimesToMaturity";
            ds.YValueMembers = "Hazard Rate (%)";
            LineSeriesCollection1.Add(ds);

            LineSeriesCollection2.Clear();
            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.XValueMember = "TimesToMaturity";
            ds.YValueMembers = "Survival Probability (%)";
            ds.Name = "Survival";
            LineSeriesCollection2.Add(ds);

            ds = new Series();
            ds.ChartType = SeriesChartType.Line;
            ds.XValueMember = "TimesToMaturity";
            ds.YValueMembers = "Default Probability (%)";
            ds.Name = "Default";
            ds.YAxisType = AxisType.Secondary;
            LineSeriesCollection2.Add(ds);
        }


        public void StartCdsPV()
        {
            LineSeriesCollection1.Clear();
            LineSeriesCollection2.Clear();
           
            Protection.Side side;
            if (ProtectionSide.ToUpper() == "BUYER")
                side = Protection.Side.Buyer;
            else
                side = Protection.Side.Seller;

            string[] ss = Spreads.Replace(" ", "").Split(new char[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);
            double[] mySpreads = new double[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                mySpreads[i] = ss[i].To<double>();
            string[] myTenors = Tenors.Replace(" ", "").Split(new char[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);

          
            DataTable dt = QuantLibFIHelper.CdsPV(side, "USD", EvalDate.To<Date>(), EffectiveDate.To<Date>(), Maturity.To<Date>(),
                RecoveryRate, mySpreads, myTenors, Notional, Frequency.Quarterly, CdsCoupon);
            Table1 = dt;
        }

        public void StartCdsPrice()
        {
            LineSeriesCollection1.Clear();
            LineSeriesCollection2.Clear();

            Protection.Side side;
            if (ProtectionSide.ToUpper() == "BUYER")
                side = Protection.Side.Buyer;
            else
                side = Protection.Side.Seller;

            string[] ss = Spreads.Replace(" ", "").Split(new char[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);
            double[] mySpreads = new double[ss.Length];
            for (int i = 0; i < ss.Length; i++)
                mySpreads[i] = ss[i].To<double>();
            string[] myTenors = Tenors.Replace(" ", "").Split(new char[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);


            DataTable dt = QuantLibFIHelper.CdsPrice(side, "USD", EvalDate.To<Date>(), EffectiveDate.To<Date>(), Maturity.To<Date>(),
                RecoveryRate, mySpreads, myTenors, Frequency.Quarterly, CdsCoupon);
            Table2 = dt;
        }
    }
}
