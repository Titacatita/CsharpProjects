using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using ChartControl;

namespace QuantBook.Ch05
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MySimpleLineViewModel : Screen
    {
        [ImportingConstructor]
        public MySimpleLineViewModel()
        {
            DisplayName = "01. My Simple Line";            
            DataCollection = new BindableCollection<LineSeries>();            
        }

        public BindableCollection<LineSeries> DataCollection { get; set; }

        public void AddChart()
        {
            DataCollection.Clear();
            LineSeries ls = new LineSeries();
            ls.LineColor = Brushes.Blue;
            ls.LineThickness = 2;
            ls.LinePattern = LinePatternEnum.Solid;
            for (int i = 0; i < 50; i++)
            {
                double x = i / 5.0;
                double y = Math.Sin(x);
                ls.LinePoints.Add(new Point(x, y));
            }
            DataCollection.Add(ls);
        }
    }
}
