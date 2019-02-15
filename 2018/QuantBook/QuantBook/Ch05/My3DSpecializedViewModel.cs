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
    public class My3DSpecializedViewModel : Screen
    {
        [ImportingConstructor]
        public My3DSpecializedViewModel()
        {
            DisplayName = "07. 3D Specialized";
            DataCollection = new BindableCollection<DataSeries3D>();
        }

        private DataSeries3D ds;
        public BindableCollection<DataSeries3D> DataCollection { get; set; }
       

        private IEnumerable<SChartTypeEnum> chart3DSType;
        public IEnumerable<SChartTypeEnum> Chart3DSType
        {
            get { return Enum.GetValues(typeof(SChartTypeEnum)).
                  Cast<SChartTypeEnum>(); }
            set
            {
                chart3DSType = value;
                NotifyOfPropertyChange(() => Chart3DSType);
            }
        }

        private SChartTypeEnum selectedChart3DSType = SChartTypeEnum.XYColor;
        public SChartTypeEnum SelectedChart3DSType
        {
            get { return selectedChart3DSType; }
            set
            {
                selectedChart3DSType = value;
                NotifyOfPropertyChange(() => SelectedChart3DSType);
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

        private bool isColorbar = true;
        public bool IsColorbar
        {
            get { return isColorbar; }
            set
            {
                isColorbar = value;
                NotifyOfPropertyChange(() => IsColorbar);
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

        private bool isLineColorMatch = false;
        public bool IsLineColorMatch
        {
            get { return isLineColorMatch; }
            set
            {
                isLineColorMatch = value;
                NotifyOfPropertyChange(() => IsLineColorMatch);
            }
        }

        public BindableCollection<string> XYColorType
        {
            get { return new BindableCollection<string> 
                  { "BlackLines", "NoLinesNoInterp", "NoLinesWithInterp" }; }
        }

        private string selectedXYColorType = "BlackLines";
        public string SelectedXYColorType
        {
            get { return selectedXYColorType; }
            set
            {
                selectedXYColorType = value;
                NotifyOfPropertyChange(() => SelectedXYColorType);
            }
        }

        public BindableCollection<string> ContourType
        {
            get { return new BindableCollection<string> 
                { "BlackLines", "LinesWithColormap" }; }
        }

        private string selectedContourType = "BlackLines";
        public string SelectedContourType
        {
            get { return selectedContourType; }
            set
            {
                selectedContourType = value;
                NotifyOfPropertyChange(() => SelectedContourType);
            }
        }

        public void AddChart()
        {
            DataCollection.Clear();
            switch (SelectedChart3DSType)
            {
                case SChartTypeEnum.XYColor:
                    AddXYColor();
                    break;
                case SChartTypeEnum.Contour:
                    AddContour();
                    break;
                case SChartTypeEnum.FillContour:
                    AddFilledContour();
                    break;
                case SChartTypeEnum.MeshContour3D:
                    AddMeshContour();
                    break;
                case SChartTypeEnum.SurfaceContour3D:
                    AddSurfaceContour();
                    break;
                case SChartTypeEnum.SurfaceFillContour3D:
                    AddSurfaceFillContour();
                    break;
            }

            DataCollection.Add(ds);
        }

        private void AddXYColor()
        {
            DisplayName = "03. X-Y Color";
            SelectedChart3DSType = SChartTypeEnum.XYColor;
            ds = new DataSeries3D();
            ChartFunctions.Peak3D(ds);

            if (SelectedXYColorType == "BlackLines")
            {
                ds.LineColor = Brushes.Black;
                IsInterp = false;
            }
            else if (SelectedXYColorType == "NoLinesNoInterp")
            {
                ds.LineColor = Brushes.Transparent;
                IsInterp = false;
            }
            else if (SelectedXYColorType == "NoLinesWithInterp")
            {
                ds.LineColor = Brushes.Transparent;
                IsInterp = true;
            }
        }

        private void AddContour()
        {
            DisplayName = "03. Contour";
            SelectedChart3DSType = SChartTypeEnum.Contour;
            ds = new DataSeries3D();
            ChartFunctions.Peak3D(ds);

            if (SelectedContourType == "BlackLines")
            {
                ds.LineColor = Brushes.Black;
                IsColorbar = false;
            }
            else if (SelectedContourType == "LinesWithColormap")
            {
                IsColorbar = true;
                IsLineColorMatch = true;
                SelectedColormap = ColormapBrushEnum.Jet;
            }
        }

        private void AddFilledContour()
        {
            DisplayName = "03. Filled Contour";
            SelectedChart3DSType = SChartTypeEnum.FillContour;
            ds = new DataSeries3D();
            ChartFunctions.Peak3D(ds);

            IsColorbar = true;
            IsLineColorMatch = true;
            SelectedColormap = ColormapBrushEnum.Jet;
            IsInterp = true;
        }

        private void AddMeshContour()
        {
            DisplayName = "03. Mesh Contour";
            SelectedChart3DSType = SChartTypeEnum.MeshContour3D;
            ds = new DataSeries3D();
            ChartFunctions.Peak3D(ds);

            ds.LineColor = Brushes.Black;
            IsColorbar = true;
            IsLineColorMatch = true;
            SelectedColormap = ColormapBrushEnum.Jet;
        }

        private void AddSurfaceContour()
        {
            DisplayName = "03. Surface Contour";
            SelectedChart3DSType = SChartTypeEnum.SurfaceContour3D;
            ds = new DataSeries3D();
            ChartFunctions.Peak3D(ds);

            ds.LineColor = Brushes.Black;
            IsColorbar = true;
            IsLineColorMatch = true;
            SelectedColormap = ColormapBrushEnum.Jet;
        }

        private void AddSurfaceFillContour()
        {
            DisplayName = "03. Surface Contour";
            SelectedChart3DSType = SChartTypeEnum.SurfaceFillContour3D;
            ds = new DataSeries3D();
            ChartFunctions.Peak3D(ds);

            ds.LineColor = Brushes.Black;
            IsColorbar = true;
            isLineColorMatch = false;
            SelectedColormap = ColormapBrushEnum.Jet;
        }

    }
}
