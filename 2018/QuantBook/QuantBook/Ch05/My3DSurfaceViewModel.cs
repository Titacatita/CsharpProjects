using System;
using System.Linq;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows.Media;
using System.Collections.Generic;
using Chart3DControl;
using QuantBook.Models.ChartModel;

namespace QuantBook.Ch05
{
    [Export(typeof(IScreen)), PartCreationPolicy(CreationPolicy.NonShared)]
    public class My3DSurfaceViewModel : Screen
    {
        [ImportingConstructor]
        public My3DSurfaceViewModel()
        {
            DisplayName = "06. My Surface";
            DataCollection = new BindableCollection<DataSeries3D>();
        }

        public BindableCollection<DataSeries3D> DataCollection { get; set; }       

        private IEnumerable<SurfaceChartTypeEnum> chart3DType;
        public IEnumerable<SurfaceChartTypeEnum> Chart3DType
        {
            get { return Enum.GetValues(typeof(SurfaceChartTypeEnum)).
                  Cast<SurfaceChartTypeEnum>(); }
            set
            {
                chart3DType = value;
                NotifyOfPropertyChange(() => Chart3DType);
            }
        }

        private SurfaceChartTypeEnum selectedChart3DType = 
               SurfaceChartTypeEnum.Mesh;
        public SurfaceChartTypeEnum SelectedChart3DType
        {
            get { return selectedChart3DType; }
            set
            {
                selectedChart3DType = value;
                NotifyOfPropertyChange(() => SelectedChart3DType);
            }
        }

        private IEnumerable<ColormapBrushEnum> colormap;
        public IEnumerable<ColormapBrushEnum> Colormap
        {
            get { return Enum.GetValues(typeof(ColormapBrushEnum)).
                  Cast<ColormapBrushEnum>(); }
            set
            {
                colormap = value;
                NotifyOfPropertyChange(() => Colormap);
            }
        }

        private ColormapBrushEnum selectedColormap = ColormapBrushEnum.Jet;
        public ColormapBrushEnum SelectedColormap
        {
            get { return selectedColormap; }
            set
            {
                selectedColormap = value;
                NotifyOfPropertyChange(() => SelectedColormap);
            }
        }

      

        private bool isColormap = true;
        public bool IsColormap
        {
            get { return isColormap; }
            set
            {
                isColormap = value;
                NotifyOfPropertyChange(() => IsColormap);
            }
        }

        private bool isInterp = false;
        public bool IsInterp
        {
            get { return isInterp; }
            set
            {
                isInterp = value;
                NotifyOfPropertyChange(() => IsInterp);
            }
        }

        public void AddChart()
        {
            DataCollection.Clear();
            DataSeries3D ds = new DataSeries3D();
            ds.LineColor = Brushes.Black;
            ChartFunctions.Peak3D(ds);

            if(SelectedChart3DType == SurfaceChartTypeEnum.Surface)
            {
                if(IsInterp)
                {
                    ds.LineColor = Brushes.Transparent;
                }
            }
            DataCollection.Add(ds);
        }

    }
}
