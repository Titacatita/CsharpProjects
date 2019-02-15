using System;
using Caliburn.Micro;
using QuantBook.Models;
using System.Collections.Generic;
//using QLNet;
using QuantLib;
using QuantBook.Models.FixedIncome;
using QuantBook.Models.Utilities;

namespace QuantBook.Ch10
{
    //[Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class DateViewModel : Screen
    {
         private readonly IEventAggregator _events;
        //[ImportingConstructor]
        public DateViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "01. Date";

           //Date d1 = new Date(25,11,2015);
            Date d1 = new Date(42333);
            Date d2 = new Date(25, Month.November, 2015);
            Date dd = d1 - 5;
            Period p1 = new Period(5, TimeUnit.Weeks);
            Period p2 = new Period(Frequency.Bimonthly);
            Period p3 = new Period(14, TimeUnit.Days);
            

            Calendar cal = new Germany(Germany.Market.FrankfurtStockExchange);
            Date d = new Date(6, Month.December, 2015);
            
            Date newd = d1.Add(p1);
            //MessageBox.Show(string.Format("p1={0},p2={1},p3={2}",cal.isBusinessDay(d),cal.isHoliday(d) ));

            //TermStructure ts = new TermStructure();


            /*Date d1 = new Date(15, Month.Jul, 2015);
            Date d2 = new Date(25, Month.Nov, 2015);
             
            DayCounter dc1 = new Actual365Fixed();
            DayCounter dc2 = new SimpleDayCounter();
            
           
            int dcc1 = dc1.dayCount(d1, d2);
            int dcc2 = dc2.dayCount(d1, d2);
            var frac1 = dc1.yearFraction(d1, d2);
            var frac2 = dc2.yearFraction(d1, d2);
            //MessageBox.Show(string.Format("d1={0}, d2={1}, f1={2}, f2={3}", dcc1,dcc2,frac1,frac2 ));
            
            //int dc1 = dc.dayCount(d1, d2);
            //var frac = dc.yearFraction(d1, d2);*/

            
        }

        public void TestBond()
        {
            //object[] bond = QlFixedIncomeHelper.BondPrice3();
            Date evalDate = new Date(15, Month.December, 2015);
            Date issueDate = evalDate;
            Date maturity = issueDate.Add(new Period(10, TimeUnit.Years));
            
            double faceValue = 1000.0;
            double rate = 0.06;
            double[] coupon = new double[] { 0.06 };
            //object[] bond = QuantLibFIHelper.BondPrice(evalDate, issueDate, maturity, faceValue, rate, coupon, Frequency.Annual);

           // string ss = string.Format("npv={0}, CleanPrice={1}, dirtyPrice={2}, accrued={3}", bond[0], bond[1], bond[2], bond[3]);
           // _events.PublishOnUIThread(new ModelEvents(new List<object> { ss }));
        }

        public void TestRate()
        {
            /*InterestRate[] rate = QlFixedIncomeHelper.InterestRate(0.08);
            string ss = string.Format("oneYearRate={0}, SemiRate={1}, ContinuousRate={2}", rate[0].rate(), rate[1].rate(), rate[2].rate());
            _events.PublishOnUIThread(new ModelEvents(new List<object> { ss }));*/

            //QlFixedIncomeHelper.ZeroCoupon2();

            //ConsoleManager.Show();
            //QlFixedIncomeHelper.SwapEvaluation();

            ConsoleManager.Show();
            QLNetFIHelper.CdsTermStructure("USD", DateTime.Today.AddDays(-1), _events);
            //QlFixedIncomeHelper.Bootstrap();
        }

       
    }
}
