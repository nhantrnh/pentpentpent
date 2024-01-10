using Contract;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LineLib
{
    public class LineShape : IShape
    {
        public override string Name => "line";
        public LineShape() {
            Preview = new System.Windows.Media.Imaging.BitmapImage();
            Preview.BeginInit();
            Preview.UriSource = new Uri(RelativeToAbsoluteConverter.Convert(@"../../../Img/line.png"), UriKind.RelativeOrAbsolute);
            Preview.EndInit();
        }
        public override UIElement Draw()
        {
            return new Line()
            {
                X1 = Points[0].X,
                Y1 = Points[0].Y,
                X2 = Points[1].X,
                Y2 = Points[1].Y,
                StrokeThickness = Configuration == null ? 1.0 : Configuration.Thickness,
                //Icon = String.Empty;
                Fill = Configuration?.ColorBrush,
                Stroke = Configuration?.ColorStroke,
                StrokeDashArray = Configuration?.StrokeDash,
            };
        }
        public override IShape Clone()
        {
            return new LineShape();
        }
    }

}
