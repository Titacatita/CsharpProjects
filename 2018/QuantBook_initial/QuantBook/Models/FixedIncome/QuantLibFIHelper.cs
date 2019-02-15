using QuantLib;
using System;
using QuantBook.Models;
using System.Data;
using System.Collections.Generic;
using Caliburn.Micro;
using QuantBook.Models.Isda;

namespace QuantBook.Models.FixedIncome
{
    public static class QuantLibFIHelper
    {
        public static object[] BondYtm(DateTime evalDate, DateTime issueDate, DateTime maturity, int settlementDays, double cleanPrice, double[] coupon, Frequency frequency)
        {
            Date evalDate1 = evalDate.To<Date>();
            Settings.instance().setEvaluationDate(evalDate1);
            Date issueDate1 = issueDate.To<Date>();
            Date maturity1 = maturity.To<Date>();
            Calendar calendar = new UnitedStates(UnitedStates.Market.GovernmentBond);
            Date settlementDate = evalDate1.Add(settlementDays);
            settlementDate = calendar.adjust(settlementDate);

            DoubleVector coupon1 = new DoubleVector();
            for (int i = 0; i < coupon.Length; i++)
                coupon1.Add(coupon[i]);

            DayCounter dc = new ActualActual(ActualActual.Convention.Bond);
            Schedule schedule = new Schedule(issueDate1, maturity1, new Period(frequency), calendar,
                BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);
            FixedRateBond fixedRateBond = new FixedRateBond(settlementDays, 100, schedule, coupon1, dc);

            object ytm = null;
            object accrued = null;
            try
            {
                ytm = fixedRateBond.yield(cleanPrice, dc, Compounding.Compounded, frequency);
                accrued = fixedRateBond.accruedAmount();
            }
            catch { }

            return new object[] { ytm, accrued };
        }


        public static object[] BondPrice(DateTime evalDate, DateTime issueDate, DateTime maturity, int settlementDays, double faceValue, double rate, double[] coupon, Frequency frequency)
        {
            Date evalDate1 = evalDate.To<Date>();
            Settings.instance().setEvaluationDate(evalDate1);
            Date issueDate1 = issueDate.To<Date>();
            Date maturity1 = maturity.To<Date>();
            Calendar calendar = new UnitedStates(UnitedStates.Market.GovernmentBond);
            Date settlementDate = evalDate1.Add(settlementDays);
            settlementDate = calendar.adjust(settlementDate);
            DoubleVector coupon1 = new DoubleVector();
            for (int i = 0; i < coupon.Length; i++)
                coupon1.Add(coupon[i]);

            DayCounter dc = new ActualActual(ActualActual.Convention.Bond);
            //DayCounter dc = new Actual365NoLeap();
            //DayCounter dc = new Thirty360();
            Schedule schedule = new Schedule(issueDate1, maturity1, new Period(frequency), calendar,
                BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);
            FixedRateBond fixedRateBond = new FixedRateBond(settlementDays, faceValue, schedule, coupon1, dc);
            FlatForward flatCurve = new FlatForward(settlementDate, rate, dc, Compounding.Compounded, Frequency.Annual);
            YieldTermStructureHandle discountingTermStructure = new YieldTermStructureHandle(flatCurve);
            DiscountingBondEngine bondEngine = new DiscountingBondEngine(discountingTermStructure);
            fixedRateBond.setPricingEngine(bondEngine);

            object npv = null;
            object cprice = null;
            object dprice = null;
            object accrued = null;
            //object cf = null;
            object ytm = null;

            try
            {

                //fixedRateBond.setPricingEngine(bondEngine);
                npv = fixedRateBond.NPV();
                cprice = fixedRateBond.cleanPrice();
                dprice = fixedRateBond.dirtyPrice();
                accrued = fixedRateBond.accruedAmount();
                ytm = fixedRateBond.yield(fixedRateBond.dayCounter(), Compounding.Compounded, frequency);
                /*cprice = fixedRateBond.cleanPrice(rate, dc, Compounding.Compounded, frequency);
                dprice = fixedRateBond.dirtyPrice(rate, dc, Compounding.Compounded, frequency);
                accrued = fixedRateBond.accruedAmount();
                ytm = fixedRateBond.yield((double)cprice, dc, Compounding.Compounded, frequency);*/  
            }
            catch { }
            return new object[] { npv, cprice, dprice, accrued, ytm };
        }

        public static DataTable BondPriceCurveRate()
        {
            Date evalDate = new Date(15, Month.January, 2015);
            Settings.instance().setEvaluationDate(evalDate);
            Date issueDate = new Date(15, Month.January, 2015);
            Date maturity = new Date(15, Month.January, 2017);
            Calendar calendar = new UnitedStates(UnitedStates.Market.GovernmentBond);
            int settlementDays = 1;
            Date settlementDate = evalDate.Add(settlementDays);
            settlementDate = calendar.adjust(settlementDate);

            Date[] rateDates = new Date[]
            {
                new Date(15, Month.January, 2015), 
                new Date(15, Month.July, 2015), 
                new Date(15, Month.January, 2016), 
                new Date(15, Month.July, 2016), 
                new Date(15, Month.January, 2017)
            };
            double[] rates = new double[] { 0, 0.004, 0.006, 0.0065, 0.007 };
            DateVector rateDates1 = new DateVector();
            DoubleVector rates1 = new DoubleVector();
            for (int i = 0; i < rates.Length; i++)
            {
                rateDates1.Add(rateDates[i]);
                rates1.Add(rates[i]);
            }
            DoubleVector coupon = new DoubleVector
            {
                0.05
            };
            DayCounter dc = new ActualActual(ActualActual.Convention.Bond);
            Schedule schedule = new Schedule(issueDate, maturity, new Period(Frequency.Semiannual), calendar,
                BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);
            FixedRateBond fixedRateBond = new FixedRateBond(settlementDays, 100, schedule, coupon, dc);
            ZeroCurve rateCurve = new ZeroCurve(rateDates1, rates1, dc, calendar, new Linear(), Compounding.Compounded, Frequency.Semiannual);
            YieldTermStructureHandle discountingTermStructure = new YieldTermStructureHandle(rateCurve);
            DiscountingBondEngine bondEngine = new DiscountingBondEngine(discountingTermStructure);
            fixedRateBond.setPricingEngine(bondEngine);

            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Value", typeof(string));
            dt.Rows.Add("Pricing Bond", "Using QuantLib");
            dt.Rows.Add("Issue Date", "1/15/2015");
            dt.Rows.Add("Evaluation Date", "1/15/2015");
            dt.Rows.Add("Times to Maturity in Years", 2);
            dt.Rows.Add("Face Value", 100);
            dt.Rows.Add("Discount Rate", "0.004, 0.006, 0.0065, 0.007");
            dt.Rows.Add("Coupon", "5%");

            try
            {
                fixedRateBond.setPricingEngine(bondEngine);
                dt.Rows.Add("Present Value", fixedRateBond.NPV());
                dt.Rows.Add("Clean Price", fixedRateBond.cleanPrice());
                dt.Rows.Add("Dirty Price", fixedRateBond.dirtyPrice());
                dt.Rows.Add("Accrued Value", fixedRateBond.accruedAmount());
                dt.Rows.Add("YTM", fixedRateBond.yield(fixedRateBond.dayCounter(), Compounding.Compounded, Frequency.Semiannual));
            }
            catch { }

            return dt;
        }


        public static DataTable BondPriceCurveRateZSpread()
        {
            Date evalDate = new Date(15, Month.January, 2015);
            Settings.instance().setEvaluationDate(evalDate);
            Date issueDate = new Date(15, Month.January, 2015);
            Date maturity = new Date(15, Month.January, 2017);
            Calendar calendar = new UnitedStates(UnitedStates.Market.GovernmentBond);
            int settlementDays = 1;
            Date settlementDate = evalDate.Add(settlementDays);
            settlementDate = calendar.adjust(settlementDate);

            Date[] rateDates = new Date[]
            {
                new Date(15, Month.January, 2015), 
                new Date(15, Month.July, 2015), 
                new Date(15, Month.January, 2016), 
                new Date(15, Month.July, 2016), 
                new Date(15, Month.January, 2017)
            };
            double[] rates = new double[] { 0, 0.004, 0.006, 0.0065, 0.007 };
            DateVector rateDates1 = new DateVector();
            DoubleVector rates1 = new DoubleVector();
            for (int i = 0; i < rates.Length; i++)
            {
                rateDates1.Add(rateDates[i]);
                rates1.Add(rates[i]);
            }
            DoubleVector coupon = new DoubleVector
            {
                0.05
            };
            DayCounter dc = new ActualActual(ActualActual.Convention.Bond);
            Schedule schedule = new Schedule(issueDate, maturity, new Period(Frequency.Semiannual), calendar,
                BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, false);
            FixedRateBond fixedRateBond = new FixedRateBond(settlementDays, 100, schedule, coupon, dc);
            ZeroCurve rateCurve = new ZeroCurve(rateDates1, rates1, dc, calendar, new Linear(), Compounding.Compounded, Frequency.Semiannual);
            YieldTermStructureHandle discountingTermStructure = new YieldTermStructureHandle(rateCurve);
            DiscountingBondEngine bondEngine = new DiscountingBondEngine(discountingTermStructure);
            fixedRateBond.setPricingEngine(bondEngine);

            DataTable dt = new DataTable();
            dt.Columns.Add("ZSpread", typeof(double));
            dt.Columns.Add("Price", typeof(double));

            fixedRateBond.setPricingEngine(bondEngine);

            double[] zSpreads = new double[21];
            for (int i = 0; i < zSpreads.Length; i++)
                zSpreads[i] = 250.0 * i;

            Leg leg = fixedRateBond.cashflows();
            for (int i = 0; i < zSpreads.Length; i++)
            {
                object zNPV = CashFlows.npv(leg, rateCurve, zSpreads[i] / 10000.0, dc, Compounding.Compounded, Frequency.Semiannual, true, settlementDate, evalDate);
                dt.Rows.Add(zSpreads[i], zNPV);
            }

            return dt;
        }


        public static DataTable ZeroCouponDirect()
        {
            DataTable res = new DataTable();

            Date evalDate = new Date(15, Month.January, 2015);
            double[] coupons = new double[] { 0.05, 0.055, 0.05, 0.06 };
            double[] bondPrices = new double[] { 101.0, 101.5, 99.0, 100.0 };
            Date[] maturities = new Date[]
            {
                new Date(15, Month.January, 2016),
                new Date(15, Month.January, 2017),
                new Date(15, Month.January, 2018),
                new Date(15, Month.January, 2019)
            };

            Settings.instance().setEvaluationDate(evalDate);
            Calendar calendar = new UnitedStates();
            DayCounter dc = new ActualActual(ActualActual.Convention.Bond);
            RateHelperVector rateHelpers = new RateHelperVector();

            for (int i = 0; i < maturities.Length; i++)
            {
                QuoteHandle quote = new QuoteHandle(new SimpleQuote(bondPrices[i]));
                DoubleVector coupon = new DoubleVector
                {
                    coupons[i]
                };
                Schedule schedule = new Schedule(evalDate, maturities[i], new Period(Frequency.Annual), calendar,
                    BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, true);
                FixedRateBondHelper helper = new FixedRateBondHelper(quote, 0, 100, schedule, coupon, dc, BusinessDayConvention.Unadjusted, 100.0, evalDate);
                rateHelpers.Add(helper);
            }

            PiecewiseLinearZero discountTS = new PiecewiseLinearZero(evalDate, rateHelpers, dc);
            //YieldTermStructure discountTS = new PiecewiseLinearZero(evalDate, rateHelpers, dc);

            res = new DataTable();
            res.Columns.Add("Maturity", typeof(string));
            res.Columns.Add("Zero Coupon Rate: R", typeof(string));
            res.Columns.Add("Equivalent Rate: Rc", typeof(string));
            res.Columns.Add("Discount Rate: B", typeof(string));

            foreach (Date d in discountTS.dates())
            {
                if (d > evalDate)
                {
                    double years = dc.yearFraction(evalDate, d);

                    InterestRate zeroRate = discountTS.zeroRate(years, Compounding.Compounded, Frequency.Annual);
                    double discount = discountTS.discount(d);
                    double eqRate = zeroRate.equivalentRate(dc, Compounding.Compounded, Frequency.Daily, evalDate, d).rate();
                    res.Rows.Add(d.ToDatetime<string>(), zeroRate.rate(), eqRate, discount);
                }
            }

            return res;
        }


        public static DataTable ZeroCouponBootstrap(double[] depositRates, Period[] depositMaturities, double[] bondPrices, double[] bondCoupons, Period[] bondMaturities, string curveType)
        {
            DataTable res = new DataTable();
            Date evalDate = new Date(15, Month.January, 2015);
            Settings.instance().setEvaluationDate(evalDate);
            Calendar calendar = new UnitedStates();
            DayCounter dc = new ActualActual(ActualActual.Convention.Bond);

            RateHelperVector rateHelpers = new RateHelperVector();
            for (int i = 0; i < depositMaturities.Length; i++)
            {
                rateHelpers.Add(new DepositRateHelper(depositRates[i], depositMaturities[i], 0, calendar, BusinessDayConvention.Unadjusted, true, dc));
            }

            for (int i = 0; i < bondMaturities.Length; i++)
            {
                QuoteHandle quote = new QuoteHandle(new SimpleQuote(bondPrices[i]));
                DoubleVector coupon = new DoubleVector
                {
                    bondCoupons[i]
                };
                Date maturity = evalDate.Add(bondMaturities[i]);
                Schedule schedule = new Schedule(evalDate, maturity, new Period(Frequency.Annual), calendar,
                    BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, true);
                FixedRateBondHelper helper = new FixedRateBondHelper(quote, 0, 100.0, schedule, coupon,
                    dc, BusinessDayConvention.Unadjusted, 100.0, evalDate);
                rateHelpers.Add(helper);
            }

            PiecewiseLinearZero ts = new PiecewiseLinearZero(evalDate, rateHelpers, dc);

            res = new DataTable();
            res.Columns.Add("Maturity", typeof(string));
            res.Columns.Add("TimesToMaturity", typeof(double));
            res.Columns.Add("Zero Coupon Rate: R", typeof(double));
            res.Columns.Add("Equivalent Rate: Rc", typeof(double));
            res.Columns.Add("Discount Rate: B", typeof(double));

            if (curveType == "DataPoints")
            {
                foreach (Date d in ts.dates())
                {
                    if (d > evalDate)
                    {
                        double years = dc.yearFraction(evalDate, d);
                        InterestRate zeroRate = ts.zeroRate(years, Compounding.Compounded, Frequency.Annual);
                        double discount = ts.discount(d);
                        double eqRate = zeroRate.equivalentRate(dc, Compounding.Compounded, Frequency.Daily, evalDate, d).rate();
                        res.Rows.Add(d.ToDatetime<string>(), years, zeroRate.rate(), eqRate, discount);
                    }
                }
            }
            else
            {
                Date d = evalDate.Add(depositMaturities[0]);
                Date lastDate = evalDate.Add(new Period(3, TimeUnit.Years));
                while (d < lastDate)
                {
                    double years = dc.yearFraction(evalDate, d);
                    InterestRate zeroRate = ts.zeroRate(years, Compounding.Compounded, Frequency.Annual);
                    double discount = ts.discount(d);
                    double eqRate = zeroRate.equivalentRate(dc, Compounding.Compounded, Frequency.Daily, evalDate, d).rate();
                    res.Rows.Add(d.ToDatetime<string>(), years, zeroRate.rate(), eqRate, discount);
                    d = d.Add(new Period(1, TimeUnit.Months));
                }
            }
            return res;
        }

        public static DataTable ZeroCouponBootstrapZSpread(double[] depositRates, Period[] depositMaturities, double[] bondPrices, double[] bondCoupons, Period[] bondMaturities, string curveType, double zSpread)
        {
            DataTable res = new DataTable();
            Date evalDate = new Date(15, Month.January, 2015);
            Settings.instance().setEvaluationDate(evalDate);
            Calendar calendar = new UnitedStates();
            DayCounter dc = new ActualActual(ActualActual.Convention.Bond);

            RateHelperVector rateHelpers = new RateHelperVector();
            for (int i = 0; i < depositMaturities.Length; i++)
            {
                rateHelpers.Add(new DepositRateHelper(depositRates[i], depositMaturities[i], 0, calendar, BusinessDayConvention.Unadjusted, true, dc));
            }

            for (int i = 0; i < bondMaturities.Length; i++)
            {
                QuoteHandle quote = new QuoteHandle(new SimpleQuote(bondPrices[i]));
                DoubleVector coupon = new DoubleVector
                {
                    bondCoupons[i]
                };
                Date maturity = evalDate.Add(bondMaturities[i]);
                Schedule schedule = new Schedule(evalDate, maturity, new Period(Frequency.Annual), calendar,
                    BusinessDayConvention.Unadjusted, BusinessDayConvention.Unadjusted, DateGeneration.Rule.Backward, true);
                FixedRateBondHelper helper = new FixedRateBondHelper(quote, 0, 100.0, schedule, coupon,
                    dc, BusinessDayConvention.Unadjusted, 100.0, evalDate);
                rateHelpers.Add(helper);
            }

            PiecewiseLinearZero ts = new PiecewiseLinearZero(evalDate, rateHelpers, dc);

            YieldTermStructureHandle tsHandle = new YieldTermStructureHandle(ts);
            QuoteHandle zsHandle = new QuoteHandle(new SimpleQuote(zSpread / 10000.0));
            ZeroSpreadedTermStructure zs = new ZeroSpreadedTermStructure(tsHandle, zsHandle);


            res = new DataTable();
            res.Columns.Add("Maturity", typeof(string));
            res.Columns.Add("TimesToMaturity", typeof(double));
            res.Columns.Add("Zero Coupon Rate: R", typeof(double));
            res.Columns.Add("Discount Rate: B", typeof(double));
            res.Columns.Add("Zero Coupon Rate: R with ZSpread", typeof(double));
            res.Columns.Add("Discount Rate: B with ZSpread", typeof(double));

            if (curveType == "DataPoints")
            {
                foreach (Date d in ts.dates())
                {
                    if (d > evalDate)
                    {
                        double years = dc.yearFraction(evalDate, d);
                        InterestRate zeroRate = ts.zeroRate(years, Compounding.Compounded, Frequency.Annual);
                        double discount = ts.discount(d);

                        InterestRate zeroRate1 = zs.zeroRate(years, Compounding.Compounded, Frequency.Annual);
                        double discount1 = zs.discount(d);
                        res.Rows.Add(d.ToDatetime<string>(), years, zeroRate.rate(), discount, zeroRate1.rate(), discount1);
                    }
                }
            }
            else
            {
                Date d = evalDate.Add(depositMaturities[0]);
                Date lastDate = evalDate.Add(new Period(3, TimeUnit.Years));
                while (d < lastDate)
                {
                    double years = dc.yearFraction(evalDate, d);
                    InterestRate zeroRate = ts.zeroRate(years, Compounding.Compounded, Frequency.Annual);
                    double discount = ts.discount(d);

                    InterestRate zeroRate1 = zs.zeroRate(years, Compounding.Compounded, Frequency.Annual);
                    double discount1 = zs.discount(d);

                    res.Rows.Add(d.ToDatetime<string>(), years, zeroRate.rate(), discount, zeroRate1.rate(), discount1);
                    d = d.Add(new Period(1, TimeUnit.Months));
                }
            }
            return res;
        }




        public static YieldTermStructure InterbankTermStructure(DateTime settlementDate1, double[] depositRates, Period[] depositMaturities,
            double[] futPrices, double[] swapRates, Period[] swapMaturities)
        {
            Calendar calendar = new JointCalendar(new UnitedKingdom(UnitedKingdom.Market.Exchange), new UnitedStates(UnitedStates.Market.Settlement));
            Date settlementDate = settlementDate1.To<Date>();
            //settlementDate = calendar.adjust(settlementDate);            
            int fixingDays = 2;
            Date evalDate = calendar.advance(settlementDate, -fixingDays, TimeUnit.Days);
            Settings.instance().setEvaluationDate(evalDate);

            RateHelperVector rateHelpers = new RateHelperVector();

            // Money market - Deposit:
            DayCounter depositDayCounter = new Actual360();
            for (int i = 0; i < depositRates.Length; i++)
                rateHelpers.Add(new DepositRateHelper(depositRates[i], depositMaturities[i], (uint)fixingDays, calendar, BusinessDayConvention.ModifiedFollowing, true, depositDayCounter));

            // Futures contracts:
            DayCounter futDayCounter = new Actual360();
            Date imm = IMM.nextDate(settlementDate);
            uint futMonths = 3;
            for (int i = 0; i < futPrices.Length; i++)
            {
                rateHelpers.Add(new FuturesRateHelper(futPrices[i], imm, futMonths, calendar, BusinessDayConvention.ModifiedFollowing, true, futDayCounter));
                imm = IMM.nextDate(imm + 1);
            }

            // Swap rates:
            Frequency swFixedLegFrequency = Frequency.Annual;
            BusinessDayConvention swFixedLegConvention = BusinessDayConvention.Unadjusted;
            DayCounter swFixedLegDayCounter = new Actual360();
            IborIndex swFloatingLegIndex = new USDLibor(new Period(3, TimeUnit.Months));
            for (int i = 0; i < swapRates.Length; i++)
                rateHelpers.Add(new SwapRateHelper(swapRates[i], swapMaturities[i], calendar, swFixedLegFrequency, swFixedLegConvention, swFixedLegDayCounter, swFloatingLegIndex));

            // Term structure:
            DayCounter tsDayCounter = new Actual360();
            PiecewiseCubicZero yieldTS = new PiecewiseCubicZero(settlementDate, rateHelpers, tsDayCounter);

            return yieldTS;
        }

        public static DataTable InterbankZeroCoupon(DateTime settlementDate1, double[] depositRates, Period[] depositMaturities,
            double[] futPrices, double[] swapRates, Period[] swapMaturities)
        {
            Date settlementDate = settlementDate1.To<Date>();
            DataTable res = new DataTable();
            PiecewiseCubicZero ts = (PiecewiseCubicZero)InterbankTermStructure(settlementDate1, depositRates, depositMaturities, futPrices, swapRates, swapMaturities);

            res = new DataTable();
            res.Columns.Add("Maturity", typeof(string));
            res.Columns.Add("TimesToMaturity", typeof(double));
            res.Columns.Add("Zero Coupon Rate: R", typeof(double));
            res.Columns.Add("Equivalent Rate: Rc", typeof(double));
            res.Columns.Add("Discount Rate: B", typeof(double));

            DayCounter dc = new Actual360();
            Date lastdate = settlementDate.Add(swapMaturities[swapMaturities.Length - 1]).Add(1);
            foreach (Date d in ts.dates())
            {
                if (d > settlementDate && d < lastdate)
                {
                    double years = dc.yearFraction(settlementDate, d);
                    InterestRate zeroRate = ts.zeroRate(years, Compounding.Compounded, Frequency.Annual);
                    double discount = ts.discount(d);
                    double eqRate = zeroRate.equivalentRate(dc, Compounding.Compounded, Frequency.Daily, settlementDate, d).rate();
                    res.Rows.Add(d.ToDatetime<string>(), years, zeroRate.rate(), eqRate, discount);
                }
            }
            return res;
        }


        public static DataTable HullWhiteInterestRates(DateTime evalDate1, double sigma, double a, double flatForwardRate)
        {
            Date evalDate = evalDate1.To<Date>();
            Settings.instance().setEvaluationDate(evalDate);
            DayCounter dc = new Thirty360();
            QuoteHandle quoteHandle = new QuoteHandle(new SimpleQuote(flatForwardRate));
            FlatForward flatCurve = new FlatForward(evalDate, quoteHandle, dc, Compounding.Compounded, Frequency.Semiannual);
            YieldTermStructureHandle flatCurveHandle = new YieldTermStructureHandle(flatCurve);
            HullWhite hw = new HullWhite(flatCurveHandle);







            DataTable res = new DataTable();
            res.Columns.Add("Maturity", typeof(string));
            res.Columns.Add("Discount Rate", typeof(double));
            res.Columns.Add("param", typeof(object));


            for (int i = 0; i < 30; i++)
            {
                double dd = 1.0 * i;
                double discount = hw.discount(dd);

                res.Rows.Add(dd, discount, hw.parameters().size());
            }

            return res;
        }



        public static DataTable ConvertibleBondPrice(ConvertableBondEngineType engineType)
        {
            double spot = 36.0;
            double spreadRate = 0.005;
            double dividendYield = 0.02;
            double riskFreeRate = 0.06;
            double vol = 0.2;
            int settlementDays = 3;
            int length = 5;
            double redemprion = 100.0;
            double conversionRatio = redemprion / spot;

            Calendar calendar = new TARGET();
            Date today = calendar.adjust(Date.todaysDate());
            Settings.instance().setEvaluationDate(today);
            Date settlementDate = calendar.advance(today, new Period(settlementDays, TimeUnit.Days));
            Date exerciseDate = calendar.advance(settlementDate, new Period(length, TimeUnit.Years));
            Date issueDate = calendar.advance(exerciseDate, new Period(-length, TimeUnit.Years));
            BusinessDayConvention convention = BusinessDayConvention.ModifiedFollowing;
            Frequency frequency = Frequency.Annual;
            Schedule schedule = new Schedule(issueDate, exerciseDate, new Period(frequency), calendar, convention, convention, DateGeneration.Rule.Backward, false);
            DividendSchedule dividends = new DividendSchedule();
            CallabilitySchedule callability = new CallabilitySchedule();
            DoubleVector coupons = new DoubleVector
            {
                0.05
            };

            DayCounter bondDayCounter = new Thirty360();

            uint[] callLength = new uint[] { 2, 4 };
            uint[] putLength = new uint[] { 3 };
            double[] callPrices = new double[] { 101.5, 100.85 };
            double[] putPrices = new double[] { 105.0 };

            for (int i = 0; i < callLength.Length; i++)
            {
                callability.Add(new SoftCallability(new CallabilityPrice(callPrices[i], CallabilityPrice.Type.Clean), schedule.date(callLength[i]), 1.2));
            }

            for (int i = 0; i < putLength.Length; i++)
            {
                callability.Add(new Callability(new CallabilityPrice(putPrices[i], CallabilityPrice.Type.Clean), Callability.Put, schedule.date(putLength[i])));
            }

            // Assume dividends are paid every 6 months.
            for (Date d = today.Add(new Period(6, TimeUnit.Months)); d < exerciseDate; d = d.Add(new Period(6, TimeUnit.Months)))
            {
                dividends.Add(new FixedDividend(1.0, d));
            }

            DayCounter dayCounter = new Actual365Fixed();
            double maturity = dayCounter.yearFraction(settlementDate, exerciseDate);

            Exercise exercise = new EuropeanExercise(exerciseDate);
            Exercise amExercise = new AmericanExercise(settlementDate, exerciseDate);

            QuoteHandle underlyingH = new QuoteHandle(new SimpleQuote(spot));
            YieldTermStructureHandle flatTS = new YieldTermStructureHandle(new FlatForward(settlementDate, riskFreeRate, dayCounter));
            YieldTermStructureHandle flatDividendTS = new YieldTermStructureHandle(new FlatForward(settlementDate, dividendYield, dayCounter));
            BlackVolTermStructureHandle flatVolTS = new BlackVolTermStructureHandle(new BlackConstantVol(settlementDate, calendar, vol, dayCounter));
            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(underlyingH, flatDividendTS, flatTS, flatVolTS);

            uint timeSteps = 801;
            QuoteHandle creditSpread = new QuoteHandle(new SimpleQuote(spreadRate));

            PricingEngine engine = new BinomialConvertibleEngine(bsmProcess, "jarrowrudd", timeSteps);

            ConvertibleFixedCouponBond europeanBond = new ConvertibleFixedCouponBond(exercise, conversionRatio, dividends, callability, creditSpread,
                issueDate, settlementDays, coupons, bondDayCounter, schedule, redemprion);
            europeanBond.setPricingEngine(engine);

            ConvertibleFixedCouponBond americanBond = new ConvertibleFixedCouponBond(amExercise, conversionRatio, dividends, callability, creditSpread,
                issueDate, settlementDays, coupons, bondDayCounter, schedule, redemprion);
            americanBond.setPricingEngine(engine);

            // select pricing engine
            if (engineType == ConvertableBondEngineType.Binomial_Jarrow_Rudd)
                engine = new BinomialConvertibleEngine(bsmProcess, "jarrowrudd", timeSteps);
            else if (engineType == ConvertableBondEngineType.Binomial_Cox_Ross_Rubinstein)
                engine = new BinomialConvertibleEngine(bsmProcess, "coxrossrubinstein", timeSteps);
            else if (engineType == ConvertableBondEngineType.Binomial_Additive_Equiprobabilities)
                engine = new BinomialConvertibleEngine(bsmProcess, "eqp", timeSteps);
            else if (engineType == ConvertableBondEngineType.Binomial_Trigeorgis)
                engine = new BinomialConvertibleEngine(bsmProcess, "trigeorgis", timeSteps);
            else if (engineType == ConvertableBondEngineType.Binomial_Tian)
                engine = new BinomialConvertibleEngine(bsmProcess, "tian", timeSteps);
            else if (engineType == ConvertableBondEngineType.Binomial_Leisen_Reimer)
                engine = new BinomialConvertibleEngine(bsmProcess, "leisenreimer", timeSteps);
            else if (engineType == ConvertableBondEngineType.Binomial_Joshi)
                engine = new BinomialConvertibleEngine(bsmProcess, "joshi4", timeSteps);

            europeanBond.setPricingEngine(engine);
            americanBond.setPricingEngine(engine);

            DataTable dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Value", typeof(string));

            dt.Rows.Add("Pricing Engine", engineType);
            dt.Rows.Add("Times To Maturity", Math.Round(maturity, 0));
            dt.Rows.Add("Spot", spot);
            dt.Rows.Add("Risk Free Interest Rate", riskFreeRate);
            dt.Rows.Add("Dividend Yield", dividendYield);
            dt.Rows.Add("Volatility", vol);
            dt.Rows.Add("European Bond Price", europeanBond.NPV());
            dt.Rows.Add("American Bond Price", americanBond.NPV());
            return dt;
        }


        public static DataTable CdsHazardRate(Date evalDate, double recoveryRate, double[] spreads, string[] tenors, bool isDataPoints)
        {
            PeriodVector periods = TenorStringToPeriodVector(tenors);
            Calendar calendar = new TARGET();
            evalDate = calendar.adjust(evalDate);
            Settings.instance().setEvaluationDate(evalDate);

            // dummy curve
            QuoteHandle flatRateH = new QuoteHandle(new SimpleQuote(0.01));
            YieldTermStructureHandle tsCurve = new YieldTermStructureHandle(new FlatForward(evalDate, flatRateH, new Actual365Fixed()));

            DateVector maturities = new DateVector();
            for (int i = 0; i < periods.Count; i++)
                maturities.Add(calendar.adjust(evalDate.Add(periods[i]), BusinessDayConvention.Following));

            DefaultProbabilityHelperVector instruments = new DefaultProbabilityHelperVector();
            for (int i = 0; i < periods.Count; i++)
            {
                QuoteHandle qh = new QuoteHandle(new SimpleQuote(spreads[i] / 10000.0));
                instruments.Add(new SpreadCdsHelper(qh, periods[i], 0, calendar, Frequency.Quarterly,
                    BusinessDayConvention.Following, DateGeneration.Rule.TwentiethIMM, new Actual365Fixed(), recoveryRate, tsCurve));
            }

            // bootstrap hazard rates
            PiecewiseFlatHazardRate hazardRateStructure = new PiecewiseFlatHazardRate(evalDate, instruments, new Actual365Fixed());

            DataTable res = new DataTable();
            res.Columns.Add("Maturity", typeof(string));
            res.Columns.Add("TimesToMaturity", typeof(double));
            res.Columns.Add("Hazard Rate (%)", typeof(double));
            res.Columns.Add("Survival Probability (%)", typeof(double));
            res.Columns.Add("Default Probability (%)", typeof(double));

            DayCounter dc = new Actual365Fixed();

            if (isDataPoints)
            {
                foreach (Date d in hazardRateStructure.dates())
                {
                    double years = dc.yearFraction(evalDate, d);
                    double hazard = Math.Round(100.0 * hazardRateStructure.hazardRate(d), 5);
                    double survive = Math.Round(100.0 * hazardRateStructure.survivalProbability(d), 5);
                    double def = Math.Round(100.0 * hazardRateStructure.defaultProbability(d), 5);
                    res.Rows.Add(d.ToDatetime<string>(), Math.Round(years, 0), hazard, survive, def);

                }
            }
            else
            {
                Date dd = evalDate;
                Date lastDate = maturities[maturities.Count - 1];
                while (dd < lastDate)
                {
                    double years = dc.yearFraction(evalDate, dd);
                    double hazard = Math.Round(100.0 * hazardRateStructure.hazardRate(dd), 5);
                    double survive = Math.Round(100.0 * hazardRateStructure.survivalProbability(dd), 5);
                    double def = Math.Round(100.0 * hazardRateStructure.defaultProbability(dd), 5);
                    res.Rows.Add(dd.ToDatetime<string>(), Math.Round(years, 4), hazard, survive, def);
                    dd = dd.Add(new Period(1, TimeUnit.Days));
                }
            }

            return res;
        }

        private static PeriodVector TenorStringToPeriodVector(string[] tenors)
        {
            PeriodVector periods = new PeriodVector();
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


        private static YieldTermStructure IsdaZeroCurve(DateTime evalDate, string ccy)
        {
            DateTime rateDate = ModelHelper.Get_previous_workday(evalDate);
            Calendar calendar = new TARGET();

            BindableCollection<IsdaRate> isdaRates = IsdaHelper.GetIsdaRates(ccy, rateDate, rateDate);

            Date date = rateDate.To<Date>();
            RateHelperVector rateHelpers = new RateHelperVector();

            PeriodVector depositMaturities = new PeriodVector();
            DoubleVector depositRates = new DoubleVector();
            PeriodVector swapMaturities = new PeriodVector();
            DoubleVector swapRates = new DoubleVector();
            DayCounter depositDayCounter = new Actual360();
            DayCounter swapDayCounter = new Thirty360(Thirty360.Convention.USA);

            int fixedDays = 0;

            foreach (IsdaRate item in isdaRates)
            {
                fixedDays = ModelHelper.Get_number_calendar_days(item.SnapTime.To<DateTime>().Date, item.SpotDate.To<DateTime>().Date);
                int num = 0;
                if (string.IsNullOrEmpty(item.FixedDayCountConvention))
                {
                    if (item.Tenor.Contains("M"))
                    {
                        num = item.Tenor.Split('M')[0].To<int>();
                        depositMaturities.Add(new Period(num, TimeUnit.Months));
                        depositRates.Add(item.Rate.To<double>());
                    }
                    else if (item.Tenor.Contains("Y"))
                    {
                        num = item.Tenor.Split('Y')[0].To<int>();
                        depositMaturities.Add(new Period(num, TimeUnit.Years));
                        depositRates.Add(item.Rate.To<double>());
                    }
                }
                else
                {
                    num = item.Tenor.Split('Y')[0].To<int>();
                    swapMaturities.Add(new Period(num, TimeUnit.Years));
                    swapRates.Add(item.Rate.To<double>());
                }
            }

            for (int i = 0; i < depositRates.Count; i++)
                rateHelpers.Add(new DepositRateHelper(depositRates[i], depositMaturities[i], 0, calendar, BusinessDayConvention.Unadjusted, false, depositDayCounter));

            Frequency swFixedLegFrequency = Frequency.Semiannual;
            BusinessDayConvention swFixedLegConvention = BusinessDayConvention.Unadjusted;
            IborIndex swFloatingLegIndex = new USDLibor(new Period(3, TimeUnit.Months));
            for (int i = 0; i < swapRates.Count; i++)
            {
                QuoteHandle qh = new QuoteHandle(new SimpleQuote(swapRates[i]));
                rateHelpers.Add(new SwapRateHelper(qh, swapMaturities[i], calendar, swFixedLegFrequency, swFixedLegConvention, swapDayCounter, swFloatingLegIndex));
            }

            return new PiecewiseLogCubicDiscount(date, rateHelpers, new Actual365Fixed());
        }

        public static DataTable CdsPV(Protection.Side protectionSide, string ccy, Date evalDate, Date effectiveDate, Date maturity, double recoveryRate, double[] spreads, string[] tenors,
            double notional, Frequency couponFrequency, double coupon, object flatRate = null)
        {
            PeriodVector periods = TenorStringToPeriodVector(tenors);
            Calendar calendar = new TARGET();
            evalDate = calendar.adjust(evalDate);
            Settings.instance().setEvaluationDate(evalDate);
            YieldTermStructureHandle tsCurve;
            if (flatRate == null)
                tsCurve = new YieldTermStructureHandle(IsdaZeroCurve(evalDate.ToDatetime<DateTime>(), ccy));
            else
                tsCurve = new YieldTermStructureHandle(new FlatForward(evalDate, new QuoteHandle(new SimpleQuote((double)flatRate)), new Actual365Fixed()));


            DefaultProbabilityHelperVector instruments = new DefaultProbabilityHelperVector();
            for (int i = 0; i < periods.Count; i++)
            {
                QuoteHandle qh = new QuoteHandle(new SimpleQuote(spreads[i] / 10000.0));
                instruments.Add(new SpreadCdsHelper(qh, periods[i], 1, calendar, couponFrequency,
                    BusinessDayConvention.Following, DateGeneration.Rule.TwentiethIMM, new Actual365Fixed(), recoveryRate, tsCurve));
            }

            // bootstrap hazard rates
            PiecewiseFlatHazardRate hazardRateStructure = new PiecewiseFlatHazardRate(evalDate, instruments, new Actual365Fixed());
            DefaultProbabilityTermStructureHandle probability = new DefaultProbabilityTermStructureHandle(hazardRateStructure);

            DataTable res = new DataTable();
            res.Columns.Add("Name", typeof(string));
            res.Columns.Add("Value", typeof(string));
            res.Rows.Add("Maturity", maturity.ToDatetime<DateTime>().ToShortDateString());
            res.Rows.Add("Coupon", coupon);

            try
            {
                Schedule cdsSchedule = new Schedule(effectiveDate, maturity, new Period(couponFrequency), calendar, BusinessDayConvention.Following,
                    BusinessDayConvention.Following, DateGeneration.Rule.TwentiethIMM, false);
                CreditDefaultSwap cds = new CreditDefaultSwap(protectionSide, notional, coupon / 10000.0, cdsSchedule, BusinessDayConvention.ModifiedFollowing, new ActualActual(), false);
                PricingEngine engine = new MidPointCdsEngine(probability, recoveryRate, tsCurve);
                cds.setPricingEngine(engine);

                double surv = 100.0 * hazardRateStructure.survivalProbability(maturity);
                double hazard = 100.0 * hazardRateStructure.hazardRate(maturity);
                double def = 100.0 * hazardRateStructure.defaultProbability(maturity);
                double npv = cds.NPV();

                //var defLeg = cds.defaultLegNPV();
                //var couponLeg = cds.couponLegNPV();
                double fairSpread = 10000.0 * cds.fairSpread();
                res.Rows.Add("SurvivalProb", surv);
                res.Rows.Add("HazardRate", hazard);
                res.Rows.Add("DefaultProb", def);
                res.Rows.Add("FairSpread", fairSpread);
                res.Rows.Add("PresentValue", npv);
                //res.Rows.Add("FeeLeg", defLeg);
                //res.Rows.Add("ContingentLeg", couponLeg);
                System.Windows.MessageBox.Show(cds.fairUpfront().ToString());
            }
            catch { }
            return res;
        }

        public static DataTable CdsPrice(Protection.Side protectionSide, string ccy, Date evalDate, Date effectiveDate, Date maturity, double recoveryRate, double[] spreads, string[] tenors,
           Frequency couponFrequency, double coupon, object flatRate = null)
        {
            double notional = 100.0;
            int numDays = ModelHelper.Get_number_calendar_days(effectiveDate.ToDatetime<DateTime>(), evalDate.ToDatetime<DateTime>()) + 1;
            double accrual = coupon * numDays / 360.0 / 100.0;
            DataTable dtCds = CdsPV(protectionSide, ccy, evalDate, effectiveDate, maturity, recoveryRate, spreads, tenors, notional, couponFrequency, coupon, flatRate);

            double upfront = 0;
            foreach (DataRow r in dtCds.Rows)
            {
                if (r["Name"].ToString() == "PresentValue")
                    upfront = r["Value"].To<double>();
            }


            double cleanPrice = 100;
            double dirtyPrice = 100;
            if (protectionSide == Protection.Side.Buyer)
            {
                accrual = -accrual;
                dirtyPrice = 100 - upfront;
                cleanPrice = dirtyPrice + accrual;
            }
            else if (protectionSide == Protection.Side.Seller)
            {
                dirtyPrice = 100 + upfront;
                cleanPrice = dirtyPrice - accrual;
            }

            double coupon1 = 0;
            double parSpread = 0;
            foreach (DataRow r in dtCds.Rows)
            {
                if (r["Name"].ToString() == "FairSpread")
                {
                    parSpread = r["Value"].To<double>();
                    coupon1 = parSpread + 1.0;
                }
            }
            DataTable dt = CdsPV(protectionSide, ccy, evalDate, effectiveDate, maturity, recoveryRate, spreads, tenors, notional, couponFrequency, coupon1, flatRate);
            double dv01 = 0;
            foreach (DataRow r in dt.Rows)
            {
                if (r["Name"].ToString() == "PresentValue")
                    dv01 = r["Value"].To<double>();
            }

            DataTable res = new DataTable();
            res.Columns.Add("Name", typeof(string));
            res.Columns.Add("Value", typeof(string));

            res.Rows.Add("Accrual", accrual);
            res.Rows.Add("Upfront", upfront);
            res.Rows.Add("CleanPrice", cleanPrice);
            res.Rows.Add("DirtyPrice", dirtyPrice);
            res.Rows.Add("RiskyAnnuity", dv01);
            return res;
        }
    }










    public enum ConvertableBondEngineType
    {
        Binomial_Jarrow_Rudd,
        Binomial_Cox_Ross_Rubinstein,
        Binomial_Additive_Equiprobabilities,
        Binomial_Trigeorgis,
        Binomial_Tian,
        Binomial_Leisen_Reimer,
        Binomial_Joshi,
    };
}
