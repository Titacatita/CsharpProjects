using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using QuantBook.Models.DataModel.Quandl;
using System.Data;

namespace QuantBook.Ch04
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class QuandlViewModel : Screen
    {
         private readonly IEventAggregator _events;
        [ImportingConstructor]
        public QuandlViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "06. Quandl";
            ticker = "IBM";
            DataSource = "WIKI";
            DataLabel = "Data from Quandl";
            StartDate = DateTime.Today.AddYears(-1);
            EndDate = DateTime.Today;
        }

        private DataTable myTable;
        public DataTable MyTable
        {
            get { return myTable; }
            set
            {
                myTable = value;
                NotifyOfPropertyChange(() => MyTable);
            }
        }

        private string ticker;
        public string Ticker
        {
            get { return ticker; }
            set
            {
                ticker = value;
                NotifyOfPropertyChange(() => Ticker);
            }
        }

        private string dataLabel;
        public string DataLabel
        {
            get { return dataLabel; }
            set
            {
                dataLabel = value;
                NotifyOfPropertyChange(() => DataLabel);
            }
        }

        private string dataSource;
        public string DataSource
        {
            get { return dataSource; }
            set
            {
                dataSource = value;
                NotifyOfPropertyChange(() => DataSource);
            }
        }

        private DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                startDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }

        private DateTime endDate;
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }

        public void GetData()
        {
            DataLabel = string.Format("Data for {0} from {1}:", Ticker, DataSource);
            DataTable table = QuandlHelper.GetQuandlData(Ticker, DataSource, StartDate, EndDate);
            MyTable = table;
        }
    }
}
