using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows;
using ChartControl;
using System.Windows.Media;

namespace QuantBook.Ch05
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class MyMultiLinesViewModel : Screen
    {
        [ImportingConstructor]
        public MyMultiLinesViewModel()
        {
            DisplayName = "02. My MultiLines";

            DataCollection = new BindableCollection<LineSeries>();
        }

        public BindableCollection<LineSeries> DataCollection { get; set; }
      
        public void AddChart()
        {
            DataCollection.Clear();
            LineSeries ds = new LineSeries();
            ds.LineColor = Brushes.Blue;
            ds.LineThickness = 2;
            ds.SeriesName = "Sine";
            ds.LinePattern = LinePatternEnum.Solid;
            ds.Symbols.BorderColor = Brushes.Blue;
            ds.Symbols.SymbolType = SymbolTypeEnum.OpenDiamond;
            for (int i = 0; i < 50; i++)
            {
                double x = i / 5.0;
                double y = Math.Sin(x);
                ds.LinePoints.Add(new Point(x, y));
            }
            DataCollection.Add(ds);

            ds = new LineSeries();
            ds.LineColor = Brushes.Red;
            ds.LineThickness = 2;
            ds.SeriesName = "Cosine";
            ds.LinePattern = LinePatternEnum.Dash;
            ds.Symbols.BorderColor = Brushes.Red;
            ds.Symbols.SymbolType = SymbolTypeEnum.Dot;
            ds.Symbols.FillColor = Brushes.Red;
            for (int i = 0; i < 50; i++)
            {
                double x = i / 5.0;
                double y = Math.Cos(x);
                ds.LinePoints.Add(new Point(x, y));
            }
            DataCollection.Add(ds);

            ds = new LineSeries();
            ds.LineColor = Brushes.DarkGreen;
            ds.LineThickness = 2;
            ds.SeriesName = "Sine^2";
            ds.LinePattern = LinePatternEnum.Dash;
            ds.Symbols.BorderColor = Brushes.DarkGreen;
            ds.Symbols.SymbolType = SymbolTypeEnum.OpenTriangle;
            for (int i = 0; i < 50; i++)
            {
                double x = i / 5.0;
                double y = Math.Sin(x) * Math.Sin(x);
                ds.LinePoints.Add(new Point(x, y));
            }
            DataCollection.Add(ds);
        }
    }
}