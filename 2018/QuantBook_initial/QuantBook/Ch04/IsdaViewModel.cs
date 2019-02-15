using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using QuantBook.Models.Isda;

namespace QuantBook.Ch04
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class IsdaViewModel : Screen
    {
         private readonly IEventAggregator _events;
        [ImportingConstructor]
        public IsdaViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "07. ISDA Rates";
            MyRates = new BindableCollection<IsdaRate>();
            EndDate = DateTime.Today.AddDays(-1);
            StartDate = EndDate.AddDays(-5);
        }

        public BindableCollection<IsdaRate> MyRates { get; set; }
        private string currency = "USD";
        public string Currency
        {
            get { return currency; }
            set
            {
                currency = value;
                NotifyOfPropertyChange(() => Currency);
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

        public async void GetRates()
        {
            MyRates.Clear();
            await Task.Run(() =>
            {
                BindableCollection<IsdaRate> rate = IsdaHelper.GetIsdaRates(Currency, StartDate, EndDate, _events);
                MyRates.AddRange(rate);
            });
        }
    }
}
