using Contract;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace EllipseLib
{
    public class EllipseShape : IShape
    {
        public override string Name => "ellipse";
        
        public EllipseShape()
        {
            
            Preview = new System.Windows.Media.Imaging.BitmapImage();
            Preview.BeginInit();
            Preview.UriSource = new Uri(RelativeToAbsoluteConverter.Convert(@"../../../Img/ellipse.png"), UriKind.RelativeOrAbsolute);
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
            
            var element = new Ellipse()
            {
                StrokeThickness = Configuration == null ? 1.0 : Configuration.Thickness,
                //Icon = String.Empty;
                Fill = Configuration?.ColorBrush,
                Stroke = Configuration?.ColorStroke,
                StrokeDashArray = Configuration?.StrokeDash,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Width = width,
                Height = height,
            };

            Canvas.SetLeft(element, Points[checkEndedPoint].X);
            Canvas.SetTop(element, Points[checkEndedPoint].Y);

            return element;
        }
        public override IShape Clone()
        {
            return new EllipseShape();
        }
    }

}
