using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using QuantBook.Models;

namespace QuantBook.Ch02
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class CommunicationViewModel : Screen
    {
        private readonly IEventAggregator _events; 

        [ImportingConstructor]
        public CommunicationViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "06. Communication";
            Ticker = "IBM";
            Date = Convert.ToDateTime("7/14/2015");
            PriceOpen = 169.43;
            PriceHigh = 169.54;
            PriceLow = 168.24;
            PriceClose = 168.61;
        }

        private string ticker;
        private DateTime date;
        private double priceOpen;
        private double priceHigh;
        private double priceLow;
        private double priceClose;

        public string Ticker
        {
            get { return ticker; }
            set
            {
                ticker = value;
                NotifyOfPropertyChange(() => Ticker);
            }
        }

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                NotifyOfPropertyChange(() => Date);
            }
        }

        public double PriceOpen
        {
            get { return priceOpen; }
            set
            {
                priceOpen = value;
                NotifyOfPropertyChange(() => PriceOpen);
            }
        }

        public double PriceHigh
        {
            get { return priceHigh; }
            set
            {
                priceHigh = value;
                NotifyOfPropertyChange(() => PriceHigh);
            }
        }

        public double PriceLow
        {
            get { return priceLow; }
            set
            {
                priceLow = value;
                NotifyOfPropertyChange(() => PriceLow);
            }
        }

        public double PriceClose
        {
            get { return priceClose; }
            set
            {
                priceClose = value;
                NotifyOfPropertyChange(() => PriceClose);
            }
        }

        private bool isStop = false;
        public bool IsStop
        {
            get { return isStop; }
            set
            {
                isStop = value;
                NotifyOfPropertyChange(() => IsStop);
            }
        }

        public async void Start()
        {
            IsStop = false;
            List<object> objList = new List<object>();
            int totalRuns = 200;

            objList.Add("Ready...");
            objList.Add(0);
            objList.Add(totalRuns);
            objList.Add(0);

            await Task.Run(() =>
                {
                    for (int i = 0; i < totalRuns; i++)
                    {
                        if (IsStop)
                            break;

                        Thread.Sleep(200);
                        if (i % 2 == 0)
                        {
                            Ticker = "MSFT";
                            Date = Convert.ToDateTime("7/14/2015");
                            PriceOpen = 45.45;
                            PriceHigh = 45.96;
                            PriceLow = 45.31;
                            PriceClose = 45.62;
                        }
                        else
                        {
                            Ticker = "IBM";
                            Date = Convert.ToDateTime("7/14/2015");
                            PriceOpen = 169.43;
                            PriceHigh = 169.54;
                            PriceLow = 168.24;
                            PriceClose = 168.61;
                        }

                        objList[0] = string.Format("Total Runs = {0}, i={1}, Ticker = {2}", totalRuns, i, Ticker);
                        objList[3] = i;
                        _events.PublishOnUIThread(new ModelEvents(objList));
                    }
                });

            objList[0] = "Ready...";
            if (IsStop)
                objList[0] = "Stop...";
            objList[1] = 0;
            objList[2] = 1;
            objList[3] = 0;
            _events.PublishOnUIThread(new ModelEvents(objList)); 

        }

        public void Stop()
        {
            IsStop = true;
        }
    }
}
