using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows;
using ChartControl;
using System.Windows.Media;

namespace QuantBook.Ch05
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class My2YViewModel : Screen
    {
        private readonly IEventAggregator _events;
        [ImportingConstructor]
        public My2YViewModel(IEventAggregator events)
        {
            this._events = events;
            DisplayName = "03. My Y2";
            
            DataCollection = new BindableCollection<LineSeries2Y>();            
        }

        public BindableCollection<LineSeries2Y> DataCollection { get; set; }
               
        public void AddChart()
        {
            DataCollection.Clear();

            //Add Y curve:
            LineSeries2Y ds = new LineSeries2Y();
            ds.Symbols.BorderColor = Brushes.Blue;
            ds.Symbols.SymbolType = SymbolTypeEnum.OpenDiamond;
            ds.LineColor = Brushes.Blue;
            ds.LineThickness = 2;
            ds.SeriesName = "x*Cos(x)";
            ds.LinePattern = LinePatternEnum.Solid;
            for (int i = 0; i < 20; i++)
            {
                double x = 1.0 * i;
                double y = x * Math.Cos(x);
                ds.LinePoints.Add(new Point(x, y));
            }
            DataCollection.Add(ds);

            //Add Y2 curve:
            ds = new LineSeries2Y();
            ds.Is2YData = true;
            ds.Symbols.BorderColor = Brushes.Red;
            ds.Symbols.SymbolType = SymbolTypeEnum.Dot;
            ds.LineColor = Brushes.Red;
            ds.LineThickness = 2;
            ds.SeriesName = "100 + 20*x";
            ds.LinePattern = LinePatternEnum.DashDot;
            for (int i = 5; i < 30; i++)
            {
                double x = 1.0 * i;
                double y = 100.0 + 20 * x;
                ds.LinePoints.Add(new Point(x, y));
            }
            DataCollection.Add(ds);
        }

    }
}
