using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Contract
{
    public class FreehandDrawing : IShape
    {
        public override string Name => "Brushes"; 

        public PointCollection Points { get; set; } = new PointCollection();

        public FreehandDrawing()
        {

            Preview = new System.Windows.Media.Imaging.BitmapImage();
            Preview.BeginInit();
            Preview.UriSource = new Uri(RelativeToAbsoluteConverter.Convert(@"../../../Img/brushes.png"), UriKind.RelativeOrAbsolute);
            Preview.EndInit();
        }
        public override UIElement Draw()
        {
            var polyline = new Polyline
            {
                Points = Points, 
                Stroke = Configuration?.ColorBrush ?? Brushes.Black,
                StrokeThickness = Configuration?.Thickness ?? 1,
                StrokeDashArray = Configuration?.StrokeDash ?? null
            };

            return polyline;
        }

        public override IShape Clone()
        {
            return new FreehandDrawing
            {
                Points = new PointCollection(Points), 
                Configuration = Configuration?.Clone() as StateShape
            };
        }
    }
}
