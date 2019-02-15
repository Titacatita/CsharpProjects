using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models.Options;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace QuantBook.Ch09
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class QlBarrierOptionViewModel : Screen
    {
         private readonly IEventAggregator _events;
        [ImportingConstructor]
        public QlBarrierOptionViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "07. QL: Barrier Option";
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

        private IEnumerable<BarrierEngineType> engineType;
        public IEnumerable<BarrierEngineType> EngineType
        {
            get { return Enum.GetValues(typeof(BarrierEngineType)).Cast<BarrierEngineType>(); }
            set
            {
                engineType = value;
                NotifyOfPropertyChange(() => EngineType);
            }
        }

        private BarrierEngineType selectedEngineType;
        public BarrierEngineType SelectedEngineType
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
            OptionTable.Columns.Add("DownIn", typeof(double));
            OptionTable.Columns.Add("DownOut", typeof(double));
            OptionTable.Columns.Add("UpIn", typeof(double));           
            OptionTable.Columns.Add("UpOut", typeof(double));
           
            OptionInputTable = new DataTable();
            OptionInputTable.Columns.Add("Parameter", typeof(string));
            OptionInputTable.Columns.Add("Value", typeof(string));
            OptionInputTable.Columns.Add("Description", typeof(string));
            OptionInputTable.Rows.Add("OptionType", "C", "P for a put option, otherwise a call option");
            OptionInputTable.Rows.Add("Spot", 100, "Current price of the underlying asset");
            OptionInputTable.Rows.Add("Strike", 100, "Strike price");
            OptionInputTable.Rows.Add("Rate", 0.1, "Interest rate");
            OptionInputTable.Rows.Add("DivYield", 0.06, "Continoues dividend yield");
            OptionInputTable.Rows.Add("Vol", 0.3, "Volatility");
            OptionInputTable.Rows.Add("Barrier", 90, "Barrier level");
            OptionInputTable.Rows.Add("Rebate", 0, "Paid off if barrier is not knocked in during its life");
        }

        public void CalculatePrice()
        {
            OptionTable.Clear();

            string optionType = OptionInputTable.Rows[0]["Value"].ToString();
            double spot = Convert.ToDouble(OptionInputTable.Rows[1]["Value"]);
            double strike = Convert.ToDouble(OptionInputTable.Rows[2]["Value"]);
            double rate = Convert.ToDouble(OptionInputTable.Rows[3]["Value"]);
            double yield = Convert.ToDouble(OptionInputTable.Rows[4]["Value"]);
            double vol = Convert.ToDouble(OptionInputTable.Rows[5]["Value"]);
            double barrier = Convert.ToDouble(OptionInputTable.Rows[6]["Value"]);
            double rebate = Convert.ToDouble(OptionInputTable.Rows[7]["Value"]);

            for (int i = 1; i <= 10; i++)
            {
                double maturity = 0.1 * i;
                /*object value_downIn = QlOptionHelper.BarrierOption(optionType, DateTime.Today, maturity, strike, spot, barrier, rebate, yield, rate, vol, QLNet.Barrier.Type.DownIn);
                object value_upIn = QlOptionHelper.BarrierOption(optionType, DateTime.Today, maturity, strike, spot, barrier, rebate, yield, rate, vol, QLNet.Barrier.Type.UpIn);
                object value_downOut = QlOptionHelper.BarrierOption(optionType, DateTime.Today, maturity, strike, spot, barrier, rebate, yield, rate, vol, QLNet.Barrier.Type.DownOut);
                object value_upOut = QlOptionHelper.BarrierOption(optionType, DateTime.Today, maturity, strike, spot, barrier, rebate, yield, rate, vol, QLNet.Barrier.Type.UpOut);*/

                object value_downIn = QuantLibHelper.BarrierOption(optionType, DateTime.Today, maturity, strike, spot, barrier, rebate, yield, rate, vol, QuantLib.Barrier.Type.DownIn, SelectedEngineType,100);
                object value_upIn = QuantLibHelper.BarrierOption(optionType, DateTime.Today, maturity, strike, spot, barrier, rebate, yield, rate, vol, QuantLib.Barrier.Type.UpIn, SelectedEngineType,100);
                object value_downOut = QuantLibHelper.BarrierOption(optionType, DateTime.Today, maturity, strike, spot, barrier, rebate, yield, rate, vol, QuantLib.Barrier.Type.DownOut, SelectedEngineType,100);
                object value_upOut = QuantLibHelper.BarrierOption(optionType, DateTime.Today, maturity, strike, spot, barrier, rebate, yield, rate, vol, QuantLib.Barrier.Type.UpOut, SelectedEngineType, 100);
                OptionTable.Rows.Add(maturity, value_downIn, value_downOut, value_upIn, value_upOut);
            }
        }
    }
}
