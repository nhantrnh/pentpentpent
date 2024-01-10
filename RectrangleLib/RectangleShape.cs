using Contract;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RectangleLib
{
    public class RectangleShape : IShape
    {
        public override string Name => "rectangle";
        public RectangleShape()
        {
            Preview = new System.Windows.Media.Imaging.BitmapImage();
            Preview.BeginInit();
            Preview.UriSource = new Uri(RelativeToAbsoluteConverter.Convert(@"../../../Img/rectangle.png"), UriKind.RelativeOrAbsolute);
            Preview.EndInit();
        }
        public override UIElement Draw()
        {
            int checkEndedPoint = 0;
            if (Points[1].X - Points[0].X > 0)
            {
                checkEndedPoint = 0;
            }
            else checkEndedPoint = 1;

            double width = Math.Abs(Points[1].X - Points[0].X);
            double height = Math.Abs(Points[1].Y - Points[0].Y);

            var element = new Rectangle()
            {
                StrokeThickness = Configuration == null ? 1.0 : Configuration.Thickness,
                //Icon = String.Empty;
                Fill = Configuration?.ColorBrush,
                Stroke = Configuration?.ColorStroke,
                StrokeDashArray = Configuration?.StrokeDash,
                Width = width,
                Height = height,
            };
            Canvas.SetLeft(element, Points[checkEndedPoint].X);
            Canvas.SetTop(element, Points[checkEndedPoint].Y);

            return element;
        }
        public override IShape Clone()
        {
            return new RectangleShape();
        }
    }

}
