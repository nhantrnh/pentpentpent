using Contract;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RectangleLib
{
    public class RectangleShape : IShape
    {
        public string Name => "rectangle";
        public List<Point> Points { get; set; } = new List<Point>();
        public StateShape? Configuration { get; set; }
        public BitmapImage? Preview { get; set; }
        public RectangleShape()
        {
            Preview = new System.Windows.Media.Imaging.BitmapImage();
            Preview.BeginInit();
            Preview.UriSource = new Uri(RelativeToAbsoluteConverter.Convert(@"../../../Img/rectangle.png"), UriKind.RelativeOrAbsolute);
            Preview.EndInit();
        }
        public UIElement Draw()
        {
            double _x = 0.0;
            double _y = 0.0;
            double width = Points[1].X - Points[0].X;
            double height = Points[1].Y - Points[0].Y;

            if (width < 0)
            {
                _x = width;
            }
            if (height < 0)
            {
                _y = height;
            }
            
            var element = new Rectangle()
            {
                StrokeThickness = Configuration == null ? 1.0 : Configuration.Thickness,
                //Icon = String.Empty;
                Fill = Configuration?.ColorBrush,
                Stroke = Configuration?.ColorStroke,
                StrokeDashArray = Configuration?.StrokeDash,
                Width = Math.Abs(width),
                Height = Math.Abs(height),
            };
            Canvas.SetLeft(element, Points[0].X + _x);
            Canvas.SetTop(element, Points[0].Y + _y);

            return element;
        }
        public IShape Clone()
        {
            return new RectangleShape();
        }
    }

}
