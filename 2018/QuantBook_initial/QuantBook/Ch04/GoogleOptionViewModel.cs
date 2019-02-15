using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using QuantBook.Models.Google;
using System.Xml.Linq;

namespace QuantBook.Ch04
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class GoogleOptionViewModel : Screen
    {
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public GoogleOptionViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "05. Google Option";
            MyExpiries = new BindableCollection<OptionExpiration>();
            MyRecentOptions = new BindableCollection<OptionResults>();
            MyAllOptions = new BindableCollection<OptionResults>();
        }

        public BindableCollection<OptionResults> MyRecentOptions { get; set; }
        public BindableCollection<OptionResults> MyAllOptions { get; set; }
        public BindableCollection<OptionExpiration> MyExpiries { get; set; }

        private string ticker = "GOOG";
        public string Ticker
        {
            get { return ticker; }
            set
            {
                ticker = value;
                NotifyOfPropertyChange(() => Ticker);
            }
        }

        public void RecentOptions()
        {
            MyExpiries.Clear();
            XElement element = GoogleOptions.GetOptionData(Ticker.ToUpper());
            MyExpiries.AddRange(GoogleOptions.GetExpiries(element));

            BindableCollection<OptionResults> o = GoogleOptions.GetOptionResults(element,Ticker);
            MyRecentOptions.Clear();
            MyRecentOptions.AddRange(o);
        }


        public async void AllOptions()
        {
            if(MyExpiries.Count<=0)
            {
                return;
            }
            MyAllOptions.Clear();
           
            await Task.Run(() =>
                {
                    foreach (OptionExpiration ep in MyExpiries)
                    {
                        XElement element = GoogleOptions.GetOptionData(Ticker.ToUpper(), ep.OptionExpiry.ToString());
                        BindableCollection<OptionResults> o = GoogleOptions.GetOptionResults(element, Ticker);
                        MyAllOptions.AddRange(o);
                    }
                });
        }
    }
}
