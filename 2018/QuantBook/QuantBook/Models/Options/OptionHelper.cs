using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuantBook.Models.Options
{
    public static class OptionHelper
    {
        private const double ONEOVERSQRT2PI = 0.39894228;
        private const double PI = 3.1415926;

        public static double[] black_scholes(double t, double s0, double x, double sigma, double r, double q, double put)
        {
            double one = 1.0, two = 2.0;
            double d1, d2, temp, temp1, np;
            double EPS = 1.0e-16;
            double PI = 3.14159;

            if (x < EPS || sigma < EPS)
            {
                return null;
            }


            temp = Math.Log(s0 / x);
            d1 = temp + (r - q + (sigma * sigma / two)) * t;
            d1 = d1 / (sigma * Math.Sqrt(t));
            d2 = d1 - sigma * Math.Sqrt(t);

            // Evaluate the option price:
            double value = 0;
            double[] greeks = new double[6];

            // The hedge statistics output as follows: greaks[0] is value, greeks[1] is gamma, 
            // greeks[2] is delta, greeks[3] is theta, greeks[4] is rho, and greeks[5] is vega

            if (put == 0)
            {
                value = s0 * Math.Exp(-q * t) * CumNorm(d1) - x * Math.Exp(-r * t) * CumNorm(d2);
            }
            else
            {
                value = (-s0 * Math.Exp(-q * t) * CumNorm(-d1) +
                    x * Math.Exp(-r * t) * CumNorm(-d2));
            }

            greeks[0] = value;

            // Calculate greeks:
            temp1 = -d1 * d1 / two;
            d2 = d1 - sigma * Math.Sqrt(t);
            np = (one / Math.Sqrt(two * PI)) * Math.Exp(temp1);

            if (put == 0)
            {
                // a call option
                greeks[2] = (CumNorm(d1)) * Math.Exp(-q * t);  // delta 

                greeks[3] = -s0 * Math.Exp(-q * t) * np * sigma / (two * Math.Sqrt(t))
                    + q * s0 * CumNorm(d1) * Math.Exp(-q * t) -
                    r * x * Math.Exp(-r * t) * CumNorm(d2); // theta 

                greeks[4] = x * t * Math.Exp(-r * t) * CumNorm(d2); // rho
            }
            else
            {
                // a put option
                greeks[2] = (CumNorm(d1) - one) * Math.Exp(-q * t); // delta 
                greeks[3] = -s0 * Math.Exp(-q * t) * np * sigma / (two * Math.Sqrt(t)) -
                    q * s0 * CumNorm(-d1) * Math.Exp(-q * t) +
                    r * x * Math.Exp(-r * t) * CumNorm(-d2); // theta
                greeks[4] = -x * t * Math.Exp(-r * t) * CumNorm(-d2); // rho
            }
            greeks[1] = np * Math.Exp(-q * t) / (s0 * sigma * Math.Sqrt(t)); // gamma
            greeks[5] = s0 * Math.Sqrt(t) * np * Math.Exp(-q * t); // vega

            return greeks;
        }


        #region Black-Scholes
        public static double BlackScholes(string optionType, double spot, double strike, double rate, double carry, double maturity, double vol)
        {
            double d1, d2, option;
            double sqrtMaturity = Math.Sqrt(maturity);

            d1 = Math.Log(spot / strike) + (carry + vol * vol * 0.5) * maturity;
            d1 /= vol * sqrtMaturity;
            d2 = d1 - vol * sqrtMaturity;

            if (optionType.ToUpper() == "P" || optionType.ToUpper() == "PUT")
                option = strike * CumNorm(-d2) * Math.Exp(-rate * maturity) - spot * CumNorm(-d1) * Math.Exp((carry - rate) * maturity);
            else
                option = spot * CumNorm(d1) * Math.Exp((carry - rate) * maturity) - strike * CumNorm(d2) * Math.Exp(-rate * maturity);

            return option;
        }


        public static double BlackScholes_Delta(string optionType, double spot, double strike, double rate, double carry, double maturity, double vol)
        {
            double d1, d2, option;
            double sqrtMaturity = Math.Sqrt(maturity);

            d1 = Math.Log(spot / strike) + (carry + vol * vol * 0.5) * maturity;
            d1 /= vol * sqrtMaturity;

            d2 = d1 - vol * sqrtMaturity;

            if (optionType.ToUpper() == "P" || optionType.ToUpper() == "PUT")
                option = (CumNorm(d1) - 1.0) * Math.Exp((carry - rate) * maturity);
            else
                option = CumNorm(d1) * Math.Exp((carry - rate) * maturity);

            return option;
        }


        public static double BlackScholes_Gamma(double spot, double strike, double rate, double carry, double maturity, double vol)
        {
            double d1, option;
            double sqrtMaturity = Math.Sqrt(maturity);

            d1 = Math.Log(spot / strike) + (carry + vol * vol * 0.5) * maturity;
            d1 /= vol * sqrtMaturity;

            option = NormDensity(d1) * Math.Exp((carry - rate) * maturity) / (spot * vol * sqrtMaturity);

            return option;
        }


        public static double BlackScholes_Vega(double spot, double strike, double rate, double carry, double maturity, double vol)
        {
            double d1, option;
            double sqrtMaturity = Math.Sqrt(maturity);

            d1 = Math.Log(spot / strike) + (carry + vol * vol * 0.5) * maturity;
            d1 /= vol * sqrtMaturity;

            option = spot * NormDensity(d1) * Math.Exp((carry - rate) * maturity) * sqrtMaturity;

            return option;
        }

        public static double BlackScholes_Theta(string optionType, double spot, double strike, double rate, double carry, double maturity, double vol)
        {
            double d1, d2, option;
            double sqrtMaturity = Math.Sqrt(maturity);

            d1 = Math.Log(spot / strike) + (carry + vol * vol * 0.5) * maturity;
            d1 /= vol * sqrtMaturity;
            d2 = d1 - vol * sqrtMaturity;

            if (optionType.ToUpper() == "P" || optionType.ToUpper() == "PUT")
            {
                double p1 = spot * Math.Exp((carry - rate) * maturity) * NormDensity(d1) * vol * 0.5 / sqrtMaturity;
                double p2 = (carry - rate) * spot * Math.Exp((carry - rate) * maturity) * CumNorm(-d1);
                double p3 = rate * strike * Math.Exp(-rate * maturity) * CumNorm(-d2);
                option = p2 + p3 - p1;
            }
            else
            {
                double c1 = spot * Math.Exp((carry - rate) * maturity) * NormDensity(d1) * vol * 0.5 / sqrtMaturity;
                double c2 = (carry - rate) * spot * Math.Exp((carry - rate) * maturity) * CumNorm(d1);
                double c3 = rate * strike * Math.Exp(-rate * maturity) * CumNorm(d2);
                option = -c1 - c2 - c3;
            }

            return option;
        }

        public static double BlackScholes_Rho(string optionType, double spot, double strike, double rate, double carry, double maturity, double vol)
        {
            double d1, d2, option;
            double sqrtMaturity = Math.Sqrt(maturity);

            d1 = Math.Log(spot / strike) + (carry + vol * vol * 0.5) * maturity;
            d1 /= vol * sqrtMaturity;
            d2 = d1 - vol * sqrtMaturity;

            if (optionType.ToUpper() == "P" || optionType.ToUpper() == "PUT")
            {
                if (carry != 0)
                    option = -maturity * strike * Math.Exp(-rate * maturity) * CumNorm(-d2);
                else
                    option = -maturity * BlackScholes("P", spot, strike, rate, 0, maturity, vol);
            }
            else
            {
                if (carry != 0)
                    option = maturity * strike * Math.Exp(-rate * maturity) * CumNorm(d2);
                else
                    option = -maturity * BlackScholes("C", spot, strike, rate, 0, maturity, vol);
            }

            return option;
        }

        public static double BlackScholes_ImpliedVol(string optionType, double spot, double strike, double rate, double carry, double maturity, double price)
        {
            double low = 0.0;
            double high = 4.0;
            double sqrtMaturity = Math.Sqrt(maturity);
            double vol;

            if (BlackScholes(optionType, spot, strike, rate, carry, maturity, high) < price)
                vol = high;
            else if (BlackScholes(optionType, spot, strike, rate, carry, maturity, low) > price)
                vol = low;
            else
            {
                vol = (high + low) * 0.5;
                int count = 0;
                while (vol - low > 0.0001 && count < 100000)
                {
                    if (BlackScholes(optionType, spot, strike, rate, carry, maturity, vol) < price)
                        low = vol;
                    else if (BlackScholes(optionType, spot, strike, rate, carry, maturity, vol) > price)
                        high = vol;
                    vol = (high + low) * 0.5;
                    count++;
                }
            }
            return vol;
        }


        private static double CumNorm(double x)
        {
            if (x < 0)
                return 1.0 - CumNorm(-x);
            else
            {
                double k = 1.0 / (1.0 + 0.2316419 * x);
                return 1.0 - ONEOVERSQRT2PI * Math.Exp(-0.5 * x * x) * ((((1.330274429 * k - 1.821255978) * k + 1.781477937) * k - 0.356563782) * k + 0.319381530) * k;
            }
        }

        private static double NormDensity(double x)
        {
            return Math.Exp(-x * x * 0.5) / Math.Sqrt(2.0 * PI);
        }

        #endregion Black-Scholes













        #region Barrier Options
        public static object BarrierOptions(string optionType, string barrierType, double spot, double strike, double rate, double divYield,
           double maturity, double vol, double barrierLevel, double rebate)
        {
            optionType = optionType.ToUpper();
            object res = null;
            switch (optionType)
            {
                case "CALL":
                case "C":
                    res = BarrierOptionsCall(barrierType, spot, strike, rate, divYield, maturity, vol, barrierLevel, rebate);
                    break;
                case "PUT":
                case "P":
                    res = BarrierOptionsPut(barrierType, spot, strike, rate, divYield, maturity, vol, barrierLevel, rebate);
                    break;
            }
            return res;
        }

         public static object BarrierOptionsCall(string barrierType, double spot, double strike, double rate, double divYield,
            double maturity, double vol, double barrierLevel, double rebate)
        {
            object res = null;
            switch (barrierType)
            {
                case "DownIn":
                    if (spot > barrierLevel)
                    {
                        if (strike >= barrierLevel)
                        {
                            res = C(1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                  E(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                        }
                        else
                        {
                            res = A(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                                  B(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                  D(1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                  E(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                        }
                    }
                    break;

                case "UpIn":
                    if (spot < barrierLevel)
                    {
                        if (strike >= barrierLevel)
                        {
                            res = A(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                  E(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                        }
                        else
                        {
                            res = B(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                                  C(-1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                  D(-1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                  E(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                        }
                    }
                    break;

                case "DownOut":
                    if (spot > barrierLevel)
                    {
                        if (strike > barrierLevel)
                        {
                            res = A(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                                  C(1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                  F(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                        }
                        else
                        {
                            res = B(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                                  D(1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                  F(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                        }
                    }
                    break;

                case "UpOut":

                    if (spot < barrierLevel)
                    {
                        if (strike > barrierLevel)
                        {
                            res = F(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                        }
                        else
                        {
                            res = A(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                                  B(1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                  C(-1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                                  D(-1, 1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                  F(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                        }
                    }
                    break;
            }
            return res;
        }

         public static object BarrierOptionsPut(string barrierType, double spot, double strike, double rate, double divYield,
            double maturity, double vol, double barrierLevel, double rebate)
         {
             object res = null;
             switch (barrierType)
             {
                 case "DownIn":
                     if (spot > barrierLevel)
                     {
                         if (strike >= barrierLevel)
                         {
                             res = B(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                                   C(1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                   D(1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                   E(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                         }
                         else
                         {
                             res = A(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                                   E(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                         }
                     }
                     break;

                 case "UpIn":
                     if (spot < barrierLevel)
                     {
                         if (strike >= barrierLevel)
                         {
                             res = A(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                   B(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                   D(-1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                   E(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                         }
                         else
                         {
                             res = C(-1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                   E(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                         }
                     }
                     break;

                 case "DownOut":
                     if (spot > barrierLevel)
                     {
                         if (strike > barrierLevel)
                         {
                             res = A(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                                   B(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                   C(1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                                   D(1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                   F(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                         }
                         else
                         {
                             res = F(1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                         }
                     }
                     break;

                 case "UpOut":
                     if (spot < barrierLevel)
                     {
                         if (strike > barrierLevel)
                         {
                             res = B(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                                   D(-1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                   F(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                         }
                         else
                         {
                             res = A(-1, barrierLevel, spot, strike, vol, maturity, rate, divYield) -
                                   C(-1, -1, barrierLevel, spot, strike, vol, maturity, rate, divYield) +
                                   F(-1, rebate, barrierLevel, spot, strike, vol, maturity, rate, divYield);
                         }
                     }
                     break;
             }
             return res;
         }




        private static double A(double phi, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
        {
            double fac = vol * Math.Sqrt(maturity);
            double m = (rate - divYield - 0.5 * vol * vol) / vol / vol;
            double x1 = Math.Log(spot / strike) / fac + (1.0 + m) * fac;
            return phi * spot * Math.Exp(-divYield * maturity) * CumNorm(phi * x1) - phi * strike * Math.Exp(-rate * maturity) * CumNorm(phi * x1 - phi * fac);
        }

        private static double B(double phi, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
        {
            double fac = vol * Math.Sqrt(maturity);
            double m = (rate - divYield - 0.5 * vol * vol) / vol / vol;
            double x2 = Math.Log(spot / barrierLevel) / fac + (1.0 + m) * fac;
            return phi * spot * Math.Exp(-divYield * maturity) * CumNorm(phi * x2) - phi * strike * Math.Exp(-rate * maturity) * CumNorm(phi * x2 - phi * fac);
        }

        private static double C(double phi, double eta, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
        {
            double hs = barrierLevel / spot;
            double fac = vol * Math.Sqrt(maturity);
            double m = (rate - divYield - 0.5 * vol * vol) / vol / vol;
            double powhs0 = Math.Pow(hs, 2 * m);
            double powhs1 = powhs0 * hs * hs;
            double y1 = Math.Log(barrierLevel * barrierLevel / strike / spot) / fac + (1.0 + m) * fac;
            return phi * spot * Math.Exp(-divYield * maturity) * powhs1 * CumNorm(eta * y1) -
                phi * strike * Math.Exp(-rate * maturity) * powhs0 * CumNorm(eta * y1 - eta * fac);
        }

        private static double D(double phi, double eta, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
        {
            double hs = barrierLevel / spot;
            double fac = vol * Math.Sqrt(maturity);
            double m = (rate - divYield - 0.5 * vol * vol) / vol / vol;
            double powhs0 = Math.Pow(hs, 2 * m);
            double powhs1 = powhs0 * hs * hs;
            double y2 = Math.Log(barrierLevel / spot) / fac + (1.0 + m) * fac;
            return phi * spot * Math.Exp(-divYield * maturity) * powhs1 * CumNorm(eta * y2) -
                phi * strike * Math.Exp(-rate * maturity) * powhs0 * CumNorm(eta * y2 - eta * fac);
        }

        private static double E(double eta, double rebate, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
        {
            if (rebate > 0)
            {
                double hs = barrierLevel / spot;
                double fac = vol * Math.Sqrt(maturity);
                double m = (rate - divYield - 0.5 * vol * vol) / vol / vol;
                double powhs0 = Math.Pow(hs, 2 * m);
                double x2 = Math.Log(spot / barrierLevel) / fac + (1.0 + m) * fac;
                double y2 = Math.Log(barrierLevel / spot) / fac + (1.0 + m) * fac;
                return rebate * Math.Exp(-rate * maturity) * (CumNorm(eta * y2 - eta * fac) - powhs0 * CumNorm(eta * y2 - eta * fac));
            }
            else
                return 0.0;
        }

        private static double F(double eta, double rebate, double barrierLevel, double spot, double strike, double vol, double maturity, double rate, double divYield)
        {
            if (rebate > 0)
            {
                double hs = barrierLevel / spot;
                double fac = vol * Math.Sqrt(maturity);
                double m = (rate - divYield - 0.5 * vol * vol) / vol / vol;
                double lambda = Math.Sqrt(m * m + 2.0 * (rate - divYield) / vol / vol);
                double powHSplus = Math.Pow(hs, m + lambda);
                double powHSminus = Math.Pow(hs, m - lambda);
                double z = Math.Log(barrierLevel / spot) / fac + lambda * fac;
                return rebate * (powHSplus * CumNorm(eta * z) - powHSminus * CumNorm(eta * z - 2.0 * eta * lambda * fac));
            }
            else
                return 0.0;
        }



        #endregion Barrier Options





















        #region American Options
        public static double American_BaroneAdesiWhaley(string optionType, double spot, double strike, double rate, double divYield, double maturity, double vol)
        {
            double carry = rate - divYield;
            optionType = optionType.ToUpper();

            if (optionType == "P" || optionType == "PUT")
                return AmericanPut_BaroneAdesiWhaley(spot, strike, rate, carry, maturity, vol);
            else
                return AmericanCall_BaroneAdesiWhaley(spot, strike, rate, carry, maturity, vol);
        }


        private static double AmericanCall_BaroneAdesiWhaley(double spot, double strike, double rate, double carry, double maturity, double vol)
        {
            double option = 0;
            double sqrtMaturity = Math.Sqrt(maturity);

            if (carry >= rate)
                option = BlackScholes("C", spot, strike, rate, carry, maturity, vol);
            else
            {
                double sk = AmericanCall_NewtonRaphson(strike, rate, carry, maturity, vol);
                double n = 2.0 * carry / vol / vol;
                double m = 2.0 * rate / vol / vol;
                //double k = 2.0 * rate / (vol * vol * (1.0 - Math.Exp(-rate * maturity)));
                double k = 1.0 - Math.Exp(-rate * maturity);
                double d1 = Math.Log(sk / strike) + (carry + vol * vol * 0.5) * maturity;
                d1 /= vol * sqrtMaturity;
                double q2 = (Math.Sqrt((n - 1.0) * (n - 1.0) + 4.0 * m / k) - (n - 1.0)) * 0.5;
                double a2 = (sk / q2) * (1.0 - Math.Exp((carry - rate) * maturity) * CumNorm(d1));
                if (spot < sk)
                    option = BlackScholes("C", spot, strike, rate, carry, maturity, vol) + a2 * Math.Pow(spot / sk, q2);
                else
                    option = spot - strike;
            }

            return option;
        }

        private static double AmericanCall_NewtonRaphson(double strike, double rate, double carry, double maturity, double vol)
        {
            double option = 0;
            double sqrtMaturity = Math.Sqrt(maturity);

            double n = 2.0 * carry / vol / vol;
            double m = 2.0 * rate / vol / vol;
            double q2u = 0.5 * (Math.Sqrt((n - 1.0) * (n - 1.0) + 4.0 * m) - (n - 1.0));
            double su = strike / (1.0 - 1.0 / q2u);
            double h2 = (carry * maturity + 2.0 * vol * Math.Sqrt(maturity)) * strike / (su - strike);
            double si = strike + (su - strike) * (1.0 - Math.Exp(h2));
            double k = 1.0 - Math.Exp(-rate * maturity);
            double d1 = Math.Log(si / strike) + (carry + vol * vol * 0.5) * maturity;
            d1 /= vol * sqrtMaturity;
            double q2 = 0.5 * (Math.Sqrt((n - 1.0) * (n - 1.0) + 4.0 * m / k) - (n - 1.0));
            double lhs = si - strike;
            double rhs = BlackScholes("C", si, strike, rate, carry, maturity, vol) + (1.0 - Math.Exp((carry - rate) * maturity) * CumNorm(d1)) * si / q2;
            double bi = Math.Exp((carry - rate) * maturity) * CumNorm(d1) * (1.0 - 1.0 / q2) + (1.0 - Math.Exp((carry - rate) * maturity) * NormDensity(d1) / (vol * sqrtMaturity)) / q2;

            while (Math.Abs((lhs - rhs) / strike) > 0.000001)
            {
                si = (strike + rhs - bi * si) / (1.0 - bi);
                d1 = Math.Log(si / strike) + (carry + vol * vol * 0.5) * maturity;
                d1 /= vol * sqrtMaturity;
                lhs = si - strike;
                rhs = BlackScholes("C", si, strike, rate, carry, maturity, vol) + (1.0 - Math.Exp((carry - rate) * maturity) * CumNorm(d1)) * si / q2;
                bi = Math.Exp((carry - rate) * maturity) * CumNorm(d1) * (1.0 - 1.0 / q2) + (1.0 - Math.Exp((carry - rate) * maturity) * NormDensity(d1) / (vol * sqrtMaturity)) / q2;
            }
            option = si;

            return option;
        }


        private static double AmericanPut_BaroneAdesiWhaley(double spot, double strike, double rate, double carry, double maturity, double vol)
        {
            double option = 0;
            double sqrtMaturity = Math.Sqrt(maturity);

            if (carry >= rate)
                option = BlackScholes("P", spot, strike, rate, carry, maturity, vol);
            else
            {
                double sk = AmericanPut_NewtonRaphson(strike, rate, carry, maturity, vol);
                double n = 2.0 * carry / vol / vol;
                double m = 2.0 * rate / vol / vol;
                double k = 1.0 - Math.Exp(-rate * maturity);
                double d1 = Math.Log(sk / strike) + (carry + vol * vol * 0.5) * maturity;
                d1 /= vol * sqrtMaturity;
                double q1 = -(Math.Sqrt((n - 1.0) * (n - 1.0) + 4.0 * m / k) + (n - 1.0)) * 0.5;
                double a1 = -(sk / q1) * (1.0 - Math.Exp((carry - rate) * maturity) * CumNorm(-d1));
                if (spot > sk)
                    option = BlackScholes("P", spot, strike, rate, carry, maturity, vol) + a1 * Math.Pow(spot / sk, q1);
                else
                    option = strike - spot;
            }

            return option;
        }

        private static double AmericanPut_NewtonRaphson(double strike, double rate, double carry, double maturity, double vol)
        {
            double option = 0;
            double sqrtMaturity = Math.Sqrt(maturity);

            double n = 2.0 * carry / vol / vol;
            double m = 2.0 * rate / vol / vol;
            double q1u = -0.5 * (Math.Sqrt((n - 1.0) * (n - 1.0) + 4.0 * m) + (n - 1.0));
            double su = strike / (1.0 - 1.0 / q1u);
            double h1 = (carry * maturity - 2.0 * vol * sqrtMaturity) * strike / (strike - su);
            double si = su + (strike - su) * Math.Exp(h1);
            double k = 1.0 - Math.Exp(-rate * maturity);
            double d1 = Math.Log(si / strike) + (carry + vol * vol * 0.5) * maturity;
            d1 /= vol * sqrtMaturity;
            double q1 = -0.5 * (Math.Sqrt((n - 1.0) * (n - 1.0) + 4.0 * m / k) + (n - 1.0));
            double lhs = strike - si;
            double rhs = BlackScholes("P", si, strike, rate, carry, maturity, vol) - (1.0 - Math.Exp((carry - rate) * maturity) * CumNorm(-d1)) * si / q1;
            double bi = -Math.Exp((carry - rate) * maturity) * CumNorm(-d1) * (1.0 - 1.0 / q1) - (1.0 + Math.Exp((carry - rate) * maturity) * NormDensity(-d1) / (vol * sqrtMaturity)) / q1;

            while (Math.Abs((lhs - rhs) / strike) > 0.000001)
            {
                si = (strike - rhs + bi * si) / (1.0 + bi);
                d1 = Math.Log(si / strike) + (carry + vol * vol * 0.5) * maturity;
                d1 /= vol * sqrtMaturity;
                lhs = strike - si;
                rhs = BlackScholes("P", si, strike, rate, carry, maturity, vol) - (1.0 - Math.Exp((carry - rate) * maturity) * CumNorm(-d1)) * si / q1;
                bi = -Math.Exp((carry - rate) * maturity) * CumNorm(-d1) * (1.0 - 1.0 / q1) - (1.0 + Math.Exp((carry - rate) * maturity) * NormDensity(-d1) / (vol * sqrtMaturity)) / q1;
            }
            option = si;

            return option;
        }
        #endregion American Options
    }
}
