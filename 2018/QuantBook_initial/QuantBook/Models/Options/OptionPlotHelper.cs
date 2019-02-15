using Chart3DControl;
using System;
using System.Windows.Media.Media3D;

namespace QuantBook.Models.Options
{
    public static class OptionPlotHelper
    {
        public static double[] PlotGreeks(DataSeries3D ds, GreekTypeEnum greekType, string optionType, double strike, double rate, double carry, double vol)
        {
            double xmin = 0.1;
            double xmax = 3.0;
            double ymin = 10;
            double ymax = 190;

            ds.XLimitMin = xmin;
            ds.YLimitMin = ymin;
            ds.XSpacing = 0.1;
            ds.YSpacing = 5;
            ds.XNumber = Convert.ToInt16((xmax - xmin) / ds.XSpacing) + 1;
            ds.YNumber = Convert.ToInt16((ymax - ymin) / ds.YSpacing) + 1;

            Point3D[,] pts = new Point3D[ds.XNumber, ds.YNumber];
            double zmin = 10000;
            double zmax = -10000;
            for (int i = 0; i < ds.XNumber; i++)
            {
                for (int j = 0; j < ds.YNumber; j++)
                {
                    double x = ds.XLimitMin + i * ds.XSpacing;
                    double y = ds.YLimitMin + j * ds.YSpacing;
                    double z = double.NaN;

                    if (greekType == GreekTypeEnum.Delta)
                        z = OptionHelper.BlackScholes_Delta(optionType, y, strike, rate, carry, x, vol);
                    else if (greekType == GreekTypeEnum.Gamma)
                        z = OptionHelper.BlackScholes_Gamma(y, strike, rate, carry, x, vol);
                    else if (greekType == GreekTypeEnum.Theta)
                        z = OptionHelper.BlackScholes_Theta(optionType, y, strike, rate, carry, x, vol);
                    else if (greekType == GreekTypeEnum.Rho)
                        z = OptionHelper.BlackScholes_Rho(optionType, y, strike, rate, carry, x, vol);
                    else if (greekType == GreekTypeEnum.Vega)
                        z = OptionHelper.BlackScholes_Vega(y, strike, rate, carry, x, vol);
                    else if (greekType == GreekTypeEnum.Price)
                        z = OptionHelper.BlackScholes(optionType, y, strike, rate, carry, x, vol);
                    if (!double.IsNaN(z))
                    {
                        pts[i, j] = new Point3D(x, y, z);
                        zmin = Math.Min(zmin, z);
                        zmax = Math.Max(zmax, z);
                    }
                }
            }
            ds.PointArray = pts;
            return new double[] { zmin, zmax };
        }

        public static double[] PlotImpliedVol1(DataSeries3D ds, string optionType, double spot, double strike, double rate, double carry)
        {
            double ymin = 0.1;
            double ymax = 1.0;
            double xmin = 0.1;
            double xmax = 1;

            ds.XLimitMin = xmin;
            ds.YLimitMin = ymin;
            ds.YSpacing = 0.02;
            ds.XSpacing = 0.02;
            ds.XNumber = Convert.ToInt16((xmax - xmin) / ds.XSpacing) + 1;
            ds.YNumber = Convert.ToInt16((ymax - ymin) / ds.YSpacing) + 1;

            Point3D[,] pts = new Point3D[ds.XNumber, ds.YNumber];
            double zmin = 10000;
            double zmax = -10000;
            for (int i = 0; i < ds.XNumber; i++)
            {
                for (int j = 0; j < ds.YNumber; j++)
                {
                    double x = ds.XLimitMin + i * ds.XSpacing;
                    double y = ds.YLimitMin + j * ds.YSpacing;
                    double z = double.NaN;
                    z = OptionHelper.BlackScholes_ImpliedVol(optionType, spot, strike, rate, carry, y, x);
                    if (!double.IsNaN(z))
                    {
                        pts[i, j] = new Point3D(x, y, z);
                        zmin = Math.Min(zmin, z);
                        zmax = Math.Max(zmax, z);
                    }
                }
            }
            ds.PointArray = pts;
            return new double[] { zmin, zmax };
        }

        public static double[] PlotImpliedVol(DataSeries3D ds, string optionType, double spot, double price, double rate, double carry)
        {
            double xmin = 0.1;
            double xmax = 1.0;
            double ymin = 9.5;
            double ymax = 10.5;

            ds.XLimitMin = xmin;
            ds.YLimitMin = ymin;
            ds.XSpacing = 0.03;
            ds.YSpacing = 0.05;
            ds.XNumber = Convert.ToInt16((xmax - xmin) / ds.XSpacing) + 1;
            ds.YNumber = Convert.ToInt16((ymax - ymin) / ds.YSpacing) + 1;

            Point3D[,] pts = new Point3D[ds.XNumber, ds.YNumber];
            double zmin = 10000;
            double zmax = -10000;
            for (int i = 0; i < ds.XNumber; i++)
            {
                for (int j = 0; j < ds.YNumber; j++)
                {
                    double x = ds.XLimitMin + i * ds.XSpacing;
                    double y = ds.YLimitMin + j * ds.YSpacing;
                    double z = double.NaN;
                    z = OptionHelper.BlackScholes_ImpliedVol(optionType, spot, y, rate, carry, x, price);
                    if (!double.IsNaN(z))
                    {
                        pts[i, j] = new Point3D(x, y, z);
                        zmin = Math.Min(zmin, z);
                        zmax = Math.Max(zmax, z);
                    }
                }
            }
            ds.PointArray = pts;
            return new double[] { zmin, zmax };
        }
    }

    public enum GreekTypeEnum
    {
        Delta = 0,
        Gamma = 1,
        Theta = 2,
        Rho = 3,
        Vega = 4,
        Price = 5,
    }
}
