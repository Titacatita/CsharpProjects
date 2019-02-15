using System;
using System.Linq;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using QuantBook.Models;
using System.Windows.Media;
using System.Windows.Controls;

namespace QuantBook
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : Conductor<IScreen>.Collection.OneActive, IConductor, IHandle<ModelEvents>
    {
        IEnumerable<IScreen> myScreens;
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public MainViewModel([ImportMany]IEnumerable<IScreen> screens, IEventAggregator events)
        {
            this.myScreens = screens;
            Items.Clear();
            this._events = events;
            _events.Subscribe(this);
            DisplayName = "Quant Book Project";
        }

        private string chapter;
        public string Chapter
        {
            get { return chapter; }
            set
            {
                chapter = value;
                NotifyOfPropertyChange(() => Chapter);
            }
        }

        public void Handle(ModelEvents evnt)
        {
            List<object> lst = evnt.EventList;
            StatusText = lst[0].ToString();
            if (lst.Count > 1)
            {
                ProgressMin = Convert.ToInt32(lst[1]);
                ProgressMax = Convert.ToInt32(lst[2]);
                ProgressValue = Convert.ToInt32(lst[3]);
            }
        }

        private int progressMin = 0;
        public int ProgressMin
        {
            get { return progressMin; }
            set
            {
                progressMin = value;
                NotifyOfPropertyChange(() => ProgressMin);
            }
        }

        private int progressMax = 1;
        public int ProgressMax
        {
            get { return progressMax; }
            set
            {
                progressMax = value;
                NotifyOfPropertyChange(() => ProgressMax);
            }
        }

        private int progressValue = 0;
        public int ProgressValue
        {
            get { return progressValue; }
            set
            {
                progressValue = value;
                NotifyOfPropertyChange(() => ProgressValue);
            }
        }
        private string statusText;
        public string StatusText
        {
            get { return statusText; }
            set
            {
                statusText = value;
                NotifyOfPropertyChange(() => StatusText);
            }
        }
        

       
        public void OnClick(object sender)
        {
            Button btn = sender as Button;
            int num = Convert.ToInt16(btn.Content.ToString().Split(' ')[1]);
            string ch = "Ch";
            if (num < 10)
                ch += "0" + num.ToString();
            else
                ch += num.ToString();
           
            Items.Clear();
            IOrderedEnumerable<IScreen> sc = from q in myScreens where q.ToString().Contains(ch) orderby q.DisplayName select q;
            Items.AddRange(sc);

            MainView view = this.GetView() as MainView;
            foreach (Button b in view.buttonPanel.Children)
            {
                b.Background = Brushes.Transparent;
                b.Foreground = Brushes.Black;
            }

            btn.Background = Brushes.Black;
            btn.Foreground = Brushes.White;
        }
    }
}