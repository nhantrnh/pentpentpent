using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Contract
{
    public class StateShape : ICloneable
    {
        public bool IsDrawn { get; set; }
        public double Thickness { get; set; }
        public SolidColorBrush? ColorBrush { get; set; }
        public SolidColorBrush ColorStroke { get; set; }
        public DoubleCollection? StrokeDash { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public string Choice { get; set; } //Line
        public List<IShape> Shapes { get; set; }
        public List<IShape> Buffer { get; set; }
        public PointCollection Points { get; set; } = new PointCollection(); // Change the type to PointCollection

        public bool IsMoved { get; set; } 
        public StateShape()
        {
            IsDrawn = false;
            Thickness = 1;
            ColorBrush = null;
            Choice = String.Empty;
            ColorStroke = Brushes.Black;
            Shapes = [];
            Buffer = [];
            IsMoved = false;
        }
        public object Clone()
        {
            return (StateShape)MemberwiseClone();
        }
    }
}
