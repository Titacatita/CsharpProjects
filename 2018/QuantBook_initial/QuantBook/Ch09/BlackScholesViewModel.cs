using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models.Options;
using System.Data;
using Chart3DControl;
using System.Windows.Media;

namespace QuantBook.Ch09
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BlackScholesViewModel : Screen
    {
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public BlackScholesViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "01. Black-Scholes";
            InitializeModel();
            DataCollection = new BindableCollection<DataSeries3D>();
        }

        public BindableCollection<DataSeries3D> DataCollection { get; set; }  

        private DataTable optionInputTable;
        public DataTable OptionInputTable
        {
            get { return optionInputTable; }
            set
            {
                optionInputTable = value;
                NotifyOfPropertyChange(() => OptionInputTable);
            }
        }

        private DataTable optionTable;
        public DataTable OptionTable
        {
            get { return optionTable; }
            set
            {
                optionTable = value;
                NotifyOfPropertyChange(() => OptionTable);
            }
        }

        private double zmin = 0;
        public double Zmin
        {
            get { return zmin; }
            set
            {
                zmin = value;
                NotifyOfPropertyChange(() => Zmin);
            }
        }

        private double zmax = 1;
        public double Zmax
        {
            get { return zmax; }
            set
            {
                zmax = value;
                NotifyOfPropertyChange(() => Zmax);
            }
        }

        private string zLable = "Delta";
        public string ZLabel
        {
            get { return zLable; }
            set
            {
                zLable = value;
                NotifyOfPropertyChange(() => ZLabel);
            }
        }

        private double zTick = 0.2;
        public double ZTick
        {
            get { return zTick; }
            set
            {
                zTick = value;
                NotifyOfPropertyChange(() => ZTick);
            }
        }


        private void InitializeModel()
        {
            OptionTable = new DataTable();
            OptionTable.Columns.Add("Maturity", typeof(double));
            OptionTable.Columns.Add("Price", typeof(double));
            OptionTable.Columns.Add("Delta", typeof(double));
            OptionTable.Columns.Add("Gamma", typeof(double));
            OptionTable.Columns.Add("Theta", typeof(double));
            OptionTable.Columns.Add("Rho", typeof(double));
            OptionTable.Columns.Add("Vega", typeof(double));

            OptionInputTable = new DataTable();
            OptionInputTable.Columns.Add("Parameter", typeof(string));
            OptionInputTable.Columns.Add("Value", typeof(string));
            OptionInputTable.Columns.Add("Description", typeof(string));
            OptionInputTable.Rows.Add("OptionType", "C", "C for a call option, P for a put option");
            OptionInputTable.Rows.Add("Spot", 100, "Current price of the underlying asset");
            OptionInputTable.Rows.Add("Strike", 100, "Strike price");
            OptionInputTable.Rows.Add("Rate", 0.1, "Interest rate");
            OptionInputTable.Rows.Add("Carry", 0.04, "Cost of carry");
            OptionInputTable.Rows.Add("Vol", 0.3, "Volatility");
        }

        public void CalculatePrice()
        {   
            string optionType = "C";
            if(optionInputTable.Rows[0]["Value"].ToString().ToUpper() != "C")
                optionType = "P";
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);

            OptionTable.Clear();
            for (int i = 0; i < 10; i++)
            {
                double maturity = (i + 1.0) / 10.0;
                double price = OptionHelper.BlackScholes(optionType, spot, strike, rate, carry, maturity, vol);
                double delta = OptionHelper.BlackScholes_Delta(optionType, spot, strike, rate, carry, maturity, vol);
                double gamma = OptionHelper.BlackScholes_Gamma(spot, strike, rate, carry, maturity, vol);
                double theta = OptionHelper.BlackScholes_Theta(optionType, spot, strike, rate, carry, maturity, vol);
                double rho = OptionHelper.BlackScholes_Rho(optionType, spot, strike, rate, carry, maturity, vol);
                double vega = OptionHelper.BlackScholes_Vega(spot, strike, rate, carry, maturity, vol);
                OptionTable.Rows.Add(maturity, price, delta, gamma, theta, rho, vega);
            }
        }

        public void PlotPrice()
        {
            ZLabel = "Price";
            string optionType = "C";
            if (optionInputTable.Rows[0]["Value"].ToString().ToUpper() != "C")
                optionType = "P";
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);

            DataCollection.Clear();
            DataSeries3D ds = new DataSeries3D();
            ds.LineColor = Brushes.Black;
            double[] z = OptionPlotHelper.PlotGreeks(ds, GreekTypeEnum.Price, optionType, strike, rate, carry, vol);
            Zmin = Math.Round(z[0], 1);
            Zmax = Math.Round(z[1], 1);
            ZTick = Math.Round((z[1] - z[0]) / 5.0, 1);
            DataCollection.Add(ds);
        }

        public void PlotDelta()
        {
            ZLabel = "Delta";
            string optionType = "C";
            if(optionInputTable.Rows[0]["Value"].ToString().ToUpper() != "C")
                optionType = "P";
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);

            DataCollection.Clear();
            DataSeries3D ds = new DataSeries3D();
            ds.LineColor = Brushes.Black;
            double[] z = OptionPlotHelper.PlotGreeks(ds, GreekTypeEnum.Delta, optionType, strike, rate, carry, vol);
            Zmin = Math.Round(z[0],1);
            Zmax = Math.Round(z[1],1);
            ZTick = Math.Round((z[1] - z[0])/5.0, 1);
            DataCollection.Add(ds);
        }

        public void PlotGamma()
        {
            ZLabel = "Gamma";
            string optionType = "C";
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);

            DataCollection.Clear();
            DataSeries3D ds = new DataSeries3D();
            ds.LineColor = Brushes.Black;
            double[] z = OptionPlotHelper.PlotGreeks(ds, GreekTypeEnum.Gamma, optionType, strike, rate, carry, vol);
            Zmin = Math.Round(z[0], 2);
            Zmax = Math.Round(z[1], 2);
            ZTick = Math.Round((z[1] - z[0]) / 5.0, 3);
            DataCollection.Add(ds);
        }

        public void PlotTheta()
        {
            ZLabel = "Theta";
            string optionType = "C";
            if (optionInputTable.Rows[0]["Value"].ToString().ToUpper() != "C")
                optionType = "P";
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);

            DataCollection.Clear();
            DataSeries3D ds = new DataSeries3D();
            ds.LineColor = Brushes.Black;
            double[] z = OptionPlotHelper.PlotGreeks(ds, GreekTypeEnum.Theta, optionType, strike, rate, carry, vol);
            Zmin = Math.Round(z[0], 0);
            Zmax = Math.Round(z[1], 0);
            ZTick = Math.Round((z[1] - z[0]) / 5.0, 0);
            DataCollection.Add(ds);
        }

        public void PlotRho()
        {
            ZLabel = "Rho";
            string optionType = "C";
            if (optionInputTable.Rows[0]["Value"].ToString().ToUpper() != "C")
                optionType = "P";
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);

            DataCollection.Clear();
            DataSeries3D ds = new DataSeries3D();
            ds.LineColor = Brushes.Black;
            double[] z = OptionPlotHelper.PlotGreeks(ds, GreekTypeEnum.Rho, optionType, strike, rate, carry, vol);
            Zmin = Math.Round(z[0], 0);
            Zmax = Math.Round(z[1], 0);
            ZTick = Math.Round((z[1] - z[0]) / 5.0, 0);
            DataCollection.Add(ds);
        }

        public void PlotVega()
        {
            ZLabel = "Vega";
            string optionType = "C";
            if (optionInputTable.Rows[0]["Value"].ToString().ToUpper() != "C")
                optionType = "P";
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);

            DataCollection.Clear();
            DataSeries3D ds = new DataSeries3D();
            ds.LineColor = Brushes.Black;
            double[] z = OptionPlotHelper.PlotGreeks(ds, GreekTypeEnum.Vega, optionType, strike, rate, carry, vol);
            Zmin = Math.Round(z[0], 0);
            Zmax = Math.Round(z[1], 0);
            ZTick = Math.Round((z[1] - z[0]) / 5.0, 0);
            DataCollection.Add(ds);
        }
    }
}
