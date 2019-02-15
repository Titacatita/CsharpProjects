using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using QuantLib;
using QuantBook.Models;
using QuantBook.Models.FixedIncome;

namespace QuantBook.Ch10
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class SimpleBondViewModel : Screen
    {
        [ImportingConstructor]
        public SimpleBondViewModel()
        {
            DisplayName = "01. Simple Bonds";
            IssueDate = new DateTime(2015, 12, 16);
            EvalDate = new DateTime(2015, 12, 16);
            SelectedFrequency = QuantLib.Frequency.Annual;

            BondTable = new DataTable();
        }

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

        private DateTime issueDate;
        public DateTime IssueDate
        {
            get { return issueDate; }
            set
            {
                issueDate = value;
                NotifyOfPropertyChange(() => IssueDate);
            }
        }

        private int settlementDays = 1;
        public int SettlementDays
        {
            get { return settlementDays; }
            set
            {
                settlementDays = value;
                NotifyOfPropertyChange(() => SettlementDays);
            }
        }

        private double faceValue = 1000;
        public double FaceValue
        {
            get { return faceValue; }
            set
            {
                faceValue = value;
                NotifyOfPropertyChange(() => FaceValue);
            }
        }

        private double discountRate = 0.06;
        public double DiscountRate
        {
            get { return discountRate; }
            set
            {
                discountRate = value;
                NotifyOfPropertyChange(() => DiscountRate);
            }
        }

        private int timesToMaturity = 10;
        public int TimesToMaturity
        {
            get { return timesToMaturity; }
            set
            {
                timesToMaturity = value;
                NotifyOfPropertyChange(() => TimesToMaturity);
            }
        }
        private string coupons = "0.05";
        public string Coupons
        {
            get { return coupons; }
            set
            {
                coupons = value;
                NotifyOfPropertyChange(() => Coupons);
            }
        }

        private DataTable bondTable;
        public DataTable BondTable
        {
            get { return bondTable; }
            set
            {
                bondTable = value;
                NotifyOfPropertyChange(() => BondTable);
            }
        }

        private IEnumerable<Frequency> frequency;
        public IEnumerable<Frequency> Frequency
        {
            get { return Enum.GetValues(typeof(Frequency)).Cast<Frequency>(); }
            set
            {
                frequency = value;
                NotifyOfPropertyChange(() => Frequency);
            }
        }

        private Frequency selectedFrequency;
        public Frequency SelectedFrequency
        {
            get { return selectedFrequency; }
            set
            {
                selectedFrequency = value;
                NotifyOfPropertyChange(() => SelectedFrequency);
            }
        }

        public void Start()
        {
            DateTime maturity = IssueDate.AddYears(TimesToMaturity);
            string[] couponstr = Coupons.Split(',');
            double[] coupon = new double[couponstr.Length];
            for (int i = 0; i < couponstr.Length; i++)
                coupon[i] = couponstr[i].To<double>();

            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Value", typeof(string));
            object[] bond = QuantLibFIHelper.BondPrice(evalDate, issueDate, maturity, SettlementDays, FaceValue, DiscountRate, coupon, SelectedFrequency);
            dt.Rows.Add("Pricing Bond", "Using QuantLib");
            dt.Rows.Add("Issue Date", IssueDate);
            dt.Rows.Add("Evaluation Date", EvalDate);
            dt.Rows.Add("Times to Maturity in Years", TimesToMaturity);
            dt.Rows.Add("Face Value", FaceValue);
            dt.Rows.Add("Discount Rate", DiscountRate);
            dt.Rows.Add("Coupon", Coupons);
            dt.Rows.Add("Present Value", bond[0]);
            dt.Rows.Add("Clean Price", bond[1]);
            dt.Rows.Add("Dirty Price", bond[2]);
            dt.Rows.Add("Accrued Value", bond[3]);
            dt.Rows.Add("YTM", bond[4]);

            BondTable = dt;           
        }

        public void StartCurveRate()
        {
            DataTable dt = QuantLibFIHelper.BondPriceCurveRate();
            BondTable = dt;
        }

        public void StartZSpread()
        {
            DataTable dt = QuantLibFIHelper.BondPriceCurveRateZSpread();
            BondTable = dt;
        }


    }
}
