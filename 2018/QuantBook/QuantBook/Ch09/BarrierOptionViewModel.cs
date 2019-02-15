﻿using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models.Options;
using System.Data;

namespace QuantBook.Ch09
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BarrierOptionViewModel : Screen
    {
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public BarrierOptionViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "04. Barrier Option";
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
            OptionInputTable.Rows.Add("OptionType", "C", "C for a call option, otherwise a put option");
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
                object value_downIn = OptionHelper.BarrierOptions(optionType, "DownIn", spot, strike, rate, yield, maturity, vol, barrier, rebate);
                object value_upIn = OptionHelper.BarrierOptions(optionType, "UpIn", spot, strike, rate, yield, maturity, vol, barrier, rebate);
                object value_downOut = OptionHelper.BarrierOptions(optionType, "DownOut", spot, strike, rate, yield, maturity, vol, barrier, rebate);
                object value_upOut = OptionHelper.BarrierOptions(optionType, "UpOut", spot, strike, rate, yield, maturity, vol, barrier, rebate);
                OptionTable.Rows.Add(maturity, value_downIn, value_downOut, value_upIn, value_upOut);
            }

        }
    }
}
