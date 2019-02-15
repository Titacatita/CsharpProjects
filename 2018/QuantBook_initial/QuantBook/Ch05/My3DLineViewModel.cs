using System;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Chart3DControl;

namespace QuantBook.Ch05
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class My3DLineViewModel : Screen
    {
        [ImportingConstructor]
        public My3DLineViewModel()
        {
            DisplayName = "05. My 3DLine";
            DataCollection = new BindableCollection<LineSeries3D>();
        }

        public BindableCollection<LineSeries3D> DataCollection { get; set; }

        public void AddChart()
        {
            DataCollection.Clear();
            LineSeries3D ds = new LineSeries3D();
            ds.LineColor = Brushes.Red;
            for (int i = 0; i < 300; i++)
            {
                double t = 0.1 * i;
                double x = Math.Exp(-t / 30) * Math.Cos(t);
                double y = Math.Exp(-t / 30) * Math.Sin(t);
                double z = t;
                ds.LinePoints.Add(new Point3D(x, y, z));
            }
            DataCollection.Add(ds);
        }
    }
}
