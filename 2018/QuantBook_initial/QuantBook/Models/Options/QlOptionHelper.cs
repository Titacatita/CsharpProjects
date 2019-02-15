using System;
using System.Collections.Generic;
using System.Linq;
using QLNet;

namespace QuantBook.Models.Options
{
    public static class QlOptionHelper
    {

        #region Bermudan Option
        public static object[] BermudanOption(string optionType, Date evalDate, double strike, double spot, double q, double r, double vol,
                     BermudanEngineType priceEngineType, int timeSteps, int samples)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            Calendar calendar = new TARGET();
            Date settleDate = evalDate + 2;
            Settings.setEvaluationDate(evalDate);

            DayCounter dc = new Actual365Fixed();
            List<Date> exerciseDates = new List<Date>();
            for (int i = 1; i <= 4; i++)
                exerciseDates.Add(settleDate + new Period(3 * i, TimeUnit.Months));

            Exercise exercise = new BermudanExercise(exerciseDates);

            // bootstrap the yield/dividend/vol curves
            Handle<Quote> underlyingH = new Handle<Quote>(new SimpleQuote(spot));
            Handle<YieldTermStructure> flatTermStructure = new Handle<YieldTermStructure>(new FlatForward(settleDate, r, dc));
            Handle<YieldTermStructure> flatDividendTS = new Handle<YieldTermStructure>(new FlatForward(settleDate, q, dc));
            Handle<BlackVolTermStructure> flatVolTS = new Handle<BlackVolTermStructure>(new BlackConstantVol(settleDate, calendar, vol, dc));
            StrikedTypePayoff payoff = new PlainVanillaPayoff(otype, strike);
            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(underlyingH, flatDividendTS, flatTermStructure, flatVolTS);
            VanillaOption option = new VanillaOption(payoff, exercise);

            
            IPricingEngine engine;
            switch (priceEngineType)
            {
                case BermudanEngineType.Binomial_Jarrow_Rudd:
                    engine = new BinomialVanillaEngine<JarrowRudd>(bsmProcess, timeSteps);
                    break;
                case BermudanEngineType.Binomial_Cox_Ross_Rubinstein:
                    engine = new BinomialVanillaEngine<CoxRossRubinstein>(bsmProcess, timeSteps);
                    break;
                case BermudanEngineType.Binomial_Additive_Equiprobabilities:
                    engine = new BinomialVanillaEngine<AdditiveEQPBinomialTree>(bsmProcess, timeSteps);
                    break;
                case BermudanEngineType.Binomial_Trigeorgis:
                    engine = new BinomialVanillaEngine<Trigeorgis>(bsmProcess, timeSteps);
                    break;
                case BermudanEngineType.Binomial_Tian:
                    engine = new BinomialVanillaEngine<Tian>(bsmProcess, timeSteps);
                    break;
                case BermudanEngineType.Binomial_Leisen_Reimer:
                    engine = new BinomialVanillaEngine<LeisenReimer>(bsmProcess, timeSteps);
                    break;
                case BermudanEngineType.Binomial_Joshi:
                    engine = new BinomialVanillaEngine<Joshi4>(bsmProcess, timeSteps);
                    break;
                case BermudanEngineType.FiniteDifference:
                    engine = new FDEuropeanEngine(bsmProcess, timeSteps, samples);
                    break;
                default:
                    throw new ArgumentException("unknown engine type");

            }

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
        #endregion Bermudan Option







        #region European Option
        public static object BarrierOption(string optionType, Date evalDate, Date maturity, double strike, double spot, double barrierLevel, double rebate,
            double q, double r, double vol, Barrier.Type barrierType)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            Calendar calendar = new TARGET();
            Date settleDate = evalDate + 2;
            Settings.setEvaluationDate(evalDate);
            DayCounter dc = new Actual365Fixed();

            Exercise exercise = new EuropeanExercise(maturity);

            // bootstrap the yield/dividend/vol curves
            Handle<Quote> underlyingH = new Handle<Quote>(new SimpleQuote(spot));
            Handle<YieldTermStructure> flatTermStructure = new Handle<YieldTermStructure>(new FlatForward(settleDate, r, dc));
            Handle<YieldTermStructure> flatDividendTS = new Handle<YieldTermStructure>(new FlatForward(settleDate, q, dc));
            Handle<BlackVolTermStructure> flatVolTS = new Handle<BlackVolTermStructure>(new BlackConstantVol(settleDate, calendar, vol, dc));
            StrikedTypePayoff payoff = new PlainVanillaPayoff(otype, strike);

            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(underlyingH, flatDividendTS, flatTermStructure, flatVolTS);
            BarrierOption option = new BarrierOption(barrierType, barrierLevel, rebate, payoff, exercise);
            IPricingEngine engine = new AnalyticBarrierEngine(bsmProcess);
            
            option.setPricingEngine(engine);

            object v = null;
            try
            {
                v = option.NPV();
            }
            catch { }

            return v;
        }

        public static object BarrierOption(string optionType, Date evalDate, double yearsToMaturity, double strike,
                     double spot, double barrierLevel, double rebate, double q, double r, double vol,
                      Barrier.Type barrierType)
        {
            Date maturity = evalDate + 2 + Convert.ToInt32(yearsToMaturity * 365 + 0.5);
            return BarrierOption(optionType, evalDate, maturity, strike, spot, barrierLevel, rebate, q, r, vol, barrierType);
        }



        public static object[] EuropeanOption(string optionType, Date startDate, double yearsToMaturity, double strike, double spot, double q, double r,
            double vol, EuropeanEngineType priceEngineType, int timeSteps, int samples)
        {
            Date maturity = startDate + Convert.ToInt32(yearsToMaturity * 360 + 0.5);
            return EuropeanOption(optionType, startDate, maturity, strike, spot, q, r, vol, priceEngineType, timeSteps, samples);
        }

        public static object[] EuropeanOption(string optionType, Date startDate, Date maturity, double strike, double spot, double q, double r,
           double vol, EuropeanEngineType priceEngineType, int timeSteps, int samples)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            DayCounter dc = new Actual360();
            SimpleQuote spot1 = new SimpleQuote(0.0);
            SimpleQuote qRate = new SimpleQuote(0.0);
            YieldTermStructure qTS = Utilities.flatRate(startDate, qRate, dc);

            SimpleQuote rRate = new SimpleQuote(0.0);
            YieldTermStructure rTS = Utilities.flatRate(startDate, rRate, dc);
            SimpleQuote vol1 = new SimpleQuote(0.0);
            BlackVolTermStructure volTS = Utilities.flatVol(startDate, vol1, dc);

            StrikedTypePayoff payoff = new PlainVanillaPayoff(otype, strike);
            Exercise exercise = new EuropeanExercise(maturity);
            spot1.setValue(spot);
            qRate.setValue(q);
            rRate.setValue(r);
            vol1.setValue(vol);

            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(new Handle<Quote>(spot1),
                                              new Handle<YieldTermStructure>(qTS),
                                              new Handle<YieldTermStructure>(rTS),
                                              new Handle<BlackVolTermStructure>(volTS));

            IPricingEngine engine;
            switch (priceEngineType)
            {
                case EuropeanEngineType.Analytic:
                    engine = new AnalyticEuropeanEngine(bsmProcess);
                    break;
                case EuropeanEngineType.Binomiall_Jarrow_Rudd:
                    engine = new BinomialVanillaEngine<JarrowRudd>(bsmProcess, timeSteps);
                    break;
                case EuropeanEngineType.Binomial_Cox_Ross_Rubinstein:
                    engine = new BinomialVanillaEngine<CoxRossRubinstein>(bsmProcess, timeSteps);
                    break;
                case EuropeanEngineType.Binomial_Additive_Equiprobabilities:
                    engine = new BinomialVanillaEngine<AdditiveEQPBinomialTree>(bsmProcess, timeSteps);
                    break;
                case EuropeanEngineType.Binomial_Trigeorgis:
                    engine = new BinomialVanillaEngine<Trigeorgis>(bsmProcess, timeSteps);
                    break;
                case EuropeanEngineType.Binomial_Tian:
                    engine = new BinomialVanillaEngine<Tian>(bsmProcess, timeSteps);
                    break;
                case EuropeanEngineType.Binomial_Leisen_Reimer:
                    engine = new BinomialVanillaEngine<LeisenReimer>(bsmProcess, timeSteps);
                    break;
                case EuropeanEngineType.Binomial_Joshi:
                    engine = new BinomialVanillaEngine<Joshi4>(bsmProcess, timeSteps);
                    break;
                case EuropeanEngineType.FiniteDifference:
                    engine = new FDEuropeanEngine(bsmProcess, timeSteps, samples);
                    break;
                case EuropeanEngineType.Integral:
                    engine = new IntegralEngine(bsmProcess);
                    break;
                case EuropeanEngineType.PseudoMonteCarlo:
                    engine = new MakeMCEuropeanEngine<PseudoRandom>(bsmProcess)
                                      .withSteps(timeSteps)
                                      .withAbsoluteTolerance(0.02)
                                      .withSeed((ulong)samples)
                                      .value();
                    break;
                case EuropeanEngineType.QuasiMonteCarlo:
                    engine = new MakeMCEuropeanEngine<LowDiscrepancy>(bsmProcess)
                                            .withSteps(timeSteps)
                                            .withSamples(samples)
                                            .value();
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

        public static object EuropeanOptionImpliedVol(string optionType, Date startDate, double yearsToMaturity, double strike, double spot, double q, double r, double targetPrice)
        {
            Date maturity = startDate + Convert.ToInt32(yearsToMaturity * 360 + 0.5);
            return EuropeanOptionImpliedVol(optionType, startDate, maturity, strike, spot, q, r, targetPrice);
        }


        public static object EuropeanOptionImpliedVol(string optionType, Date startDate, Date maturity, double strike, double spot, double q, double r, double targetPrice)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            DayCounter dc = new Actual360();
            SimpleQuote spot1 = new SimpleQuote(0.0);
            SimpleQuote qRate = new SimpleQuote(0.0);
            YieldTermStructure qTS = Utilities.flatRate(startDate, qRate, dc);

            SimpleQuote rRate = new SimpleQuote(0.0);
            YieldTermStructure rTS = Utilities.flatRate(startDate, rRate, dc);
            SimpleQuote vol1 = new SimpleQuote(0.0);
            BlackVolTermStructure volTS = Utilities.flatVol(startDate, vol1, dc);

            StrikedTypePayoff payoff = new PlainVanillaPayoff(otype, strike);
            Exercise exercise = new EuropeanExercise(maturity);
            spot1.setValue(spot);
            qRate.setValue(q);
            rRate.setValue(r);
            vol1.setValue(0.3);

            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(new Handle<Quote>(spot1),
                                              new Handle<YieldTermStructure>(qTS),
                                              new Handle<YieldTermStructure>(rTS),
                                              new Handle<BlackVolTermStructure>(volTS));
            VanillaOption option = new VanillaOption(payoff, exercise);

            object v = null;
            try
            {
                v = option.impliedVolatility(targetPrice, bsmProcess, 1.0e-6, 10000, 0, 4.0);
            }
            catch { }            
            
            return v;
        }
        #endregion European Option















        #region American Option
        public static object[] AmericanOption(string optionType, Date startDate, double yearsToMaturity, double strike, double spot, double q, double r,
            double vol, AmericanEngineType priceEngineType, int timeSteps, int samples)
        {
            Date maturity = startDate + Convert.ToInt32(yearsToMaturity * 360 + 0.5);
            return AmericanOption(optionType, startDate, maturity, strike, spot, q, r, vol, priceEngineType, timeSteps, samples);
        }



        public static object[] AmericanOption(string optionType, Date startDate, Date maturity, double strike, double spot, double q, double r, double vol,
            AmericanEngineType priceEngineType, int timeSteps, int samples)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            DayCounter dc = new Actual360();
            SimpleQuote spot1 = new SimpleQuote(0.0);
            SimpleQuote qRate = new SimpleQuote(0.0);
            YieldTermStructure qTS = Utilities.flatRate(startDate, qRate, dc);

            SimpleQuote rRate = new SimpleQuote(0.0);
            YieldTermStructure rTS = Utilities.flatRate(startDate, rRate, dc);
            SimpleQuote vol1 = new SimpleQuote(0.0);
            BlackVolTermStructure volTS = Utilities.flatVol(startDate, vol1, dc);

            StrikedTypePayoff payoff = new PlainVanillaPayoff(otype, strike);
            Exercise exercise = new AmericanExercise(startDate, maturity);
            spot1.setValue(spot);
            qRate.setValue(q);
            rRate.setValue(r);
            vol1.setValue(vol);

            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(new Handle<Quote>(spot1),
                                              new Handle<YieldTermStructure>(qTS),
                                              new Handle<YieldTermStructure>(rTS),
                                              new Handle<BlackVolTermStructure>(volTS));

            IPricingEngine engine;
            switch (priceEngineType)
            {
                /*case AmericanEngineType.Ju_Quadratic:
                    engine = new JuQuadraticApproximationEngine(bsmProcess);
                    break;*/
                case AmericanEngineType.Barone_Adesi_Whaley:
                    engine = new BaroneAdesiWhaleyApproximationEngine(bsmProcess);
                    break;
                case AmericanEngineType.Bjerksund_Stensland:
                    engine = new BjerksundStenslandApproximationEngine(bsmProcess);
                    break;
                case AmericanEngineType.FiniteDifference:
                    engine = new FDAmericanEngine(bsmProcess, timeSteps, samples);
                    break;
                case AmericanEngineType.Binomial_Jarrow_Rudd:
                    engine = new BinomialVanillaEngine<JarrowRudd>(bsmProcess, timeSteps);
                    break;
                case AmericanEngineType.Binomial_Cox_Ross_Rubinstein:
                    engine = new BinomialVanillaEngine<CoxRossRubinstein>(bsmProcess, timeSteps);
                    break;
                case AmericanEngineType.Binomial_Additive_Equiprobabilities:
                    engine = new BinomialVanillaEngine<AdditiveEQPBinomialTree>(bsmProcess, timeSteps);
                    break;
                case AmericanEngineType.Binomial_Trigeorgis:
                    engine = new BinomialVanillaEngine<Trigeorgis>(bsmProcess, timeSteps);
                    break;
                case AmericanEngineType.Binomial_Tian:
                    engine = new BinomialVanillaEngine<Tian>(bsmProcess, timeSteps);
                    break;
                case AmericanEngineType.Binomial_Leisen_Reimer:
                    engine = new BinomialVanillaEngine<LeisenReimer>(bsmProcess, timeSteps);
                    break;
                case AmericanEngineType.Binomial_Joshi:
                    engine = new BinomialVanillaEngine<Joshi4>(bsmProcess, timeSteps);
                    break;
                /*case AmericanEngineType.MonteCarlo_Longstaff_Schwartz:
                    engine = new MakeMCAmericanEngine<PseudoRandom>(bsmProcess)
                            .withSteps(timeSteps)
                            .withAntitheticVariate()
                            .withCalibrationSamples(4096)
                            .withAbsoluteTolerance(0.02)
                            .withSeed((ulong)samples)
                            .value();
                    break;*/
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

        

        public static object AmericanOptionImpliedVol(string optionType, Date startDate, double yearsToMaturity, double strike, double spot, double q, double r, double targetPrice)
        {
            Date maturity = startDate + Convert.ToInt32(yearsToMaturity * 360 + 0.5);
            return AmericanOptionImpliedVol(optionType, startDate, maturity, strike, spot, q, r, targetPrice);
        }
        
        public static object AmericanOptionImpliedVol(string optionType, Date startDate, Date maturity, double strike, double spot, double q, double r, double targetPrice)
        {
            optionType = optionType.ToUpper();
            Option.Type otype = Option.Type.Call;
            if (optionType == "P" || optionType == "PUT")
                otype = Option.Type.Put;

            DayCounter dc = new Actual360();
            SimpleQuote spot1 = new SimpleQuote(0.0);
            SimpleQuote qRate = new SimpleQuote(0.0);
            YieldTermStructure qTS = Utilities.flatRate(startDate, qRate, dc);

            SimpleQuote rRate = new SimpleQuote(0.0);
            YieldTermStructure rTS = Utilities.flatRate(startDate, rRate, dc);
            SimpleQuote vol1 = new SimpleQuote(0.0);
            BlackVolTermStructure volTS = Utilities.flatVol(startDate, vol1, dc);

            StrikedTypePayoff payoff = new PlainVanillaPayoff(otype, strike);
            Exercise exercise = new AmericanExercise(startDate, maturity);
            spot1.setValue(spot);
            qRate.setValue(q);
            rRate.setValue(r);
            vol1.setValue(0.3);            

            BlackScholesMertonProcess bsmProcess = new BlackScholesMertonProcess(new Handle<Quote>(spot1),
                                              new Handle<YieldTermStructure>(qTS),
                                              new Handle<YieldTermStructure>(rTS),
                                              new Handle<BlackVolTermStructure>(volTS));
            VanillaOption option = new VanillaOption(payoff, exercise);

            object v = null;
            try
            {
                v = option.impliedVolatility(targetPrice, bsmProcess, 1.0e-6, 10000, 0, 4.0);
            }
            catch { }

            return v;
        }

        #endregion American Option



    }


     public enum BondEngineType
    {
        DiscountingBond,
        BackCallableFxiedRateBond,
        BlackCallableZeroCouponBond,
        TreeCallableFixedRateBond,        
    }





    public class Flag : IObserver
    {
        private bool up_;

        public Flag()
        {
            up_ = false;
        }

        public void raise() { up_ = true; }
        public void lower() { up_ = false; }
        public bool isUp() { return up_; }
        public void update() { raise(); }
    };



    public static class Utilities
    {
        public static YieldTermStructure flatRate(Date today, double forward, DayCounter dc)
        {
            return new FlatForward(today, new SimpleQuote(forward), dc);
        }
        public static YieldTermStructure flatRate(Date today, Quote forward, DayCounter dc)
        {
            return new FlatForward(today, forward, dc);
        }

        public static BlackVolTermStructure flatVol(Date today, double vol, DayCounter dc)
        {
            return flatVol(today, new SimpleQuote(vol), dc);
        }

        public static BlackVolTermStructure flatVol(Date today, Quote vol, DayCounter dc)
        {
            return new BlackConstantVol(today, new NullCalendar(), new Handle<Quote>(vol), dc);
        }

        public static double norm(Vector v, int size, double h)
        {
            // squared values
            List<double> f2 = new InitializedList<double>(size);

            for (int i = 0; i < v.Count; i++)
                f2[i] = v[i] * v[i];

            // numeric integral of f^2
            double I = h * (f2.Sum() - 0.5 * f2.First() - 0.5 * f2.Last());
            return Math.Sqrt(I);
        }

        public static double relativeError(double x1, double x2, double reference)
        {
            if (reference != 0.0)
                return Math.Abs(x1 - x2) / reference;
            else
                // fall back to absolute error
                return Math.Abs(x1 - x2);
        }
    }

    // this cleans up index-fixing histories when destroyed
    public class IndexHistoryCleaner
    {
        ~IndexHistoryCleaner() { IndexManager.instance().clearHistories(); }
    };

}
