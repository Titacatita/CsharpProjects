using System;
using QuantLib;
using System.Windows;
using System.Data;

namespace QuantBook.Models.Options
{
    public static class QuantLibHelper
    {
        #region European options
        public static object[] EuropeanOption(string optionType, DateTime evalDate, double yearsToMaturity, double strike, double spot, double q, double r,
            double vol, EuropeanEngineType priceEngineType, int timeSteps)
        {
            Date startDate = new Date((int)evalDate.ToOADate());
            Date maturity = startDate + Convert.ToInt32(yearsToMaturity * 360 + 0.5);
            return EuropeanOption(optionType, startDate, maturity, strike, spot, q, r, vol, priceEngineType, timeSteps);
        }

        public static object[] EuropeanOption(string optionType, Date evalDate, Date maturity, double strike, double spot, double q, double r,
           double vol, EuropeanEngineType priceEngineType, int timeSteps)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            Settings.instance().setEvaluationDate(evalDate);
            Date settlementDate = evalDate;
            Calendar calendar = new TARGET();
            DayCounter dc = new Actual360();
            QuoteHandle spot1 = new QuoteHandle(new SimpleQuote(spot));
            YieldTermStructureHandle qTS = new YieldTermStructureHandle(new FlatForward(settlementDate, q, dc));
            YieldTermStructureHandle rTS = new YieldTermStructureHandle(new FlatForward(settlementDate, r, dc));
            BlackVolTermStructureHandle volTS = new BlackVolTermStructureHandle(new BlackConstantVol(settlementDate, calendar, vol, dc));
            Payoff payoff = new PlainVanillaPayoff(otype, strike);
            Exercise exercise = new EuropeanExercise(maturity);
            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(spot1, qTS, rTS, volTS);            

            PricingEngine engine;
            switch (priceEngineType)
            {
                case EuropeanEngineType.Analytic:
                    engine = new AnalyticEuropeanEngine(bsmProcess);
                    break;
                case EuropeanEngineType.Binomiall_Jarrow_Rudd:
                    engine = new BinomialVanillaEngine(bsmProcess, "jarrowrudd", (uint)timeSteps);
                    break;
                case EuropeanEngineType.Binomial_Cox_Ross_Rubinstein:
                    engine = new BinomialVanillaEngine(bsmProcess, "coxrossrubinstein", (uint)timeSteps);
                    break;
                case EuropeanEngineType.Binomial_Additive_Equiprobabilities:
                    engine = new BinomialVanillaEngine(bsmProcess, "eqp", (uint)timeSteps);
                    break;
                case EuropeanEngineType.Binomial_Trigeorgis:
                    engine = new BinomialVanillaEngine(bsmProcess, "trigeorgis", (uint)timeSteps);
                    break;
                case EuropeanEngineType.Binomial_Tian:
                    engine = new BinomialVanillaEngine(bsmProcess, "tain", (uint)timeSteps);
                    break;
                case EuropeanEngineType.Binomial_Leisen_Reimer:
                    engine = new BinomialVanillaEngine(bsmProcess, "leisenreimer", (uint)timeSteps);
                    break;
                case EuropeanEngineType.Binomial_Joshi:
                    engine = new BinomialVanillaEngine(bsmProcess, "joshi4", (uint)timeSteps);
                    break;
                case EuropeanEngineType.FiniteDifference:
                    engine = new FDEuropeanEngine(bsmProcess, (uint)timeSteps, (uint)timeSteps - 1);
                    break;
                case EuropeanEngineType.Integral:
                    engine = new IntegralEngine(bsmProcess);
                    break;
                case EuropeanEngineType.PseudoMonteCarlo:
                    string traits = "pseudorandom";
                    int mcTimeSteps = 1;
                    int timeStepsPerYear = int.MaxValue;
                    bool brownianBridge = false;
                    bool antitheticVariate = false;
                    int requiredSamples = int.MaxValue;
                    double requiredTolerance = 0.05;
                    int maxSamples = int.MaxValue;
                    int seed = 42;
                    engine = new MCEuropeanEngine(bsmProcess, traits, mcTimeSteps, timeStepsPerYear, brownianBridge, antitheticVariate, requiredSamples, requiredTolerance, maxSamples, seed);
                    break;
                case EuropeanEngineType.QuasiMonteCarlo:
                    traits = "lowdiscrepancy";
                    mcTimeSteps = 1;
                    timeStepsPerYear = int.MaxValue;
                    brownianBridge = false;
                    antitheticVariate = false;
                    requiredSamples = 32768;  // 2^15
                    requiredTolerance = double.MaxValue;
                    maxSamples = int.MaxValue;
                    seed = 0;
                    engine = new MCEuropeanEngine(bsmProcess, traits, mcTimeSteps, timeStepsPerYear, brownianBridge, antitheticVariate, requiredSamples, requiredTolerance, maxSamples, seed);
                    break;

                default:
                    throw new ArgumentException("unknown engine type");
            }

            VanillaOption option = new VanillaOption(payoff, exercise);
            option.setPricingEngine(engine);

            object value = null;
            object delta = null;
            object gamma = null;
            object theta = null;
            object rho = null;
            object vega = null;

            try
            {
                value = option.NPV();
                delta = option.delta();
                gamma = option.gamma();
                theta = option.theta();
                rho = option.rho();
                vega = option.vega();
            }
            catch { }

            return new object[] { value, delta, gamma, theta, rho, vega };
        }

        public static object EuropeanOptionImpliedVol(string optionType, DateTime evalDate, double yearsToMaturity, double strike, double spot, double q, double r, double targetPrice)
        {
            Date startDate = new Date((int)evalDate.ToOADate());
            Date maturity = startDate + Convert.ToInt32(yearsToMaturity * 360 + 0.5);
            return EuropeanOptionImpliedVol(optionType, startDate, maturity, strike, spot, q, r, targetPrice);
        }

        public static object EuropeanOptionImpliedVol(string optionType, Date evalDate, Date maturity, double strike, double spot, double q, double r, double targetPrice)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            Settings.instance().setEvaluationDate(evalDate);
            Date settlementDate = evalDate;
            Calendar calendar = new TARGET();
            DayCounter dc = new Actual360();
            QuoteHandle spot1 = new QuoteHandle(new SimpleQuote(spot));
            YieldTermStructureHandle qTS = new YieldTermStructureHandle(new FlatForward(settlementDate, q, dc));
            YieldTermStructureHandle rTS = new YieldTermStructureHandle(new FlatForward(settlementDate, r, dc));
            BlackVolTermStructureHandle volTS = new BlackVolTermStructureHandle(new BlackConstantVol(settlementDate, calendar, 0.3, dc));
            Payoff payoff = new PlainVanillaPayoff(otype, strike);
            Exercise exercise = new EuropeanExercise(maturity);
            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(spot1, qTS, rTS, volTS);
            VanillaOption option = new VanillaOption(payoff, exercise);

            object v = null;
            try
            {
                v = option.impliedVolatility(targetPrice, bsmProcess, 1.0e-6, 10000, 0, 4.0);
            }
            catch { }

            return v;
        }

        #endregion European options












        #region American options
        public static object[] AmericanOption(string optionType, DateTime evalDate, double yearsToMaturity, double strike, double spot, double q, double r,
            double vol, AmericanEngineType priceEngineType, int timeSteps)
        {
            Date startDate = new Date((int)evalDate.ToOADate());
            Date maturity = startDate + Convert.ToInt32(yearsToMaturity * 360 + 0.5);
            return AmericanOption(optionType, startDate, maturity, strike, spot, q, r, vol, priceEngineType, timeSteps);
        }



        public static object[] AmericanOption(string optionType, Date evalDate, Date maturity, double strike, double spot, double q, double r, double vol,
            AmericanEngineType priceEngineType, int timeSteps)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            Settings.instance().setEvaluationDate(evalDate);
            Date settlementDate = evalDate;
            Calendar calendar = new TARGET();
            DayCounter dc = new Actual360();
            QuoteHandle spot1 = new QuoteHandle(new SimpleQuote(spot));
            YieldTermStructureHandle qTS = new YieldTermStructureHandle(new FlatForward(settlementDate, q, dc));
            YieldTermStructureHandle rTS = new YieldTermStructureHandle(new FlatForward(settlementDate, r, dc));
            BlackVolTermStructureHandle volTS = new BlackVolTermStructureHandle(new BlackConstantVol(settlementDate, calendar, vol, dc));
            Payoff payoff = new PlainVanillaPayoff(otype, strike);
            Exercise exercise = new AmericanExercise(settlementDate, maturity);
            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(spot1, qTS, rTS, volTS);

            PricingEngine engine;
            switch (priceEngineType)
            {
                case AmericanEngineType.Barone_Adesi_Whaley:
                    engine = new BaroneAdesiWhaleyEngine(bsmProcess);
                    break;
                case AmericanEngineType.Bjerksund_Stensland:
                    engine = new BjerksundStenslandEngine(bsmProcess);
                    break;
                case AmericanEngineType.FiniteDifference:
                    engine = new FDAmericanEngine(bsmProcess, (uint)timeSteps, (uint)timeSteps - 1);
                    break;
                case AmericanEngineType.Binomial_Jarrow_Rudd:
                    engine = new BinomialVanillaEngine(bsmProcess, "jarrowrudd", (uint)timeSteps);
                    break;
                case AmericanEngineType.Binomial_Cox_Ross_Rubinstein:
                    engine = new BinomialVanillaEngine(bsmProcess, "coxrossrubinstein", (uint)timeSteps);
                    break;
                case AmericanEngineType.Binomial_Additive_Equiprobabilities:
                    engine = new BinomialVanillaEngine(bsmProcess, "eqp", (uint)timeSteps);
                    break;
                case AmericanEngineType.Binomial_Trigeorgis:
                    engine = new BinomialVanillaEngine(bsmProcess, "trigeorgis", (uint)timeSteps);
                    break;
                case AmericanEngineType.Binomial_Tian:
                    engine = new BinomialVanillaEngine(bsmProcess, "tian", (uint)timeSteps);
                    break;
                case AmericanEngineType.Binomial_Leisen_Reimer:
                    engine = new BinomialVanillaEngine(bsmProcess, "leisenreimer", (uint)timeSteps);
                    break;
                case AmericanEngineType.Binomial_Joshi:
                    engine = new BinomialVanillaEngine(bsmProcess, "joshi4", (uint)timeSteps);
                    break;
                default:
                    throw new ArgumentException("unknown engine type");
            }

            VanillaOption option = new VanillaOption(payoff, exercise);
            option.setPricingEngine(engine);

            object value = null;
            object delta = null;
            object gamma = null;
            object theta = null;
            object rho = null;
            object vega = null;

            try
            {
                value = option.NPV();
                delta = option.delta();
                gamma = option.gamma();
                theta = option.theta();
                rho = option.rho();
                vega = option.vega();
            }
            catch { }
            return new object[] { value, delta, gamma, theta, rho, vega };
        }



        public static object AmericanOptionImpliedVol(string optionType, DateTime evalDate, double yearsToMaturity, double strike, double spot, double q, double r, double targetPrice)
        {
            Date startDate = new Date((int)evalDate.ToOADate());
            Date maturity = startDate + Convert.ToInt32(yearsToMaturity * 360 + 0.5);
            return AmericanOptionImpliedVol(optionType, startDate, maturity, strike, spot, q, r, targetPrice);
        }

        public static object AmericanOptionImpliedVol(string optionType, Date evalDate, Date maturity, double strike, double spot, double q, double r, double targetPrice)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            Settings.instance().setEvaluationDate(evalDate);
            Date settlementDate = evalDate;
            Calendar calendar = new TARGET();
            DayCounter dc = new Actual360();
            QuoteHandle spot1 = new QuoteHandle(new SimpleQuote(spot));
            YieldTermStructureHandle qTS = new YieldTermStructureHandle(new FlatForward(settlementDate, q, dc));
            YieldTermStructureHandle rTS = new YieldTermStructureHandle(new FlatForward(settlementDate, r, dc));
            BlackVolTermStructureHandle volTS = new BlackVolTermStructureHandle(new BlackConstantVol(settlementDate, calendar, 0.3, dc));
            Payoff payoff = new PlainVanillaPayoff(otype, strike);
            Exercise exercise = new AmericanExercise(settlementDate, maturity);
            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(spot1, qTS, rTS, volTS);
            VanillaOption option = new VanillaOption(payoff, exercise);

            object v = null;
            try
            {
                v = option.impliedVolatility(targetPrice, bsmProcess, 1.0e-6, 10000, 0, 4.0);
            }
            catch { }

            return v;
        }

        #endregion American options






        #region Barrier options
        public static object BarrierOption(string optionType, DateTime evalDate, double yearsToMaturity, double strike,
                     double spot, double barrierLevel, double rebate, double q, double r, double vol,
                      Barrier.Type barrierType, BarrierEngineType priceEngineType, int timeSteps)
        {
            Date evalDate1 = new Date((int)evalDate.ToOADate());
            Date maturity = evalDate1 + 2 + Convert.ToInt32(yearsToMaturity * 365 + 0.5);
            return BarrierOption(optionType, evalDate1, maturity, strike, spot, barrierLevel, rebate, q, r, vol, barrierType, priceEngineType, timeSteps);
        }

        public static object BarrierOption(string optionType, Date evalDate, Date maturity, double strike, double spot, double barrierLevel, double rebate,
           double q, double r, double vol, Barrier.Type barrierType, BarrierEngineType priceEngineType, int timeSteps)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            Calendar calendar = new TARGET();
            Date settleDate = evalDate;
            Settings.instance().setEvaluationDate(evalDate);
            DayCounter dc = new Actual365Fixed();
            Exercise exercise = new EuropeanExercise(maturity);

            QuoteHandle underlyingH = new QuoteHandle(new SimpleQuote(spot));
            YieldTermStructureHandle flatTermStructure = new YieldTermStructureHandle(new FlatForward(settleDate, r, dc));
            YieldTermStructureHandle flatDividendTS = new YieldTermStructureHandle(new FlatForward(settleDate, q, dc));
            BlackVolTermStructureHandle flatVolTS = new BlackVolTermStructureHandle(new BlackConstantVol(settleDate, calendar, vol, dc));
            PlainVanillaPayoff payoff = new PlainVanillaPayoff(otype, strike);
            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(underlyingH, flatDividendTS, flatTermStructure, flatVolTS);

            BarrierOption option = new BarrierOption(barrierType, barrierLevel, rebate, payoff, exercise);

            PricingEngine engine;
            
           
            switch(priceEngineType)
            {
                case BarrierEngineType.Analytic:
                    engine = new AnalyticBarrierEngine(bsmProcess);
                    break;
                case BarrierEngineType.MonteCarlo:
                    string traits = "pseudorandom";
                    uint mcTimeSteps = 1;
                    uint timeStepsPerYear = int.MaxValue;
                    bool brownianBridge = false;
                    bool antitheticVariate = false;
                    int requiredSamples = int.MaxValue;
                    double requiredTolerance = 0.05;
                    int maxSamples = int.MaxValue;
                    int seed = 42;
                    engine = new MCBarrierEngine(bsmProcess, traits, mcTimeSteps, timeStepsPerYear, brownianBridge, antitheticVariate, requiredSamples, requiredTolerance, maxSamples, false, seed);
                  
                    break;
                default:
                    throw new ArgumentException("unknown engine type");
            }

            option.setPricingEngine(engine);

            object v = null;
            try
            {
                v = option.NPV();
            }
            catch { }

            return v;
        }
        #endregion Barrier options






        #region Bermudan options
        public static object[] BermudanOption(string optionType, DateTime evalDate, int exerciseFrequency, int exerciseTimes, double strike, double spot, double q, double r,
            double vol, BermudanEngineType priceEngineType, int timeSteps)
        {
            Date evalDate1 = new Date((int)evalDate.ToOADate());
            Settings.instance().setEvaluationDate(evalDate1);
            Date settlementDate = evalDate1 + 2;

            DateVector exerciseDates = new DateVector(exerciseTimes);
            for (int i = 1; i <= exerciseTimes; i++)
            {
                Period forwordPeriod = new Period(exerciseFrequency * i, TimeUnit.Months);
                Date forwordDate = settlementDate.Add(forwordPeriod);
                exerciseDates.Add(forwordDate);
            }
            return BermudanOption(optionType, evalDate1, settlementDate, exerciseDates, strike, spot, q, r, vol, priceEngineType, timeSteps);
        }

        private static object[] BermudanOption(string optionType, Date evalDate, Date settlementDate, DateVector exerciseDates, double strike, double spot, double q, double r,
            double vol, BermudanEngineType priceEngineType, int timeSteps)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            Calendar calendar = new TARGET();
            Exercise exercise = new BermudanExercise(exerciseDates);
            DayCounter dc = new Actual360();
            QuoteHandle spot1 = new QuoteHandle(new SimpleQuote(spot));
            YieldTermStructureHandle qTS = new YieldTermStructureHandle(new FlatForward(settlementDate, q, dc));
            YieldTermStructureHandle rTS = new YieldTermStructureHandle(new FlatForward(settlementDate, r, dc));
            BlackVolTermStructureHandle volTS = new BlackVolTermStructureHandle(new BlackConstantVol(settlementDate, calendar, vol, dc));
            PlainVanillaPayoff payoff = new PlainVanillaPayoff(otype, strike);
            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(spot1, qTS, rTS, volTS);

            PricingEngine engine;
            switch (priceEngineType)
            {
                case BermudanEngineType.Binomial_Jarrow_Rudd:
                    engine = new BinomialVanillaEngine(bsmProcess, "jarrowrudd", (uint)timeSteps);
                    break;
                case BermudanEngineType.Binomial_Cox_Ross_Rubinstein:
                    engine = new BinomialVanillaEngine(bsmProcess, "coxrossrubinstein", (uint)timeSteps);
                    break;
                case BermudanEngineType.Binomial_Additive_Equiprobabilities:
                    engine = new BinomialVanillaEngine(bsmProcess, "eqp", (uint)timeSteps);
                    break;
                case BermudanEngineType.Binomial_Trigeorgis:
                    engine = new BinomialVanillaEngine(bsmProcess, "trigeorgis", (uint)timeSteps);
                    break;
                case BermudanEngineType.Binomial_Tian:
                    engine = new BinomialVanillaEngine(bsmProcess, "tain", (uint)timeSteps);
                    break;
                case BermudanEngineType.Binomial_Leisen_Reimer:
                    engine = new BinomialVanillaEngine(bsmProcess, "leisenreimer", (uint)timeSteps);
                    break;
                case BermudanEngineType.Binomial_Joshi:
                    engine = new BinomialVanillaEngine(bsmProcess, "joshi4", (uint)timeSteps);
                    break;
                case BermudanEngineType.FiniteDifference:
                    engine = new FDBermudanEngine(bsmProcess, (uint)timeSteps, (uint)timeSteps - 1);
                    break;
                default:
                    throw new ArgumentException("unknown engine type");
            }

            VanillaOption option = new VanillaOption(payoff, exercise);
            option.setPricingEngine(engine);

            object value = null;
            object delta = null;
            object gamma = null;
            object theta = null;
            object rho = null;
            object vega = null;

            try
            {
                value = option.NPV();
                delta = option.delta();
                gamma = option.gamma();
                theta = option.theta();
                rho = option.rho();
                vega = option.vega();
            }
            catch { }

            return new object[] { value, delta, gamma, theta, rho, vega };
        }

        #endregion Bermudan options










        #region Real-world options
        public static DataTable AmericanOptionRealWorld(string optionType, Date evalDate, Date maturity, double spot, double[] strikes, double[] vols, 
            double[] rates, double dividend, int dividendFrequency, Date exDivDate,  AmericanEngineType priceEngineType, int timeSteps)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            Calendar calendar = new UnitedStates(UnitedStates.Market.NYSE);
            Settings.instance().setEvaluationDate(evalDate);
            Date settlementDate = evalDate + 2;
            settlementDate = calendar.adjust(settlementDate);
            DayCounter dc = new ActualActual();

            // build yield term structure from Libor rates
            YieldTermStructureHandle yieldTS = new YieldTermStructureHandle(GetYieldCurve(settlementDate, 2, rates, calendar, dc));

            //build dividend term structure
            YieldTermStructureHandle dividendTS = new YieldTermStructureHandle(GetDividendCurve(evalDate, maturity, exDivDate, spot, dividend, dividendFrequency, calendar));

            //build vol term structure:
            BlackVolTermStructureHandle volTS = new BlackVolTermStructureHandle(GetVolCurve(evalDate, maturity, strikes, vols, calendar, dc));

            QuoteHandle spot1 = new QuoteHandle(new SimpleQuote(spot));
            Exercise exercise = new AmericanExercise(settlementDate, maturity);
            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(spot1, dividendTS, yieldTS, volTS);

            PricingEngine engine;
            switch (priceEngineType)
            {
                case AmericanEngineType.Barone_Adesi_Whaley:
                    engine = new BaroneAdesiWhaleyEngine(bsmProcess);
                    break;
                case AmericanEngineType.Bjerksund_Stensland:
                    engine = new BjerksundStenslandEngine(bsmProcess);
                    break;
                case AmericanEngineType.FiniteDifference:
                    engine = new FDAmericanEngine(bsmProcess, (uint)timeSteps, (uint)timeSteps - 1);
                    break;
                case AmericanEngineType.Binomial_Jarrow_Rudd:
                    engine = new BinomialVanillaEngine(bsmProcess, "jarrowrudd", (uint)timeSteps);
                    break;
                case AmericanEngineType.Binomial_Cox_Ross_Rubinstein:
                    engine = new BinomialVanillaEngine(bsmProcess, "coxrossrubinstein", (uint)timeSteps);
                    break;
                case AmericanEngineType.Binomial_Additive_Equiprobabilities:
                    engine = new BinomialVanillaEngine(bsmProcess, "eqp", (uint)timeSteps);
                    break;
                case AmericanEngineType.Binomial_Trigeorgis:
                    engine = new BinomialVanillaEngine(bsmProcess, "trigeorgis", (uint)timeSteps);
                    break;
                case AmericanEngineType.Binomial_Tian:
                    engine = new BinomialVanillaEngine(bsmProcess, "tian", (uint)timeSteps);
                    break;
                case AmericanEngineType.Binomial_Leisen_Reimer:
                    engine = new BinomialVanillaEngine(bsmProcess, "leisenreimer", (uint)timeSteps);
                    break;
                case AmericanEngineType.Binomial_Joshi:
                    engine = new BinomialVanillaEngine(bsmProcess, "joshi4", (uint)timeSteps);
                    break;
                default:
                    throw new ArgumentException("unknown engine type");
            }

            DataTable res = new DataTable();
            res.Columns.Add("Strike", typeof(double));
            res.Columns.Add("Price", typeof(double));
            res.Columns.Add("Delta", typeof(double));
            res.Columns.Add("Gamma", typeof(double));
            res.Columns.Add("Theta", typeof(double));
            res.Columns.Add("Rho", typeof(double));
            res.Columns.Add("Vega", typeof(double));
           
            for (int i = 0; i < strikes.Length; i++)
            {
                Payoff payoff = new PlainVanillaPayoff(otype, strikes[i]);
                VanillaOption option = new VanillaOption(payoff, exercise);
                option.setPricingEngine(engine);

                object value = null;
                object delta = null;
                object gamma = null;
                object theta = null;
                object rho = null;
                object vega = null;
                
                try
                {
                    value = option.NPV();
                    delta = option.delta();
                    gamma = option.gamma();
                    theta = option.theta();
                    rho = option.rho();
                    vega = option.vega();
                }
                catch { }

                res.Rows.Add(strikes[i], value, delta, gamma, theta, rho, vega);
            }
            return res;
        }

        private static BlackVolTermStructure GetVolCurve(Date evalDate, Date maturity, double[] strikes, double[] vols, Calendar calendar, DayCounter dc)
        {
            DoubleVector strikes1 = new DoubleVector();
            for (int i = 0; i < strikes.Length; i++)
                strikes1.Add(strikes[i]);

            DateVector expirations = new DateVector
            {
                maturity
            };

            Matrix volMatrix = new Matrix((uint)strikes1.Count, 1);

            for (int i = 0; i < vols.Length; i++)
            {
                volMatrix.set((uint)i, 0, vols[i]);
            }
            return new BlackVarianceSurface(evalDate, calendar, expirations, strikes1, volMatrix, dc);            
        }

        private static YieldTermStructure GetYieldCurve(Date settlementDate, uint fixingDays, double[] rates, Calendar calendar, DayCounter dc)
        {
            Period[] periods = new Period[]
            {
                new Period(1, TimeUnit.Days),
                new Period(1, TimeUnit.Weeks),
                new Period(1, TimeUnit.Months),
                new Period(2, TimeUnit.Months),
                new Period(3, TimeUnit.Months),
                new Period(6, TimeUnit.Months),
                new Period(12, TimeUnit.Months)
            };

            RateHelperVector liborRates = new RateHelperVector();
            for (int i = 0; i < rates.Length; i++)
                liborRates.Add(new DepositRateHelper(rates[i], periods[i], (uint)fixingDays, calendar, BusinessDayConvention.ModifiedFollowing, true, dc));

            return  new PiecewiseCubicZero(settlementDate, liborRates, dc);            
        }

        private static ZeroCurve GetDividendCurve(Date evalDate, Date maturity, Date exDivDate, double spot, double dividend, int dividendFrequency, Calendar calendar)
        {
            double annualDividend = dividend * 12.0 / dividendFrequency;
            Settings.instance().setEvaluationDate(evalDate);
            int settlementDays = 2;
            int dividendDiscountDays = settlementDays + calendar.businessDaysBetween(evalDate, maturity);
            double dividendYield = annualDividend / spot * dividendDiscountDays / 252;

            DateVector exDivDates = new DateVector();
            DoubleVector dividendYields = new DoubleVector();

            //Last ex div date and yield
            exDivDates.Add(calendar.advance(exDivDate, new Period(-dividendFrequency, TimeUnit.Months), BusinessDayConvention.ModifiedPreceding, true));
            dividendYields.Add(dividendYield);

            //Currently announced ex div date and yield
            exDivDates.Add(exDivDate);
            dividendYields.Add(dividendYield);

            //Next ex div date (projected) and yield
            Date projectedNextExDivDate = calendar.advance(exDivDate, new Period(dividendFrequency, TimeUnit.Months), BusinessDayConvention.ModifiedPreceding, true);
            exDivDates.Add(projectedNextExDivDate);
            dividendYields.Add(dividendYield);

            return new ZeroCurve(exDivDates, dividendYields, new ActualActual(), calendar);
        }
        
        #endregion Real-world options
    }
















    public enum BermudanEngineType
    {
        FiniteDifference,
        Binomial_Jarrow_Rudd,
        Binomial_Cox_Ross_Rubinstein,
        Binomial_Additive_Equiprobabilities,
        Binomial_Trigeorgis,
        Binomial_Tian,
        Binomial_Leisen_Reimer,
        Binomial_Joshi,
    };

    public enum AmericanEngineType
    {
        Barone_Adesi_Whaley,
        Bjerksund_Stensland,
        FiniteDifference,
        Binomial_Jarrow_Rudd,
        Binomial_Cox_Ross_Rubinstein,
        Binomial_Additive_Equiprobabilities,
        Binomial_Trigeorgis,
        Binomial_Tian,
        Binomial_Leisen_Reimer,
        Binomial_Joshi,
    };

    public enum EuropeanEngineType
    {
        Analytic,
        Binomiall_Jarrow_Rudd,
        Binomial_Cox_Ross_Rubinstein,
        Binomial_Additive_Equiprobabilities,
        Binomial_Trigeorgis,
        Binomial_Tian,
        Binomial_Leisen_Reimer,
        Binomial_Joshi,
        FiniteDifference,
        Integral,
        PseudoMonteCarlo,
        QuasiMonteCarlo
    };

    public enum BarrierEngineType
    {
        Analytic = 0,
        MonteCarlo = 1,
    };

}
