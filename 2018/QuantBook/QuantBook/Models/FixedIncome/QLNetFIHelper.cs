using System;
using System.Collections.Generic;
using QLNet;
using System.Data;
using Caliburn.Micro;
using QuantBook.Models.Isda;

namespace QuantBook.Models.FixedIncome
{
    public static class QLNetFIHelper
    {
      
        #region Bond Pricing
        public static DataTable CallableBondPrice(Date evalDate, Date issueDate, Date callDate, Date maturity, double coupon, double sigma, double a, double rate)
        {            
            Settings.setEvaluationDate(evalDate);
            DayCounter dc = new ActualActual(ActualActual.Convention.Bond);
            Handle<YieldTermStructure> flatCurveHandle = new Handle<YieldTermStructure>(new FlatForward(evalDate,
                                                                 rate,
                                                                 dc,
                                                                 Compounding.Compounded,
                                                                 Frequency.Semiannual));
            CallabilitySchedule callSchedule = new CallabilitySchedule();
            double callPrice = 100.0;
            int numberOfCallDates = 24;

            for (int i = 0; i < numberOfCallDates; i++)
            {
                Calendar nullCalendar = new NullCalendar();
                Callability.Price myPrice = new Callability.Price(callPrice, Callability.Price.Type.Clean);
                callSchedule.Add(new Callability(myPrice, Callability.Type.Call, callDate));
                callDate = nullCalendar.advance(callDate, 3, TimeUnit.Months);
            }


            int settlementDays = 3; 
            Calendar bondCalendar = new UnitedStates(UnitedStates.Market.GovernmentBond);
            Frequency frequency = Frequency.Quarterly;
            double redemption = 100.0;
            double faceAmount = 100.0;

            DayCounter bondDayCounter = new ActualActual(ActualActual.Convention.Bond);
            BusinessDayConvention accrualConvention = BusinessDayConvention.Unadjusted;
            BusinessDayConvention paymentConvention = BusinessDayConvention.Unadjusted;

            Schedule sch = new Schedule(issueDate, maturity, new Period(frequency), bondCalendar,
                                         accrualConvention, accrualConvention,
                                         DateGeneration.Rule.Backward, false);

            int maxIterations = 1000;
            double accuracy = 1e-8;
            int gridIntervals = 40;
            ShortRateModel hw;

            if(sigma<=0)
                sigma = Const.QL_EPSILON;

            hw = new HullWhite(flatCurveHandle, a, sigma);

            IPricingEngine engine = new TreeCallableFixedRateBondEngine(hw, gridIntervals, flatCurveHandle);

            CallableFixedRateBond callableBond = new CallableFixedRateBond(settlementDays, faceAmount, sch,
                                                                            new InitializedList<double>(1, coupon),
                                                                            bondDayCounter, paymentConvention,
                                                                            redemption, issueDate, callSchedule);
           
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Value", typeof(string));
            dt.Rows.Add("Pricing Bond", "Using QLNet");
            dt.Rows.Add("Issue Date", issueDate);
            dt.Rows.Add("Evaluation Date", evalDate);
            dt.Rows.Add("Maturity", maturity);            
            dt.Rows.Add("Flat Rate", rate);
            dt.Rows.Add("Reversion a", a);
            dt.Rows.Add("sigma", sigma);
            dt.Rows.Add("Coupon", coupon);

            try
            {
                callableBond.setPricingEngine(engine);
                dt.Rows.Add("Present Value", callableBond.NPV());
                dt.Rows.Add("Clean Price", callableBond.cleanPrice());
                dt.Rows.Add("Dirty Price",callableBond.dirtyPrice());
                dt.Rows.Add("Accrued Value",callableBond.accruedAmount());
                dt.Rows.Add("YTM", callableBond.yield(bondDayCounter, Compounding.Compounded, frequency, accuracy, maxIterations));
            }
            catch { }

            return dt;
        }

        




        public static void Bootstrap()
        {
            //http://billiontrader.com/post/102
            Calendar calendar = new JointCalendar(new UnitedKingdom(UnitedKingdom.Market.Exchange), new UnitedStates(UnitedStates.Market.Settlement));
            Date settlementDate = new Date(18, 2, 2015);
            int fixingDays = 2;
            Date todaysDate = calendar.advance(settlementDate, -fixingDays, TimeUnit.Days);
            Settings.setEvaluationDate(todaysDate);
            DayCounter depoDayCounter = new Actual360();

            List<RateHelper> rateHelpers = new List<RateHelper>();


            //deposit:
            List<double> depoRates = new List<double>() { 0.001375, 0.001717, 0.002112, 0.002581 };
            List<Period> depoMaturities = new List<Period>()
            {
                new Period(1,TimeUnit.Weeks),
                new Period(1,TimeUnit.Months),
                new Period(2,TimeUnit.Months),
                new Period(3,TimeUnit.Months)
            };

            for (int i = 0; i < depoRates.Count; i++)
            {
                rateHelpers.Add(new DepositRateHelper(new Handle<Quote>(new SimpleQuote(depoRates[i])), depoMaturities[i], fixingDays,
                    calendar, BusinessDayConvention.ModifiedFollowing, true, depoDayCounter));
            }



            //Futures:
            DayCounter futDayCounter = new Actual360();
            List<double> futQuotes = new List<double>() { 99.725, 99.585, 99.385, 99.16, 98.93, 98.715 };
            Date imm = IMM.nextDate(settlementDate);
            int futMonths = 3;
            for (int i = 0; i < futQuotes.Count; i++)
            {
                rateHelpers.Add(new FuturesRateHelper(new Handle<Quote>(new SimpleQuote(futQuotes[i])), imm, futMonths, calendar, BusinessDayConvention.ModifiedFollowing, true, futDayCounter));
                imm = IMM.nextDate(imm + 1);
            }



            //swap rates:
            List<double> swapRates = new List<double>() { 0.0089268, 0.0123343, 0.0147985, 0.0165843, 0.0179191 };
            List<Period> swapMaturities = new List<Period>()
            {
                new Period(2,TimeUnit.Years),
                new Period(3,TimeUnit.Years),
                new Period(4,TimeUnit.Years),
                new Period(5,TimeUnit.Years),
                new Period(6,TimeUnit.Years)
            };

            Frequency swFixedLegFrequency = Frequency.Annual;
            BusinessDayConvention swFixedLegConvention = BusinessDayConvention.Unadjusted;
            DayCounter swFixedLegDayCounter = new Actual360();
            IborIndex swFloatingLegIndex = new USDLibor(new Period(3, TimeUnit.Months));

            for (int i = 0; i < swapRates.Count; i++)
            {
                rateHelpers.Add(new SwapRateHelper(new Handle<Quote>(new SimpleQuote(swapRates[i])), swapMaturities[i], calendar,
                    swFixedLegFrequency, swFixedLegConvention, swFixedLegDayCounter, swFloatingLegIndex));
            }


            //term structure:
            DayCounter termStructureDayCounter = new Actual360();
            PiecewiseYieldCurve<Discount, LogCubic> depoFutSwapTermStructure = new PiecewiseYieldCurve<Discount, LogCubic>(settlementDate, rateHelpers, termStructureDayCounter);

            // utilities for reporting
            List<string> headers1 = new List<string>();
            headers1.Add("Maturity   ");
            headers1.Add("Eq_Rate  ");
            headers1.Add("Zero_Rate");
            headers1.Add("Discount ");
            string separator1 = " | ";
            int width1 = headers1[0].Length + separator1.Length
                       + headers1[1].Length + separator1.Length
                       + headers1[2].Length + separator1.Length
                       + headers1[3].Length + separator1.Length - 1;
            string rule1 = string.Format("").PadLeft(width1, '-'), dblrule1 = string.Format("").PadLeft(width1, '=');
            string tab1 = string.Format("").PadLeft(8, ' ');

            Console.WriteLine(dblrule1);
            Console.WriteLine("{0}: CDS Term Structure Curve", settlementDate);
            Console.WriteLine(rule1);
            Console.WriteLine(headers1[0] + separator1
                      + headers1[1] + separator1
                      + headers1[2] + separator1
                      + headers1[3] + separator1);
            Console.WriteLine(rule1);

            int count = 1;
            foreach (Date d in depoFutSwapTermStructure.dates())
            {
                DayCounter dc = depoDayCounter;
                Compounding compound = Compounding.Simple;
                if (count > 4)
                    dc = futDayCounter;
                if (count > 10)
                    compound = Compounding.Compounded;

                double yrs = dc.yearFraction(settlementDate, d);
                //var mat = settlementDate + d;
                InterestRate zero_rate = depoFutSwapTermStructure.zeroRate(yrs, compound, Frequency.Annual);
                double eq_rate = zero_rate.equivalentRate(dc, compound, Frequency.Annual, settlementDate, d).rate();
                double discount = depoFutSwapTermStructure.discount(d);
                Console.Write("{0," + headers1[0].Length + ":00.00000}" + separator1, d);
                Console.Write("{0," + headers1[1].Length + ":0.000000}" + separator1, 100.0 * eq_rate);
                Console.Write("{0," + headers1[2].Length + ":0.000000}" + separator1, 100.0 * zero_rate.rate());
                Console.WriteLine("{0," + headers1[3].Length + ":0.000000}" + separator1, discount);
                count++;
            }
        }




        public static void CdsPricing(string ccy, Date evalDate, double notional, double coupon, double recovery, DataTable dtHazard)
        {
            Calendar calendar = new TARGET();
            Date todaysDate = new Date(15, 5, 2007);
            Settings.setEvaluationDate(todaysDate);

            // dummy curve:
            //var tsCurve = new FlatForward(todaysDate, 0.01, new Actual365Fixed());
            YieldTermStructure tsCurve = CdsZeroCurve(ccy, evalDate);
            List<Date> maturities = new List<Date>();
            List<double> hazardRates = new List<double>();
            foreach (DataRow r in dtHazard.Rows)
            {
                maturities.Add(r["Maturity"].To<DateTime>());
                hazardRates.Add(0.01 * r["HazardRate"].To<double>());
            }

            /*List<DefaultProbabilityTermStructure> probs = new List<DefaultProbabilityTermStructure>();
            for (int i = 0; i < hazardRates.Count; i++)
            {
                probs.Add(new FlatHazardRate(maturities[i], hazardRates[i], new Actual360()));
            }*/
            InterpolatedHazardRateCurve<ForwardFlat> hazardCurve = new InterpolatedHazardRateCurve<ForwardFlat>(maturities,hazardRates,new Thirty360());

            Handle<YieldTermStructure> discountCurve = new Handle<YieldTermStructure>(tsCurve);
            MidPointCdsEngine engine = new MidPointCdsEngine(new Handle<DefaultProbabilityTermStructure>(hazardCurve), recovery, discountCurve);
             for (int i = 0; i < maturities.Count; i++)
             {
                 Schedule cdsSchedule = new Schedule(evalDate, maturities[i], new Period(Frequency.Quarterly), calendar, BusinessDayConvention.Following,
                     BusinessDayConvention.Following, DateGeneration.Rule.TwentiethIMM, false);
                 CreditDefaultSwap cds = new CreditDefaultSwap(Protection.Side.Buyer, notional, coupon / 10000.0, cdsSchedule, BusinessDayConvention.ModifiedFollowing, new Actual360(), false);
                 cds.setPricingEngine(engine);
                 System.Windows.MessageBox.Show(string.Format("upfron={0}, uf={1}", cds.upfrontNPV(), cds.NPV()));
             }



        }


        private static List<Period> TenorStringToPeriodVector(string[] tenors)
        {
            List<Period> periods = new List<Period>();
            foreach (string s in tenors)
            {
                int num = s.Remove(s.Length - 1, 1).To<int>();
                string sub = s.Substring(s.Length - 1, 1).ToUpper();

                if (sub == "M")
                    periods.Add(new Period(num, TimeUnit.Months));
                else if (sub == "Y")
                    periods.Add(new Period(num, TimeUnit.Years));
            }
            return periods;
        }

        public static YieldTermStructure CdsZeroCurve(string ccy, DateTime date)
        {
            BindableCollection<IsdaRate> isdaRates = new BindableCollection<IsdaRate>();
            isdaRates = IsdaHelper.GetIsdaRates(ccy, date, date);

            List<double> depoRates = new List<double>();
            List<Period> depoMaturities = new List<Period>();
            List<double> swapRates = new List<double>();
            List<Period> swapMaturities = new List<Period>();
            DayCounter dc = new Thirty360(Thirty360.Thirty360Convention.USA);

            foreach (IsdaRate item in isdaRates)
            {
                int num = 0;
                if (string.IsNullOrEmpty(item.FixedDayCountConvention))
                {
                    if (item.Tenor.Contains("M"))
                    {
                        num = Convert.ToInt32(item.Tenor.Split('M')[0]);
                        depoMaturities.Add(new Period(num, TimeUnit.Months));
                        depoRates.Add(Convert.ToDouble(item.Rate));
                    }
                    else if (item.Tenor.Contains("Y"))
                    {
                        num = Convert.ToInt32(item.Tenor.Split('Y')[0]);
                        depoMaturities.Add(new Period(num, TimeUnit.Years));
                        depoRates.Add(Convert.ToDouble(item.Rate));
                    }
                }
                else
                {
                    num = Convert.ToInt32(item.Tenor.Split('Y')[0]);
                    swapMaturities.Add(new Period(num, TimeUnit.Years));
                    swapRates.Add(Convert.ToDouble(item.Rate));
                }
            }

            Date calcDate = new Date(date.Day, date.Month, date.Year);
            Settings.setEvaluationDate(calcDate);
            Calendar calendar = new UnitedStates();

            List<RateHelper> rateHelpers = new List<RateHelper>();
            for (int i = 0; i < depoRates.Count; i++)
            {
                rateHelpers.Add(new DepositRateHelper(depoRates[i], depoMaturities[i], 0, calendar, BusinessDayConvention.Unadjusted, true, dc));
            }

            Frequency swFixedLegFrequency = Frequency.Annual;
            BusinessDayConvention swFixedLegConvention = BusinessDayConvention.Unadjusted;
            DayCounter swFixedLegDayCounter = new Thirty360(Thirty360.Thirty360Convention.USA);
            IborIndex swFloatingLegIndex = new USDLibor(new Period(3, TimeUnit.Months));
            for (int i = 0; i < swapRates.Count; i++)
            {
                rateHelpers.Add(new SwapRateHelper(new Handle<Quote>(new SimpleQuote(swapRates[i])), swapMaturities[i], calendar, swFixedLegFrequency, swFixedLegConvention, dc, swFloatingLegIndex));
                //rateHelpers.Add(new SwapRateHelper(new Handle<Quote>(new SimpleQuote(swapRates[i])), swapMaturities[i], calendar, swFixedLegFrequency, swFixedLegConvention,dc, new EuriborSW()));
            }


            PiecewiseYieldCurve<Discount, LogLinear> discountingTermStructure = new PiecewiseYieldCurve<Discount, LogLinear>(calcDate, rateHelpers, dc);
            return discountingTermStructure;
        }



        public static void CdsTermStructure(string ccy, DateTime date, IEventAggregator events)
        {
            BindableCollection<IsdaRate> isdaRates = new BindableCollection<IsdaRate>();
            isdaRates = IsdaHelper.GetIsdaRates(ccy, date, date, events);

            List<double> depoRates = new List<double>();
            List<Period> depoMaturities = new List<Period>();
            List<double> swapRates = new List<double>();
            List<Period> swapMaturities = new List<Period>();
            DayCounter dc = new Thirty360(Thirty360.Thirty360Convention.USA);

            foreach (IsdaRate item in isdaRates)
            {
                int num = 0;
                if (string.IsNullOrEmpty(item.FixedDayCountConvention))
                {                    
                    if (item.Tenor.Contains("M"))
                    {
                        num = Convert.ToInt32(item.Tenor.Split('M')[0]);
                        depoMaturities.Add(new Period(num, TimeUnit.Months));
                        depoRates.Add(Convert.ToDouble(item.Rate));
                    }
                    else if (item.Tenor.Contains("Y"))
                    {
                        num = Convert.ToInt32(item.Tenor.Split('Y')[0]);
                        depoMaturities.Add(new Period(num, TimeUnit.Years));
                        depoRates.Add(Convert.ToDouble(item.Rate));
                    }
                }
                else
                {
                    num = Convert.ToInt32(item.Tenor.Split('Y')[0]);
                    swapMaturities.Add(new Period(num, TimeUnit.Years));
                    swapRates.Add(Convert.ToDouble(item.Rate));
                }
            }

            Date calcDate = new Date(date.Day, date.Month, date.Year);
            Settings.setEvaluationDate(calcDate);
            Calendar calendar = new UnitedStates();

            List<RateHelper> rateHelpers = new List<RateHelper>();
            for (int i = 0; i < depoRates.Count; i++)
            {
                rateHelpers.Add(new DepositRateHelper(depoRates[i], depoMaturities[i], 0, calendar, BusinessDayConvention.Unadjusted, true, dc));
            }

            Frequency swFixedLegFrequency = Frequency.Annual;
            BusinessDayConvention swFixedLegConvention = BusinessDayConvention.Unadjusted;
            DayCounter swFixedLegDayCounter = new Thirty360(Thirty360.Thirty360Convention.USA);
            IborIndex swFloatingLegIndex = new USDLibor(new Period(3, TimeUnit.Months));
            for(int i =0;i<swapRates.Count;i++)
            {
                rateHelpers.Add(new SwapRateHelper(new Handle<Quote>(new SimpleQuote(swapRates[i])), swapMaturities[i], calendar, swFixedLegFrequency, swFixedLegConvention, dc, swFloatingLegIndex));
                //rateHelpers.Add(new SwapRateHelper(new Handle<Quote>(new SimpleQuote(swapRates[i])), swapMaturities[i], calendar, swFixedLegFrequency, swFixedLegConvention,dc, new EuriborSW()));
            }


            PiecewiseYieldCurve<Discount, LogLinear> discountingTermStructure = new PiecewiseYieldCurve<Discount, LogLinear>(calcDate, rateHelpers, dc);
            
            /*foreach (var d in discountingTermStructure.dates())
            {
                var yrs = dc.yearFraction(calcDate, d);
                var zero_rate = discountingTermStructure.zeroRate(yrs, Compounding.Compounded, Frequency.Annual);
                var eq_rate = zero_rate.equivalentRate(dc, Compounding.Compounded, Frequency.Annual, calcDate, d).rate();
                var discount = discountingTermStructure.discount(d);
                System.Windows.MessageBox.Show(string.Format("mat={0}, rate={1}, zero_rate={2}, discount={3}", yrs, eq_rate, zero_rate.rate(), discount));
            }*/


            // utilities for reporting
            List<string> headers1 = new List<string>();
            headers1.Add("Maturity ");
            headers1.Add("Eq_Rate  ");
            headers1.Add("Zero_Rate");
            headers1.Add("Discount ");
            string separator1 = " | ";
            int width1 = headers1[0].Length + separator1.Length
                       + headers1[1].Length + separator1.Length
                       + headers1[2].Length + separator1.Length
                       + headers1[3].Length + separator1.Length - 1;
            string rule1 = string.Format("").PadLeft(width1, '-'), dblrule1 = string.Format("").PadLeft(width1, '=');
            string tab1 = string.Format("").PadLeft(8, ' ');

            Console.WriteLine(dblrule1);
            Console.WriteLine("{0}: CDS Term Structure Curve", calcDate);
            Console.WriteLine(rule1);
            Console.WriteLine(headers1[0] + separator1
                      + headers1[1] + separator1
                      + headers1[2] + separator1
                      + headers1[3] + separator1);
            Console.WriteLine(rule1);

            foreach (Date d in discountingTermStructure.dates())
            {
                double yrs = dc.yearFraction(calcDate, d);
                InterestRate zero_rate = discountingTermStructure.zeroRate(yrs, Compounding.Compounded, Frequency.Annual);
                double eq_rate = zero_rate.equivalentRate(dc, Compounding.Compounded, Frequency.Annual, calcDate, d).rate();
                double discount = discountingTermStructure.discount(d);
                Console.Write("{0," + headers1[0].Length + ":00.00000}" + separator1, yrs);
                Console.Write("{0," + headers1[1].Length + ":0.000000}" + separator1, 100.0 * eq_rate);
                Console.Write("{0," + headers1[2].Length + ":0.000000}" + separator1, 100.0 * zero_rate.rate());
                Console.WriteLine("{0," + headers1[3].Length + ":0.000000}" + separator1, discount);
            }

        }


        public static void ZeroCoupon()
        {
            List<double> depo_rates = new List<double>() { 5.25, 5.5 };
            List<Period> depo_maturities = new List<Period>() { new Period(6, TimeUnit.Months), new Period(12, TimeUnit.Months) };

            List<double> bond_rates = new List<double>() { 5.75, 6.0, 6.25, 6.5, 6.75, 6.80, 7.00, 7.1, 7.15, 7.2, 7.3, 7.35, 7.4, 7.5, 7.6, 7.6, 7.7, 7.8 };
            List<Period> bond_maturities = new List<Period>();
            for (int i = 3; i <= 20; i++)
            {
                bond_maturities.Add(new Period(i * 6, TimeUnit.Months));
            }

            Date calc_date = new Date(15, 1, 2015);
            Settings.setEvaluationDate(calc_date);
            Calendar calendar = new UnitedStates();

            List<RateHelper> rateHelpers = new List<RateHelper>();
            for (int i = 0; i < depo_rates.Count; i++)
            {
                rateHelpers.Add(new DepositRateHelper(depo_rates[i] / 100.0, depo_maturities[i], 0, calendar, BusinessDayConvention.Unadjusted, true, new Thirty360()));
            }

            for (int i = 0; i < bond_rates.Count; i++)
            {
                Date termination_date = calc_date + bond_maturities[i];
                Schedule schedule = new Schedule(calc_date, termination_date, new Period(Frequency.Semiannual), calendar, BusinessDayConvention.Unadjusted,
                    BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, true);
                rateHelpers.Add(new FixedRateBondHelper(new Handle<Quote>(new SimpleQuote(100.0)), 0, 100.0, schedule, new List<double>() { bond_rates[i] / 100.0 },
                    new Thirty360(), BusinessDayConvention.Unadjusted, 100.0, calc_date));
            }

            PiecewiseYieldCurve<Discount, LogLinear> discountingTermStructure = new PiecewiseYieldCurve<Discount, LogLinear>(calc_date, rateHelpers, new Thirty360());
            DayCounter dc = new Thirty360();

            foreach (Date d in discountingTermStructure.dates())
            {
                double yrs = dc.yearFraction(calc_date, d);
                InterestRate zero_rate = discountingTermStructure.zeroRate(yrs, Compounding.Compounded, Frequency.Semiannual);
                double eq_rate = zero_rate.equivalentRate(dc, Compounding.Compounded, Frequency.Semiannual, calc_date, d).rate();
                double discount = discountingTermStructure.discount(d);
                System.Windows.MessageBox.Show(string.Format("mat={0}, rate={1}, zero_rate={2}, discount={3}", yrs, eq_rate, zero_rate.rate(), discount));
            }

        }





















        public static void SwapEvaluation()
        {
            DateTime timer = DateTime.Now;
            string output = string.Empty;

            Calendar calendar = new TARGET();

            Date settlementDate = new Date(22, Month.September, 2004);
            // must be a business day
            settlementDate = calendar.adjust(settlementDate);

            int fixingDays = 2;
            Date todaysDate = calendar.advance(settlementDate, -fixingDays, TimeUnit.Days);
            // nothing to do with Date::todaysDate
            Settings.setEvaluationDate(todaysDate);


            todaysDate = Settings.evaluationDate();
            Console.WriteLine("Today: {0}, {1}", todaysDate.DayOfWeek, todaysDate);
            Console.WriteLine("Settlement date: {0}, {1}", settlementDate.DayOfWeek, settlementDate);
            output += string.Format("\nToday: {0}, {1}", todaysDate.DayOfWeek, todaysDate);
            output += string.Format("\nSettlement date: {0}, {1}", settlementDate.DayOfWeek, settlementDate);

            // deposits
            double d1wQuote = 0.0382;
            double d1mQuote = 0.0372;
            double d3mQuote = 0.0363;
            double d6mQuote = 0.0353;
            double d9mQuote = 0.0348;
            double d1yQuote = 0.0345;
            // FRAs
            double fra3x6Quote = 0.037125;
            double fra6x9Quote = 0.037125;
            double fra6x12Quote = 0.037125;
            // futures
            double fut1Quote = 96.2875;
            double fut2Quote = 96.7875;
            double fut3Quote = 96.9875;
            double fut4Quote = 96.6875;
            double fut5Quote = 96.4875;
            double fut6Quote = 96.3875;
            double fut7Quote = 96.2875;
            double fut8Quote = 96.0875;
            // swaps
            double s2yQuote = 0.037125;
            double s3yQuote = 0.0398;
            double s5yQuote = 0.0443;
            double s10yQuote = 0.05165;
            double s15yQuote = 0.055175;


            /********************
            ***    QUOTES    ***
            ********************/

            // SimpleQuote stores a value which can be manually changed;
            // other Quote subclasses could read the value from a database
            // or some kind of data feed.

            // deposits
            Quote d1wRate = new SimpleQuote(d1wQuote);
            Quote d1mRate = new SimpleQuote(d1mQuote);
            Quote d3mRate = new SimpleQuote(d3mQuote);
            Quote d6mRate = new SimpleQuote(d6mQuote);
            Quote d9mRate = new SimpleQuote(d9mQuote);
            Quote d1yRate = new SimpleQuote(d1yQuote);
            // FRAs
            Quote fra3x6Rate = new SimpleQuote(fra3x6Quote);
            Quote fra6x9Rate = new SimpleQuote(fra6x9Quote);
            Quote fra6x12Rate = new SimpleQuote(fra6x12Quote);
            // futures
            Quote fut1Price = new SimpleQuote(fut1Quote);
            Quote fut2Price = new SimpleQuote(fut2Quote);
            Quote fut3Price = new SimpleQuote(fut3Quote);
            Quote fut4Price = new SimpleQuote(fut4Quote);
            Quote fut5Price = new SimpleQuote(fut5Quote);
            Quote fut6Price = new SimpleQuote(fut6Quote);
            Quote fut7Price = new SimpleQuote(fut7Quote);
            Quote fut8Price = new SimpleQuote(fut8Quote);
            // swaps
            Quote s2yRate = new SimpleQuote(s2yQuote);
            Quote s3yRate = new SimpleQuote(s3yQuote);
            Quote s5yRate = new SimpleQuote(s5yQuote);
            Quote s10yRate = new SimpleQuote(s10yQuote);
            Quote s15yRate = new SimpleQuote(s15yQuote);


            /*********************
            ***  RATE HELPERS ***
            *********************/

            // RateHelpers are built from the above quotes together with
            // other instrument dependant infos.  Quotes are passed in
            // relinkable handles which could be relinked to some other
            // data source later.

            // deposits
            DayCounter depositDayCounter = new Actual360();

            RateHelper d1w = new DepositRateHelper(new Handle<Quote>(d1wRate), new Period(1, TimeUnit.Weeks),
                fixingDays, calendar, BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);
            RateHelper d1m = new DepositRateHelper(new Handle<Quote>(d1mRate), new Period(1, TimeUnit.Months),
                fixingDays, calendar, BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);
            RateHelper d3m = new DepositRateHelper(new Handle<Quote>(d3mRate), new Period(3, TimeUnit.Months),
                fixingDays, calendar, BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);
            RateHelper d6m = new DepositRateHelper(new Handle<Quote>(d6mRate), new Period(6, TimeUnit.Months),
                fixingDays, calendar, BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);
            RateHelper d9m = new DepositRateHelper(new Handle<Quote>(d9mRate), new Period(9, TimeUnit.Months),
                fixingDays, calendar, BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);
            RateHelper d1y = new DepositRateHelper(new Handle<Quote>(d1yRate), new Period(1, TimeUnit.Years),
                fixingDays, calendar, BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);

            // setup FRAs
            RateHelper fra3x6 = new FraRateHelper(new Handle<Quote>(fra3x6Rate), 3, 6, fixingDays, calendar,
                        BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);
            RateHelper fra6x9 = new FraRateHelper(new Handle<Quote>(fra6x9Rate), 6, 9, fixingDays, calendar,
                        BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);
            RateHelper fra6x12 = new FraRateHelper(new Handle<Quote>(fra6x12Rate), 6, 12, fixingDays, calendar,
                        BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);


            // setup futures
            // Handle<Quote> convexityAdjustment = new Handle<Quote>(new SimpleQuote(0.0));
            int futMonths = 3;
            Date imm = IMM.nextDate(settlementDate);

            RateHelper fut1 = new FuturesRateHelper(new Handle<Quote>(fut1Price), imm, futMonths, calendar,
                    BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);

            imm = IMM.nextDate(imm + 1);
            RateHelper fut2 = new FuturesRateHelper(new Handle<Quote>(fut2Price), imm, futMonths, calendar,
                    BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);

            imm = IMM.nextDate(imm + 1);
            RateHelper fut3 = new FuturesRateHelper(new Handle<Quote>(fut3Price), imm, futMonths, calendar,
                    BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);

            imm = IMM.nextDate(imm + 1);
            RateHelper fut4 = new FuturesRateHelper(new Handle<Quote>(fut4Price), imm, futMonths, calendar,
                    BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);

            imm = IMM.nextDate(imm + 1);
            RateHelper fut5 = new FuturesRateHelper(new Handle<Quote>(fut5Price), imm, futMonths, calendar,
                    BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);

            imm = IMM.nextDate(imm + 1);
            RateHelper fut6 = new FuturesRateHelper(new Handle<Quote>(fut6Price), imm, futMonths, calendar,
                    BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);

            imm = IMM.nextDate(imm + 1);
            RateHelper fut7 = new FuturesRateHelper(new Handle<Quote>(fut7Price), imm, futMonths, calendar,
                    BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);

            imm = IMM.nextDate(imm + 1);
            RateHelper fut8 = new FuturesRateHelper(new Handle<Quote>(fut8Price), imm, futMonths, calendar,
                    BusinessDayConvention.ModifiedFollowing, true, depositDayCounter);


            // setup swaps
            Frequency swFixedLegFrequency = Frequency.Annual;
            BusinessDayConvention swFixedLegConvention = BusinessDayConvention.Unadjusted;
            DayCounter swFixedLegDayCounter = new Thirty360(Thirty360.Thirty360Convention.European);

            IborIndex swFloatingLegIndex = new Euribor6M();

            RateHelper s2y = new SwapRateHelper(new Handle<Quote>(s2yRate), new Period(2, TimeUnit.Years),
                calendar, swFixedLegFrequency, swFixedLegConvention, swFixedLegDayCounter, swFloatingLegIndex);
            RateHelper s3y = new SwapRateHelper(new Handle<Quote>(s3yRate), new Period(3, TimeUnit.Years),
                calendar, swFixedLegFrequency, swFixedLegConvention, swFixedLegDayCounter, swFloatingLegIndex);
            RateHelper s5y = new SwapRateHelper(new Handle<Quote>(s5yRate), new Period(5, TimeUnit.Years),
                calendar, swFixedLegFrequency, swFixedLegConvention, swFixedLegDayCounter, swFloatingLegIndex);
            RateHelper s10y = new SwapRateHelper(new Handle<Quote>(s10yRate), new Period(10, TimeUnit.Years),
                calendar, swFixedLegFrequency, swFixedLegConvention, swFixedLegDayCounter, swFloatingLegIndex);
            RateHelper s15y = new SwapRateHelper(new Handle<Quote>(s15yRate), new Period(15, TimeUnit.Years),
                calendar, swFixedLegFrequency, swFixedLegConvention, swFixedLegDayCounter, swFloatingLegIndex);



            /*********************
            **  CURVE BUILDING **
            *********************/

            // Any DayCounter would be fine.
            // ActualActual::ISDA ensures that 30 years is 30.0
            DayCounter termStructureDayCounter = new ActualActual(ActualActual.Convention.ISDA);

            double tolerance = 1.0e-15;

            // A depo-swap curve
            List<RateHelper> depoSwapInstruments = new List<RateHelper>();
            depoSwapInstruments.Add(d1w);
            depoSwapInstruments.Add(d1m);
            depoSwapInstruments.Add(d3m);
            depoSwapInstruments.Add(d6m);
            depoSwapInstruments.Add(d9m);
            depoSwapInstruments.Add(d1y);
            depoSwapInstruments.Add(s2y);
            depoSwapInstruments.Add(s3y);
            depoSwapInstruments.Add(s5y);
            depoSwapInstruments.Add(s10y);
            depoSwapInstruments.Add(s15y);
            PiecewiseYieldCurve<Discount, LogLinear> depoSwapTermStructure = new PiecewiseYieldCurve<Discount, LogLinear>(
                        settlementDate, depoSwapInstruments, termStructureDayCounter, new List<Handle<Quote>>(), new List<Date>(), tolerance);

            // utilities for reporting
            List<string> headers1 = new List<string>();
            headers1.Add("Maturity ");
            headers1.Add("Eq_Rate  ");
            headers1.Add("Zero_Rate");
            headers1.Add("Discount ");
            string separator1 = " | ";
            int width1 = headers1[0].Length + separator1.Length
                       + headers1[1].Length + separator1.Length
                       + headers1[2].Length + separator1.Length
                       + headers1[3].Length + separator1.Length - 1;
            string rule1 = string.Format("").PadLeft(width1, '-'), dblrule1 = string.Format("").PadLeft(width1, '=');
            string tab1 = string.Format("").PadLeft(8, ' ');

            Console.WriteLine(dblrule1);
            Console.WriteLine("DepoSwap Term Structure Curve");
            Console.WriteLine(rule1);
            Console.WriteLine(headers1[0] + separator1
                      + headers1[1] + separator1
                      + headers1[2] + separator1
                      + headers1[3] + separator1);
            Console.WriteLine(rule1);

            foreach (Date d in depoSwapTermStructure.dates())
            {
                double yrs = termStructureDayCounter.yearFraction(todaysDate, d);
                if (yrs >= 15)
                    yrs = 15.0;
                InterestRate zero_rate = depoSwapTermStructure.zeroRate(yrs, Compounding.Compounded, Frequency.Annual);
                double eq_rate = zero_rate.equivalentRate(termStructureDayCounter, Compounding.Compounded, Frequency.Annual, todaysDate, d).rate();
                double discount = depoSwapTermStructure.discount(d);
                Console.Write("{0," + headers1[0].Length + ":00.00000}" + separator1, yrs);
                Console.Write("{0," + headers1[1].Length + ":0.000000}" + separator1, eq_rate);
                Console.Write("{0," + headers1[2].Length + ":0.000000}" + separator1, zero_rate.rate());
                Console.WriteLine("{0," + headers1[3].Length + ":0.000000}" + separator1, discount);
            }




            // A depo-futures-swap curve
            List<RateHelper> depoFutSwapInstruments = new List<RateHelper>();
            depoFutSwapInstruments.Add(d1w);
            depoFutSwapInstruments.Add(d1m);
            depoFutSwapInstruments.Add(fut1);
            depoFutSwapInstruments.Add(fut2);
            depoFutSwapInstruments.Add(fut3);
            depoFutSwapInstruments.Add(fut4);
            depoFutSwapInstruments.Add(fut5);
            depoFutSwapInstruments.Add(fut6);
            depoFutSwapInstruments.Add(fut7);
            depoFutSwapInstruments.Add(fut8);
            depoFutSwapInstruments.Add(s3y);
            depoFutSwapInstruments.Add(s5y);
            depoFutSwapInstruments.Add(s10y);
            depoFutSwapInstruments.Add(s15y);
            PiecewiseYieldCurve<Discount, LogLinear> depoFutSwapTermStructure = new PiecewiseYieldCurve<Discount, LogLinear>(
                    settlementDate, depoFutSwapInstruments, termStructureDayCounter, new List<Handle<Quote>>(), new List<Date>(), tolerance);


            Console.WriteLine(dblrule1);
            Console.WriteLine("DepoFutSwap Term Structure Curve");
            Console.WriteLine(rule1);
            Console.WriteLine(headers1[0] + separator1
                      + headers1[1] + separator1
                      + headers1[2] + separator1
                      + headers1[3] + separator1);
            Console.WriteLine(rule1);

            foreach (Date d in depoFutSwapTermStructure.dates())
            {
                double yrs = termStructureDayCounter.yearFraction(todaysDate, d);
                if (yrs >= 15)
                    yrs = 15.0;
                InterestRate zero_rate = depoFutSwapTermStructure.zeroRate(yrs, Compounding.Compounded, Frequency.Annual);
                double eq_rate = zero_rate.equivalentRate(termStructureDayCounter, Compounding.Compounded, Frequency.Annual, todaysDate, d).rate();
                double discount = depoFutSwapTermStructure.discount(d);
                Console.Write("{0," + headers1[0].Length + ":00.00000}" + separator1, yrs);
                Console.Write("{0," + headers1[1].Length + ":0.000000}" + separator1, eq_rate);
                Console.Write("{0," + headers1[2].Length + ":0.000000}" + separator1, zero_rate.rate());
                Console.WriteLine("{0," + headers1[3].Length + ":0.000000}" + separator1, discount);
            }




            // A depo-FRA-swap curve
            List<RateHelper> depoFRASwapInstruments = new List<RateHelper>();
            depoFRASwapInstruments.Add(d1w);
            depoFRASwapInstruments.Add(d1m);
            depoFRASwapInstruments.Add(d3m);
            depoFRASwapInstruments.Add(fra3x6);
            depoFRASwapInstruments.Add(fra6x9);
            depoFRASwapInstruments.Add(fra6x12);
            depoFRASwapInstruments.Add(s2y);
            depoFRASwapInstruments.Add(s3y);
            depoFRASwapInstruments.Add(s5y);
            depoFRASwapInstruments.Add(s10y);
            depoFRASwapInstruments.Add(s15y);
            PiecewiseYieldCurve<Discount, LogLinear> depoFRASwapTermStructure = new PiecewiseYieldCurve<Discount, LogLinear>(
                    settlementDate, depoFRASwapInstruments, termStructureDayCounter, new List<Handle<Quote>>(), new List<Date>(), tolerance);



            Console.WriteLine(dblrule1);
            Console.WriteLine("DepoFRASwap Term Structure Curve");
            Console.WriteLine(rule1);
            Console.WriteLine(headers1[0] + separator1
                      + headers1[1] + separator1
                      + headers1[2] + separator1
                      + headers1[3] + separator1);
            Console.WriteLine(rule1);

            foreach (Date d in depoFRASwapTermStructure.dates())
            {
                double yrs = termStructureDayCounter.yearFraction(todaysDate, d);
                if (yrs >= 15)
                    yrs = 15.0;
                InterestRate zero_rate = depoFRASwapTermStructure.zeroRate(yrs, Compounding.Compounded, Frequency.Annual);
                double eq_rate = zero_rate.equivalentRate(termStructureDayCounter, Compounding.Compounded, Frequency.Annual, todaysDate, d).rate();
                double discount = depoFRASwapTermStructure.discount(d);
                Console.Write("{0," + headers1[0].Length + ":00.00000}" + separator1, yrs);
                Console.Write("{0," + headers1[1].Length + ":0.000000}" + separator1, eq_rate);
                Console.Write("{0," + headers1[2].Length + ":0.000000}" + separator1, zero_rate.rate());
                Console.WriteLine("{0," + headers1[3].Length + ":0.000000}" + separator1, discount);
            }











            // Term structures that will be used for pricing:
            // the one used for discounting cash flows
            RelinkableHandle<YieldTermStructure> discountingTermStructure = new RelinkableHandle<YieldTermStructure>();
            // the one used for forward rate forecasting
            RelinkableHandle<YieldTermStructure> forecastingTermStructure = new RelinkableHandle<YieldTermStructure>();


            /*********************
            * SWAPS TO BE PRICED *
            **********************/

            // constant nominal 1,000,000 Euro
            double nominal = 1000000.0;
            // fixed leg
            Frequency fixedLegFrequency = Frequency.Annual;
            BusinessDayConvention fixedLegConvention = BusinessDayConvention.Unadjusted;
            BusinessDayConvention floatingLegConvention = BusinessDayConvention.ModifiedFollowing;
            DayCounter fixedLegDayCounter = new Thirty360(Thirty360.Thirty360Convention.European);
            double fixedRate = 0.04;
            DayCounter floatingLegDayCounter = new Actual360();

            // floating leg
            Frequency floatingLegFrequency = Frequency.Semiannual;
            IborIndex euriborIndex = new Euribor6M(forecastingTermStructure);
            double spread = 0.0;

            int lenghtInYears = 5;
            VanillaSwap.Type swapType = VanillaSwap.Type.Payer;

            Date maturity = settlementDate + new Period(lenghtInYears, TimeUnit.Years);
            Schedule fixedSchedule = new Schedule(settlementDate, maturity, new Period(fixedLegFrequency),
                                     calendar, fixedLegConvention, fixedLegConvention, DateGeneration.Rule.Forward, false);
            Schedule floatSchedule = new Schedule(settlementDate, maturity, new Period(floatingLegFrequency),
                                     calendar, floatingLegConvention, floatingLegConvention, DateGeneration.Rule.Forward, false);
            VanillaSwap spot5YearSwap = new VanillaSwap(swapType, nominal, fixedSchedule, fixedRate, fixedLegDayCounter,
                                        floatSchedule, euriborIndex, spread, floatingLegDayCounter);

            Date fwdStart = calendar.advance(settlementDate, 1, TimeUnit.Years);
            Date fwdMaturity = fwdStart + new Period(lenghtInYears, TimeUnit.Years);
            Schedule fwdFixedSchedule = new Schedule(fwdStart, fwdMaturity, new Period(fixedLegFrequency),
                                        calendar, fixedLegConvention, fixedLegConvention, DateGeneration.Rule.Forward, false);
            Schedule fwdFloatSchedule = new Schedule(fwdStart, fwdMaturity, new Period(floatingLegFrequency),
                                        calendar, floatingLegConvention, floatingLegConvention, DateGeneration.Rule.Forward, false);
            VanillaSwap oneYearForward5YearSwap = new VanillaSwap(swapType, nominal, fwdFixedSchedule, fixedRate, fixedLegDayCounter,
                                        fwdFloatSchedule, euriborIndex, spread, floatingLegDayCounter);


            /***************
            * SWAP PRICING *
            ****************/

            // utilities for reporting
            List<string> headers = new List<string>();
            headers.Add("term structure");
            headers.Add("net present value");
            headers.Add("fair spread");
            headers.Add("fair fixed rate");
            string separator = " | ";
            int width = headers[0].Length + separator.Length
                       + headers[1].Length + separator.Length
                       + headers[2].Length + separator.Length
                       + headers[3].Length + separator.Length - 1;
            string rule = string.Format("").PadLeft(width, '-'), dblrule = string.Format("").PadLeft(width, '=');
            string tab = string.Format("").PadLeft(8, ' ');

            // calculations

            Console.WriteLine(dblrule);
            Console.WriteLine("5-year market swap-rate = {0:0.00%}", s5yRate.value());
            Console.WriteLine(dblrule);

            Console.WriteLine(tab + "5-years swap paying {0:0.00%}", fixedRate);
            Console.WriteLine(headers[0] + separator
                      + headers[1] + separator
                      + headers[2] + separator
                      + headers[3] + separator);
            Console.WriteLine(rule);

            double NPV;
            double fairRate;
            double fairSpread;

            IPricingEngine swapEngine = new DiscountingSwapEngine(discountingTermStructure);

            spot5YearSwap.setPricingEngine(swapEngine);
            oneYearForward5YearSwap.setPricingEngine(swapEngine);

            // Of course, you're not forced to really use different curves
            forecastingTermStructure.linkTo(depoSwapTermStructure);
            discountingTermStructure.linkTo(depoSwapTermStructure);

            NPV = spot5YearSwap.NPV();
            fairSpread = spot5YearSwap.fairSpread();
            fairRate = spot5YearSwap.fairRate();

            Console.Write("{0," + headers[0].Length + ":0.00}" + separator, "depo-swap");
            Console.Write("{0," + headers[1].Length + ":0.00}" + separator, NPV);
            Console.Write("{0," + headers[2].Length + ":0.00%}" + separator, fairSpread);
            Console.WriteLine("{0," + headers[3].Length + ":0.00%}" + separator, fairRate);

            // let's check that the 5 years swap has been correctly re-priced
            if (!(Math.Abs(fairRate - s5yQuote) < 1e-8))
                throw new ApplicationException("5-years swap mispriced by " + Math.Abs(fairRate - s5yQuote));


            forecastingTermStructure.linkTo(depoFutSwapTermStructure);
            discountingTermStructure.linkTo(depoFutSwapTermStructure);

            NPV = spot5YearSwap.NPV();
            fairSpread = spot5YearSwap.fairSpread();
            fairRate = spot5YearSwap.fairRate();

            Console.Write("{0," + headers[0].Length + ":0.00}" + separator, "depo-fut-swap");
            Console.Write("{0," + headers[1].Length + ":0.00}" + separator, NPV);
            Console.Write("{0," + headers[2].Length + ":0.00%}" + separator, fairSpread);
            Console.WriteLine("{0," + headers[3].Length + ":0.00%}" + separator, fairRate);

            if (!(Math.Abs(fairRate - s5yQuote) < 1e-8))
                throw new ApplicationException("5-years swap mispriced by " + Math.Abs(fairRate - s5yQuote));

            forecastingTermStructure.linkTo(depoFRASwapTermStructure);
            discountingTermStructure.linkTo(depoFRASwapTermStructure);

            NPV = spot5YearSwap.NPV();
            fairSpread = spot5YearSwap.fairSpread();
            fairRate = spot5YearSwap.fairRate();

            Console.Write("{0," + headers[0].Length + ":0.00}" + separator, "depo-FRA-swap");
            Console.Write("{0," + headers[1].Length + ":0.00}" + separator, NPV);
            Console.Write("{0," + headers[2].Length + ":0.00%}" + separator, fairSpread);
            Console.WriteLine("{0," + headers[3].Length + ":0.00%}" + separator, fairRate);

            if (!(Math.Abs(fairRate - s5yQuote) < 1e-8))
                throw new ApplicationException("5-years swap mispriced by " + Math.Abs(fairRate - s5yQuote));

            Console.WriteLine(rule);

            // now let's price the 1Y forward 5Y swap
            Console.WriteLine(tab + "5-years, 1-year forward swap paying {0:0.00%}", fixedRate);
            Console.WriteLine(headers[0] + separator
                      + headers[1] + separator
                      + headers[2] + separator
                      + headers[3] + separator);
            Console.WriteLine(rule);

            forecastingTermStructure.linkTo(depoSwapTermStructure);
            discountingTermStructure.linkTo(depoSwapTermStructure);

            NPV = oneYearForward5YearSwap.NPV();
            fairSpread = oneYearForward5YearSwap.fairSpread();
            fairRate = oneYearForward5YearSwap.fairRate();

            Console.Write("{0," + headers[0].Length + ":0.00}" + separator, "depo-swap");
            Console.Write("{0," + headers[1].Length + ":0.00}" + separator, NPV);
            Console.Write("{0," + headers[2].Length + ":0.00%}" + separator, fairSpread);
            Console.WriteLine("{0," + headers[3].Length + ":0.00%}" + separator, fairRate);

            forecastingTermStructure.linkTo(depoFutSwapTermStructure);
            discountingTermStructure.linkTo(depoFutSwapTermStructure);

            NPV = oneYearForward5YearSwap.NPV();
            fairSpread = oneYearForward5YearSwap.fairSpread();
            fairRate = oneYearForward5YearSwap.fairRate();

            Console.Write("{0," + headers[0].Length + ":0.00}" + separator, "depo-fut-swap");
            Console.Write("{0," + headers[1].Length + ":0.00}" + separator, NPV);
            Console.Write("{0," + headers[2].Length + ":0.00%}" + separator, fairSpread);
            Console.WriteLine("{0," + headers[3].Length + ":0.00%}" + separator, fairRate);

            forecastingTermStructure.linkTo(depoFRASwapTermStructure);
            discountingTermStructure.linkTo(depoFRASwapTermStructure);

            NPV = oneYearForward5YearSwap.NPV();
            fairSpread = oneYearForward5YearSwap.fairSpread();
            fairRate = oneYearForward5YearSwap.fairRate();

            Console.Write("{0," + headers[0].Length + ":0.00}" + separator, "depo-FRA-swap");
            Console.Write("{0," + headers[1].Length + ":0.00}" + separator, NPV);
            Console.Write("{0," + headers[2].Length + ":0.00%}" + separator, fairSpread);
            Console.WriteLine("{0," + headers[3].Length + ":0.00%}" + separator, fairRate);

            // now let's say that the 5-years swap rate goes up to 4.60%.
            // A smarter market element--say, connected to a data source-- would
            // notice the change itself. Since we're using SimpleQuotes,
            // we'll have to change the value manually--which forces us to
            // downcast the handle and use the SimpleQuote
            // interface. In any case, the point here is that a change in the
            // value contained in the Quote triggers a new bootstrapping
            // of the curve and a repricing of the swap.

            SimpleQuote fiveYearsRate = s5yRate as SimpleQuote;
            fiveYearsRate.setValue(0.0460);

            Console.WriteLine(dblrule);
            Console.WriteLine("5-year market swap-rate = {0:0.00%}", s5yRate.value());
            Console.WriteLine(dblrule);

            Console.WriteLine(tab + "5-years swap paying {0:0.00%}", fixedRate);
            Console.WriteLine(headers[0] + separator
                      + headers[1] + separator
                      + headers[2] + separator
                      + headers[3] + separator);
            Console.WriteLine(rule);

            // now get the updated results
            forecastingTermStructure.linkTo(depoSwapTermStructure);
            discountingTermStructure.linkTo(depoSwapTermStructure);

            NPV = spot5YearSwap.NPV();
            fairSpread = spot5YearSwap.fairSpread();
            fairRate = spot5YearSwap.fairRate();

            Console.Write("{0," + headers[0].Length + ":0.00}" + separator, "depo-swap");
            Console.Write("{0," + headers[1].Length + ":0.00}" + separator, NPV);
            Console.Write("{0," + headers[2].Length + ":0.00%}" + separator, fairSpread);
            Console.WriteLine("{0," + headers[3].Length + ":0.00%}" + separator, fairRate);

            if (!(Math.Abs(fairRate - s5yRate.value()) < 1e-8))
                throw new ApplicationException("5-years swap mispriced by " + Math.Abs(fairRate - s5yRate.value()));

            forecastingTermStructure.linkTo(depoFutSwapTermStructure);
            discountingTermStructure.linkTo(depoFutSwapTermStructure);

            NPV = spot5YearSwap.NPV();
            fairSpread = spot5YearSwap.fairSpread();
            fairRate = spot5YearSwap.fairRate();

            Console.Write("{0," + headers[0].Length + ":0.00}" + separator, "depo-fut-swap");
            Console.Write("{0," + headers[1].Length + ":0.00}" + separator, NPV);
            Console.Write("{0," + headers[2].Length + ":0.00%}" + separator, fairSpread);
            Console.WriteLine("{0," + headers[3].Length + ":0.00%}" + separator, fairRate);

            if (!(Math.Abs(fairRate - s5yRate.value()) < 1e-8))
                throw new ApplicationException("5-years swap mispriced by " + Math.Abs(fairRate - s5yRate.value()));

            forecastingTermStructure.linkTo(depoFRASwapTermStructure);
            discountingTermStructure.linkTo(depoFRASwapTermStructure);

            NPV = spot5YearSwap.NPV();
            fairSpread = spot5YearSwap.fairSpread();
            fairRate = spot5YearSwap.fairRate();

            Console.Write("{0," + headers[0].Length + ":0.00}" + separator, "depo-FRA-swap");
            Console.Write("{0," + headers[1].Length + ":0.00}" + separator, NPV);
            Console.Write("{0," + headers[2].Length + ":0.00%}" + separator, fairSpread);
            Console.WriteLine("{0," + headers[3].Length + ":0.00%}" + separator, fairRate);

            if (!(Math.Abs(fairRate - s5yRate.value()) < 1e-8))
                throw new ApplicationException("5-years swap mispriced by " + Math.Abs(fairRate - s5yRate.value()));

            Console.WriteLine(rule);

            // the 1Y forward 5Y swap changes as well

            Console.WriteLine(tab + "5-years, 1-year forward swap paying {0:0.00%}", fixedRate);
            Console.WriteLine(headers[0] + separator
                      + headers[1] + separator
                      + headers[2] + separator
                      + headers[3] + separator);
            Console.WriteLine(rule);

            forecastingTermStructure.linkTo(depoSwapTermStructure);
            discountingTermStructure.linkTo(depoSwapTermStructure);

            NPV = oneYearForward5YearSwap.NPV();
            fairSpread = oneYearForward5YearSwap.fairSpread();
            fairRate = oneYearForward5YearSwap.fairRate();

            Console.Write("{0," + headers[0].Length + ":0.00}" + separator, "depo-swap");
            Console.Write("{0," + headers[1].Length + ":0.00}" + separator, NPV);
            Console.Write("{0," + headers[2].Length + ":0.00%}" + separator, fairSpread);
            Console.WriteLine("{0," + headers[3].Length + ":0.00%}" + separator, fairRate);

            forecastingTermStructure.linkTo(depoFutSwapTermStructure);
            discountingTermStructure.linkTo(depoFutSwapTermStructure);

            NPV = oneYearForward5YearSwap.NPV();
            fairSpread = oneYearForward5YearSwap.fairSpread();
            fairRate = oneYearForward5YearSwap.fairRate();

            Console.Write("{0," + headers[0].Length + ":0.00}" + separator, "depo-fut-swap");
            Console.Write("{0," + headers[1].Length + ":0.00}" + separator, NPV);
            Console.Write("{0," + headers[2].Length + ":0.00%}" + separator, fairSpread);
            Console.WriteLine("{0," + headers[3].Length + ":0.00%}" + separator, fairRate);

            forecastingTermStructure.linkTo(depoFRASwapTermStructure);
            discountingTermStructure.linkTo(depoFRASwapTermStructure);

            NPV = oneYearForward5YearSwap.NPV();
            fairSpread = oneYearForward5YearSwap.fairSpread();
            fairRate = oneYearForward5YearSwap.fairRate();

            Console.Write("{0," + headers[0].Length + ":0.00}" + separator, "depo-FRA-swap");
            Console.Write("{0," + headers[1].Length + ":0.00}" + separator, NPV);
            Console.Write("{0," + headers[2].Length + ":0.00%}" + separator, fairSpread);
            Console.WriteLine("{0," + headers[3].Length + ":0.00%}" + separator, fairRate);


            Console.WriteLine(" \nRun completed in {0}", DateTime.Now - timer);

            Console.Write("Press any key to continue ...");
            Console.ReadKey();
        }























        public static void ZeroCoupon3()
        {
            List<double> depo_rates = new List<double>() { 4.4, 4.5, 4.6, 4.7, 4.9, 5.0, 5.1 };
            List<Period> depo_maturities = new List<Period>() 
            { 
                new Period(1, TimeUnit.Days), 
                new Period(1, TimeUnit.Months),
                new Period(2, TimeUnit.Months),
                new Period(3, TimeUnit.Months),
                new Period(6, TimeUnit.Months),
                new Period(9, TimeUnit.Months),
                new Period(12, TimeUnit.Months),
            };

            List<double> bond_rates = new List<double>() { 5.0, 6.0, 5.5, 5.0 };
            List<double> bond_prices = new List<double>() { 103.7, 102.0, 99.5, 97.6 };
            List<Period> bond_maturities = new List<Period>()
            {
                new Period(14, TimeUnit.Months),
                new Period(21, TimeUnit.Months),
                new Period(2, TimeUnit.Years),
                new Period(3, TimeUnit.Years),
            };

            Date calc_date = new Date(15, 1, 2015);
            Settings.setEvaluationDate(calc_date);
            Calendar calendar = new UnitedStates();

            List<RateHelper> rateHelpers = new List<RateHelper>();
            for (int i = 0; i < depo_rates.Count; i++)
            {
                rateHelpers.Add(new DepositRateHelper(depo_rates[i] / 100.0, depo_maturities[i], 0, calendar, BusinessDayConvention.Unadjusted, true, new Thirty360()));
            }

            for (int i = 0; i < bond_rates.Count; i++)
            {
                Date termination_date = calc_date + bond_maturities[i];
                Schedule schedule = new Schedule(calc_date, termination_date, new Period(Frequency.Annual), calendar, BusinessDayConvention.Unadjusted,
                    BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, true);
                rateHelpers.Add(new FixedRateBondHelper(new Handle<Quote>(new SimpleQuote(bond_prices[i])), 0, bond_prices[i], schedule, new List<double>() { bond_rates[i] / 100.0 },
                    new Thirty360(), BusinessDayConvention.Unadjusted, 100.0, calc_date));
            }

            PiecewiseYieldCurve<Discount, Linear> discountingTermStructure = new PiecewiseYieldCurve<Discount, Linear>(calc_date, rateHelpers, new Thirty360());
            DayCounter dc = new Thirty360();

            try
            {
                foreach (Date d in discountingTermStructure.dates())
                {
                    double yrs = dc.yearFraction(calc_date, d);
                    InterestRate zero_rate = discountingTermStructure.zeroRate(yrs, Compounding.Compounded, Frequency.Annual);
                    double eq_rate = zero_rate.equivalentRate(dc, Compounding.Compounded, Frequency.Annual, calc_date, d).rate();
                    double discount = discountingTermStructure.discount(d);
                    System.Windows.MessageBox.Show(string.Format("mat={0}, rate={1}, zero_rate={2}, discount={3}", yrs, eq_rate, zero_rate.rate(), discount));
                }
            }
            catch { }

        }


        public static void ZeroCoupon2()
        {
            List<double> depo_rates = new List<double>() { 4.4, 4.5, 4.6, 4.7, 4.9, 5.0, 5.1 };
            List<Period> depo_maturities = new List<Period>() 
            { 
                new Period(1, TimeUnit.Days), 
                new Period(1, TimeUnit.Months),
                new Period(2, TimeUnit.Months),
                new Period(3, TimeUnit.Months),
                new Period(6, TimeUnit.Months),
                new Period(9, TimeUnit.Months),
                new Period(12, TimeUnit.Months),
            };

            List<double> bond_rates = new List<double>() { 5.0, 6.0, 5.5, 5.0 };
            List<double> bond_prices = new List<double>() { 103.7, 102.0, 99.5, 97.6 };
            List<Period> bond_maturities = new List<Period>()
            {
                new Period(14, TimeUnit.Months),
                new Period(21, TimeUnit.Months),
                new Period(2, TimeUnit.Years),
                new Period(3, TimeUnit.Years),
            };

            Date calc_date = new Date(15, 1, 2015);
            Settings.setEvaluationDate(calc_date);
            Calendar calendar = new UnitedStates();

            List<RateHelper> rateHelpers = new List<RateHelper>();
            for (int i = 0; i < depo_rates.Count; i++)
            {
                rateHelpers.Add(new DepositRateHelper(depo_rates[i] / 100.0, depo_maturities[i], 0, calendar, BusinessDayConvention.Unadjusted, true, new Thirty360()));
            }

            for (int i = 0; i < bond_rates.Count; i++)
            {
                Date termination_date = calc_date + bond_maturities[i];
                Schedule schedule = new Schedule(calc_date, termination_date, new Period(Frequency.Annual), calendar, BusinessDayConvention.Unadjusted,
                    BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, true);
                rateHelpers.Add(new FixedRateBondHelper(new Handle<Quote>(new SimpleQuote(bond_prices[i])), 0, bond_prices[i], schedule, new List<double>() { bond_rates[i] / 100.0 },
                    new Thirty360(), BusinessDayConvention.Unadjusted, 100.0, calc_date));
            }

            PiecewiseYieldCurve<Discount, Linear> discountingTermStructure = new PiecewiseYieldCurve<Discount, Linear>(calc_date, rateHelpers, new Thirty360());
            DayCounter dc = new Thirty360();

            //try
            {
                foreach (Date d in discountingTermStructure.dates())
                {
                    double yrs = dc.yearFraction(calc_date, d);
                    InterestRate zero_rate = discountingTermStructure.zeroRate(yrs, Compounding.Compounded, Frequency.Annual);
                    double eq_rate = zero_rate.equivalentRate(dc, Compounding.Compounded, Frequency.Annual, calc_date, d).rate();
                    double discount = discountingTermStructure.discount(d);
                    System.Windows.MessageBox.Show(string.Format("mat={0}, rate={1}, zero_rate={2}, discount={3}", yrs, eq_rate, zero_rate.rate(), discount));
                }
            }
            //catch { }

        }





        public static void ZeroCoupon1()
        {
            //fixed-income book pp98-99:

            List<double> bond_rates = new List<double>() { 5.0, 5.5, 5.0, 6.0 };
            List<double> bond_prices = new List<double>() { 101.0, 101.5, 99.0, 100.0 };
            List<Period> bond_maturities = new List<Period>() 
            { 
                new Period(1, TimeUnit.Years), 
                new Period(2, TimeUnit.Years), 
                new Period(3, TimeUnit.Years), 
                new Period(4, TimeUnit.Years), 
            };

            Date calc_date = new Date(15, 1, 2015);
            Settings.setEvaluationDate(calc_date);
            Calendar calendar = new UnitedStates();

            List<RateHelper> rateHelpers = new List<RateHelper>();
            for (int i = 0; i < bond_rates.Count; i++)
            {
                Date termination_date = calc_date + bond_maturities[i];
                Schedule schedule = new Schedule(calc_date, termination_date, new Period(Frequency.Annual), calendar, BusinessDayConvention.Unadjusted,
                    BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, true);
                rateHelpers.Add(new FixedRateBondHelper(new Handle<Quote>(new SimpleQuote(bond_prices[i])), 0, bond_prices[i], schedule, new List<double>() { bond_rates[i] / 100.0 },
                    new Thirty360(), BusinessDayConvention.Unadjusted, 100.0, calc_date));
            }

            PiecewiseYieldCurve<Discount, LogLinear> discountingTermStructure = new PiecewiseYieldCurve<Discount, LogLinear>(calc_date, rateHelpers, new Thirty360());
            DayCounter dc = new Thirty360();

            foreach (Date d in discountingTermStructure.dates())
            {
                double yrs = dc.yearFraction(calc_date, d);
                InterestRate zero_rate = discountingTermStructure.zeroRate(yrs, Compounding.Compounded, Frequency.Annual);
                double eq_rate = zero_rate.equivalentRate(dc, Compounding.Compounded, Frequency.Annual, calc_date, d).rate();
                double discount = discountingTermStructure.discount(d);
                System.Windows.MessageBox.Show(string.Format("mat={0}, rate={1}, zero_rate={2}, discount={3}", yrs, eq_rate, zero_rate.rate(), discount));
            }
        }

       




        public static InterestRate[] InterestRate(double annualRate)
        {
            //Rate compounded annually
            InterestRate effectiveRate = new InterestRate(annualRate, new ActualActual(), Compounding.Compounded, Frequency.Annual);

            //Equivalent semi-annual one-year rate:
            InterestRate semiRate = effectiveRate.equivalentRate(Compounding.Compounded, Frequency.Semiannual, 1);

            //Equivalent one-year rate for continuously compounded:
            InterestRate continueRate = effectiveRate.equivalentRate(Compounding.Continuous, Frequency.Annual, 1);

            return new InterestRate[] { effectiveRate, semiRate, continueRate };
        }

        public static object[] BondPrice3()
        {
            //Flat interest rate curve
            Calendar calendar = new UnitedStates(UnitedStates.Market.GovernmentBond);
            int settlementDays = 3;
            Date today = Date.Today;
            Date issueDate = today;
            Date terminationDate = issueDate + new Period(10, TimeUnit.Years);
            double rate = 0.06;
            double faceValue = 1000.0;
            List<double> coupon = new List<double>() { 0.05};
            
            Schedule schedule = new Schedule(issueDate, terminationDate, new Period(Frequency.Annual), calendar, 
                BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);
            FixedRateBond fixedRateBond = new FixedRateBond(settlementDays, faceValue, schedule, coupon, new ActualActual(ActualActual.Convention.Bond));
            FlatForward flatCurve = new FlatForward(issueDate, rate, new ActualActual(ActualActual.Convention.Bond), Compounding.Compounded, Frequency.Annual);
            //RelinkableHandle<YieldTermStructure> discountingTermStructure = new RelinkableHandle<YieldTermStructure>();
            //discountingTermStructure.linkTo(flatCurve);
            Handle<YieldTermStructure> discountingTermStructure = new Handle<YieldTermStructure>(flatCurve);
            DiscountingBondEngine bondEngine = new DiscountingBondEngine(discountingTermStructure);
            fixedRateBond.setPricingEngine(bondEngine);

            object npv = null;
            object cprice = null;
            object dprice = null;
            object accrued = null;

            try
            {
                fixedRateBond.setPricingEngine(bondEngine);
                npv = fixedRateBond.NPV();
                cprice = fixedRateBond.cleanPrice();
                dprice = fixedRateBond.dirtyPrice();
                accrued = fixedRateBond.accruedAmount();
            }
            catch { }
            return new object[] { npv, cprice, dprice, accrued };
        }

        public static object[] BondPrice2()
        {
            Frequency frequency = Frequency.Annual;
            Date today = new Date(15, 1, 2015);
            Settings.setEvaluationDate(today);
            Date issueDate = new Date(15, 1, 2015);
            Date maturity = new Date(15, 1, 2016);
            Period tenor = new Period(Frequency.Semiannual);

            List<double> spotRates = new List<double>() { 1.0e-5, 0.005, 0.007 };
            List<Date> spotDates = new List<Date>() { new Date(15, 1, 2015), new Date(15, 7, 2015), new Date(15, 1, 2016) };
            DayCounter dc = new Thirty360();
            Calendar calendar = new UnitedStates();
            //var spotCurve = new InterpolatedZeroCurve<Cubic>(spotDates, spotRates, dc, calendar, new Cubic(), Compounding.Compounded, Frequency.Annual);
            InterpolatedZeroCurve<Cubic> spotCurve = new InterpolatedZeroCurve<Cubic>(spotDates, spotRates, dc, calendar, new Cubic(), Compounding.Compounded, Frequency.Annual);
            RelinkableHandle<YieldTermStructure> discountingTermStructure = new RelinkableHandle<YieldTermStructure>();
            discountingTermStructure.linkTo(spotCurve);

            Schedule schedule = new Schedule(issueDate, maturity, tenor, calendar, BusinessDayConvention.Unadjusted, 
                BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);
            FixedRateBond fixedRateBond = new FixedRateBond(0, 100, schedule, new List<double>() { 0.06 }, dc);
            DiscountingBondEngine bondEngine = new DiscountingBondEngine(discountingTermStructure);
            fixedRateBond.setPricingEngine(bondEngine);

            object npv = null;
            object cprice = null;
            object dprice = null;
            object accrued = null;

            foreach (Date d in spotCurve.dates())
            {
                double yrs = dc.yearFraction(today, d);
                InterestRate zero_rate = spotCurve.zeroRate(yrs, Compounding.Compounded, Frequency.Semiannual);
                double eq_rate = zero_rate.equivalentRate(dc, Compounding.Compounded, Frequency.Semiannual, today, d).rate();
                double discount = spotCurve.discount(d);
                //System.Windows.MessageBox.Show(string.Format("mat={0}, rate={1}, zero_rate={2}, discount={3}", yrs, eq_rate, zero_rate, discount));
            }

            try
            {
                fixedRateBond.setPricingEngine(bondEngine);
                npv = fixedRateBond.NPV();
                cprice = fixedRateBond.cleanPrice();
                dprice = fixedRateBond.dirtyPrice();
                accrued = fixedRateBond.accruedAmount();
            }
            catch { }
            return new object[] { npv, cprice, dprice, accrued };
        }

        public static object[] BondPrice2a()
        {
            Frequency frequency = Frequency.Annual;
            Date today = new Date(15, 1, 2015);
            Settings.setEvaluationDate(today);
            Date issueDate = new Date(15, 1, 2015);
            Date maturity = new Date(15, 1, 2017);
            Period tenor = new Period(Frequency.Semiannual);

            List<double> spotRates = new List<double>() { 1.0e-7, 0.004, 0.006, 0.0065, 0.007 };
            List<Date> spotDates = new List<Date>() { new Date(15, 1, 2015), new Date(15, 7, 2015), new Date(15, 1, 2016), new Date(15, 7, 2016), new Date(15, 1, 2017) };
            DayCounter dc = new ActualActual(ActualActual.Convention.Bond);
            //DayCounter dc = new Actual365Fixed();
            Calendar calendar = new UnitedStates();
            //var spotCurve = new InterpolatedZeroCurve<Cubic>(spotDates, spotRates, dc, calendar, new Cubic(), Compounding.Compounded, Frequency.Annual);
            InterpolatedZeroCurve<Cubic> spotCurve = new InterpolatedZeroCurve<Cubic>(spotDates, spotRates, dc, calendar, new Cubic(), Compounding.Compounded, Frequency.Annual);
            RelinkableHandle<YieldTermStructure> discountingTermStructure = new RelinkableHandle<YieldTermStructure>();
            discountingTermStructure.linkTo(spotCurve);

            Schedule schedule = new Schedule(issueDate, maturity, tenor, calendar, BusinessDayConvention.Unadjusted,
                BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);
            FixedRateBond fixedRateBond = new FixedRateBond(0, 100, schedule, new List<double>() { 0.05 }, dc);
            DiscountingBondEngine bondEngine = new DiscountingBondEngine(discountingTermStructure);
            fixedRateBond.setPricingEngine(bondEngine);

            object npv = null;
            object cprice = null;
            object dprice = null;
            object accrued = null;

           /* foreach (var d in spotCurve.dates())
            {
                var yrs = dc.yearFraction(today, d);
                var zero_rate = spotCurve.zeroRate(yrs, Compounding.Compounded, Frequency.Semiannual);
                var eq_rate = zero_rate.equivalentRate(dc, Compounding.Compounded, Frequency.Semiannual, today, d).rate();
                var discount = spotCurve.discount(d);
                //System.Windows.MessageBox.Show(string.Format("mat={0}, rate={1}, zero_rate={2}, discount={3}", yrs, eq_rate, zero_rate, discount));
            }*/

            //try
            {
                fixedRateBond.setPricingEngine(bondEngine);
                npv = fixedRateBond.NPV();
                cprice = fixedRateBond.cleanPrice();
                dprice = fixedRateBond.dirtyPrice();
                accrued = fixedRateBond.accruedAmount();
            }
            //catch { }
            return new object[] { npv, cprice, dprice, accrued };
        }

        public static object[] BondPrice1()
        {
          
            Date today = new Date(15, 1, 2015);
            Settings.setEvaluationDate(today);
            Date issueDate = new Date(15, 1, 2015);
            Date maturity = new Date(15, 1, 2016);
            Period tenor = new Period(Frequency.Semiannual);

            List<double> spotRates = new List<double>() { 1.0e-5, 0.005, 0.007 };
            List<Date> spotDates = new List<Date>() { new Date(15, 1, 2015), new Date(15, 7, 2015), new Date(15, 1, 2016) };

            DayCounter dc = new Thirty360();
            Calendar calendar = new UnitedStates();
            InterpolatedZeroCurve<Linear> spotCurve = new InterpolatedZeroCurve<Linear>(spotDates, spotRates, dc, calendar, new Linear(), Compounding.Compounded, Frequency.Annual);        
            RelinkableHandle<YieldTermStructure> discountingTermStructure = new RelinkableHandle<YieldTermStructure>();
            discountingTermStructure.linkTo(spotCurve);

            Schedule schedule = new Schedule(issueDate, maturity, tenor, calendar, BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);
            FixedRateBond fixedRateBond = new FixedRateBond(0, 100, schedule, new List<double>() { 0.06 }, dc);
            DiscountingBondEngine bondEngine = new DiscountingBondEngine(discountingTermStructure);
            fixedRateBond.setPricingEngine(bondEngine);
            
            object npv = null;
            object cprice = null;
            object dprice = null;
            object accrued = null;

            
            /*foreach(var d in spotCurve.dates())
            {
                var yrs = dc.yearFraction(today, d);
                var zero_rate = spotCurve.zeroRate(yrs, Compounding.Compounded, Frequency.Semiannual);
                var eq_rate = zero_rate.equivalentRate(dc, Compounding.Compounded, Frequency.Semiannual, today, d).rate();

                System.Windows.MessageBox.Show(string.Format("mat={0}, rate={1}, zero_rate={2}", yrs, eq_rate,zero_rate));
            }*/

            try
            {
                fixedRateBond.setPricingEngine(bondEngine);
                npv = fixedRateBond.NPV();
                cprice = fixedRateBond.cleanPrice();
                dprice = fixedRateBond.dirtyPrice();
                accrued = fixedRateBond.accruedAmount();
            }
            catch { }
            return new object[] { npv, cprice, dprice, accrued };
        }


        public static object[] BondPrice()
        {
            Date today = new Date(15, 1, 2015);
            Settings.setEvaluationDate(today);
            Date issueDate = new Date(15, 1, 2015);
            Date maturity = new Date(15, 1, 2016);
            Period tenor = new Period(Frequency.Semiannual);

            //List<double> spotRates = new List<double>() { 0.0, 0.005, 0.007 };
            //List<Date> spotDates = new List<Date>() { new Date(15, 1, 2015), new Date(15, 7, 2015), new Date(15, 1, 2016) };
         
            DayCounter dc = new Thirty360();
            Calendar calendar = new UnitedStates();

            DayCounter zcBondsDayCounter = new Thirty360();
            int fixingDays = 0;

            RateHelper zc3m = new DepositRateHelper(1.0e-5,
                                                         new Period(1, TimeUnit.Months), fixingDays,
                                                         calendar, BusinessDayConvention.Unadjusted,
                                                         false, zcBondsDayCounter);
            RateHelper zc6m = new DepositRateHelper(0.005,
                                                         new Period(6, TimeUnit.Months), fixingDays,
                                                         calendar, BusinessDayConvention.Unadjusted,
                                                         false, zcBondsDayCounter);
            RateHelper zc1y = new DepositRateHelper(0.007,
                                                         new Period(1, TimeUnit.Years), fixingDays,
                                                         calendar, BusinessDayConvention.Unadjusted,
                                                         false, zcBondsDayCounter);

           List<RateHelper> instruments = new List<RateHelper>();
            //instruments.Add(zc3m);
            instruments.Add(zc6m);
            instruments.Add(zc1y);
            PiecewiseYieldCurve<Discount, Linear> spotCurve = new PiecewiseYieldCurve<Discount, Linear>(today, instruments, dc, new List<Handle<Quote>>(), new List<Date>(), 1.0e-15);

            foreach(Date d in spotCurve.dates())
           {
                double yrs = dc.yearFraction(today, d);
                InterestRate zero_rate = spotCurve.zeroRate(yrs, Compounding.Compounded, Frequency.Semiannual);
                double eq_rate = zero_rate.equivalentRate(dc, Compounding.Compounded, Frequency.Semiannual, today, d).rate();

               //System.Windows.MessageBox.Show(string.Format("mat={0}, rate={1}, zero_rate={2}", yrs, eq_rate,zero_rate.rate()));
           }

            RelinkableHandle<YieldTermStructure> discountingTermStructure = new RelinkableHandle<YieldTermStructure>();
            discountingTermStructure.linkTo(spotCurve);

            Schedule schedule = new Schedule(issueDate, maturity, tenor, calendar, BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);
            FixedRateBond fixedRateBond = new FixedRateBond(0, 100, schedule, new List<double>() { 0.06 }, dc);
            DiscountingBondEngine bondEngine = new DiscountingBondEngine(discountingTermStructure);
            fixedRateBond.setPricingEngine(bondEngine);
           
            object npv = null;
            object cprice = null;
            object dprice = null;
            object accrued = null;

            try
            {
                fixedRateBond.setPricingEngine(bondEngine);
                npv = fixedRateBond.NPV();
                cprice = fixedRateBond.cleanPrice();
                dprice = fixedRateBond.dirtyPrice();
                accrued = fixedRateBond.accruedAmount();
            }
            catch { }

            return new object[] { npv, cprice, dprice, accrued };
        }

        #endregion Bond Pricing

    }
}
