using System;
using System.Data;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using QuantBook.Models.Bloomberg;

namespace QuantBook.Ch04
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class BloombergViewModel : Screen
    {
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public BloombergViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "08. Bloomberg";
        }

        private DateTime startDate = Convert.ToDateTime("1/1/2015");
        private   DateTime endDate = DateTime.Today;
        private string[] tickers = new string[] { "IBM Equity", "SPX INDEX", "MSFT Equity" };
        private string[] flds = new string[] { "PX_OPEN", "PX_HIGH", "PX_LOW", "PX_LAST", "PX_VOLUME" };
       
        private DataTable myTable = new DataTable();
        public DataTable MyTable
        {
            get { return myTable; }
            set
            {
                myTable = value;
                NotifyOfPropertyChange(() => MyTable);
            }
        }

        private string dataType = "Historical Data:";
        public string DataType
        {
            get { return dataType; }
            set
            {
                dataType = value;
                NotifyOfPropertyChange(() => DataType);
            }
        }

        public void HistData()
        {
            DataType = "Hostorical Data:";
            MyTable = BloombergHelper.BloombergHistData(tickers, flds, startDate, endDate);
        }

        public void RefData()
        {
            DataType = "Reference Data:";
            MyTable = BloombergHelper.BloombergReferenceData(tickers, flds);
        }

        public void TickData()
        {
            DataType = "Tick Data:";
            MyTable = BloombergHelper.BloombergTickData("IBM US Equity", endDate.AddDays(-1), endDate); 
        }

        public void BarData()
        {
            DataType = "Bar Data:";
            MyTable = BloombergHelper.BloombergBarData("IBM US Equity", 1, endDate.AddDays(-1), endDate);
        }
    }
}
