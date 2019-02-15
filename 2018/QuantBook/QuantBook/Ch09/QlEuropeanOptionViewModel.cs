using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models.Options;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using QuantLib;

namespace QuantBook.Ch09
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class QlEuropeanOptionViewModel : Screen
    {
        [ImportingConstructor]
        public QlEuropeanOptionViewModel()
        {
            DisplayName = "05. QL: European";
            InitializeModel();
        }

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

        private DataTable volInputTable;
        public DataTable VolInputTable
        {
            get { return volInputTable; }
            set
            {
                volInputTable = value;
                NotifyOfPropertyChange(() => VolInputTable);
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

        private DataTable volTable;
        public DataTable VolTable
        {
            get { return volTable; }
            set
            {
                volTable = value;
                NotifyOfPropertyChange(() => VolTable);
            }
        }

        private IEnumerable<EuropeanEngineType> engineType;
        public IEnumerable<EuropeanEngineType> EngineType
        {
            get { return Enum.GetValues(typeof(EuropeanEngineType)).Cast<EuropeanEngineType>(); }
            set
            {
                engineType = value;
                NotifyOfPropertyChange(() => EngineType);
            }
        }

        private EuropeanEngineType selectedEngineType;
        public EuropeanEngineType SelectedEngineType
        {
            get { return selectedEngineType; }
            set
            {
                selectedEngineType = value;
                NotifyOfPropertyChange(() => SelectedEngineType);
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
            
            VolTable = new DataTable();
            VolTable.Columns.Add("Maturity", typeof(double));
            VolTable.Columns.Add("Option Price", typeof(double));
            VolTable.Columns.Add("Volatility", typeof(double));

            OptionInputTable = new DataTable();
            OptionInputTable.Columns.Add("Parameter", typeof(string));
            OptionInputTable.Columns.Add("Value", typeof(string));
            OptionInputTable.Columns.Add("Description", typeof(string));
            OptionInputTable.Rows.Add("OptionType", "C", "C for a call option, otherwise a put option");
            OptionInputTable.Rows.Add("Spot", 100, "Current price of the underlying asset");
            OptionInputTable.Rows.Add("Strike", 100, "Strike price");
            OptionInputTable.Rows.Add("Rate", 0.1, "Interest rate");
            OptionInputTable.Rows.Add("Carry", 0.04, "Cost of carry");
            OptionInputTable.Rows.Add("Vol", 0.3, "Volatility");

            VolInputTable = new DataTable();
            VolInputTable.Columns.Add("Parameter", typeof(string));
            VolInputTable.Columns.Add("Value", typeof(string));
            VolInputTable.Columns.Add("Description", typeof(string));
            VolInputTable.Rows.Add("OptionType", "C", "C for a call option, otherwise a put option");
            VolInputTable.Rows.Add("Spot", 10, "Current price of the underlying asset");
            VolInputTable.Rows.Add("Strike", 10.5, "Strike price");
            VolInputTable.Rows.Add("Rate", 0.1, "Interest rate");
            VolInputTable.Rows.Add("Carry", 0.06, "Cost of carry");
            VolInputTable.Rows.Add("Vol", 0.3, "Volatility");
        }

        public void CalculatePrice()
        {
            string optionType = OptionInputTable.Rows[0]["Value"].ToString(); 
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double carry = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);

            OptionTable.Clear();
            for (int i = 0; i < 10; i++)
            {
                double maturity = (i + 1.0) / 10.0;
                //object[] res = QlOptionHelper.EuropeanOption(optionType, DateTime.Today, maturity, strike, spot, rate - carry, rate, vol, SelectedEngineType, 5, 50);
                
                object[] res = QuantLibHelper.EuropeanOption(optionType, DateTime.Today, maturity, strike, spot, rate - carry, rate, vol, SelectedEngineType, 200);
                OptionTable.Rows.Add(maturity, res[0], res[1], res[2], res[3], res[4], res[5]);
            }
        }

        public void CalculateVol()
        {
            string optionType = VolInputTable.Rows[0]["Value"].ToString();
            double spot = Convert.ToDouble(VolInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(VolInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(VolInputTable.Rows[3]["Value"]);
            double carry = Convert.ToDouble(VolInputTable.Rows[4]["Value"]);
           
            VolTable.Clear();
            double[] prices = new double[] { 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5, 0.55, 0.6 };
            for (int i = 0; i < 10; i++)
            {
                double maturity = (i + 1.0) / 10.0;
                //object volatility = QlOptionHelper.EuropeanOptionImpliedVol(optionType, DateTime.Today, maturity, strike, spot, rate - carry, rate, prices[i]);
                object volatility = QuantLibHelper.EuropeanOptionImpliedVol(optionType, DateTime.Today, maturity, strike, spot, rate - carry, rate, prices[i]);
                VolTable.Rows.Add(maturity, prices[i], volatility);
            }
        }
    }
}
