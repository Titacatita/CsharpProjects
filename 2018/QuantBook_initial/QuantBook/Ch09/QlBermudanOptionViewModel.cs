using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models.Options;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace QuantBook.Ch09
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class QlBermudanOptionViewModel : Screen
    {
        [ImportingConstructor]
        public QlBermudanOptionViewModel()
        {
            DisplayName = "08. QL: Bermudan";
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

        private IEnumerable<BermudanEngineType> engineType;
        public IEnumerable<BermudanEngineType> EngineType
        {
            get { return Enum.GetValues(typeof(BermudanEngineType)).Cast<BermudanEngineType>(); }
            set
            {
                engineType = value;
                NotifyOfPropertyChange(() => EngineType);
            }
        }

        private BermudanEngineType selectedEngineType;
        public BermudanEngineType SelectedEngineType
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

            OptionInputTable = new DataTable();
            OptionInputTable.Columns.Add("Parameter", typeof(string));
            OptionInputTable.Columns.Add("Value", typeof(string));
            OptionInputTable.Columns.Add("Description", typeof(string));
            OptionInputTable.Rows.Add("OptionType", "C", "C for a call option, otherwise a put option");
            OptionInputTable.Rows.Add("Spot", 100, "Current price of the underlying asset");
            OptionInputTable.Rows.Add("Rate", 0.1, "Interest rate");
            OptionInputTable.Rows.Add("Carry", 0.04, "Cost of carry");
            OptionInputTable.Rows.Add("Vol", 0.3, "Volatility");
            optionInputTable.Rows.Add("ExerciseInterval", 3, "Exercise interval (in month)");
            optionInputTable.Rows.Add("ExerciseTimes", 4, "Total exercise times");
        }

        public void CalculatePrice()
        {
            string optionType = OptionInputTable.Rows[0]["Value"].ToString();
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double carry = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            int interval = Convert.ToInt32(OptionInputTable.Rows[5]["Value"]);
            int times = Convert.ToInt32(OptionInputTable.Rows[6]["Value"]);

            OptionTable.Clear();
            for (int i = 1; i < 11; i++)
            {
                double strike = 20.0 * i;
                object[] res = QuantLibHelper.BermudanOption(optionType, DateTime.Today,interval,times, strike, spot, rate - carry, rate, vol, SelectedEngineType, 200);
                OptionTable.Rows.Add(strike, res[0], res[1], res[2], res[3], res[4], res[5]);
            }
        }
    }
}
