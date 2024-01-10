using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Contract
{
    public class FreeBrushes : IShape
    {
        public string Name => "Brushes";
        public List<Point> Points { get; set; } = new List<Point>();
        public string Icon => "ellipse";
        public StateShape? Configuration { get; set; }
        public BitmapImage? Preview { get; set; }
        public PointCollection Pointss { get; set; } = new PointCollection();

        public FreeBrushes()
        {

            Preview = new System.Windows.Media.Imaging.BitmapImage();
            Preview.BeginInit();
            Preview.UriSource = new Uri(RelativeToAbsoluteConverter.Convert(@"../../../Img/brushes.png"), UriKind.RelativeOrAbsolute);
            Preview.EndInit();
        }
        public UIElement Draw()
        {
            var polyline = new Polyline
            {
                Points = Pointss, 
                Stroke = Configuration?.ColorBrush ?? Brushes.Black,
                StrokeThickness = Configuration?.Thickness ?? 1,
                StrokeDashArray = Configuration?.StrokeDash ?? null
            };

            return polyline;
        }

        public IShape Clone()
        {
            return new FreeBrushes
            {
                Pointss = new PointCollection(Pointss), 
                Configuration = Configuration?.Clone() as StateShape
            };
        }
    }
}
