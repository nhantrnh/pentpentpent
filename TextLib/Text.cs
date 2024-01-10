using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Contract
{
    public class TextDrawing : IShape
    {
        public string Name => "Text";
        public List<Point> Points { get; set; } = new List<Point>();
        public StateShape? Configuration { get; set; }
        public BitmapImage? Preview { get; set; }
        public TextDrawing()
        {
            Preview = new System.Windows.Media.Imaging.BitmapImage();
            Preview.BeginInit();
            Preview.UriSource = new Uri(RelativeToAbsoluteConverter.Convert(@"../../../Img/text.png"), UriKind.RelativeOrAbsolute);
            Preview.EndInit();
        }
        public UIElement Draw()
        {

            TextBlock textBlock = new TextBlock()
            {
                
                Foreground = Configuration?.ColorBrush ?? Brushes.Black,
                FontSize = 13,
                Text = "",
            };
            return textBlock;
        }

        public IShape Clone()
        {
            return new TextDrawing();
        }
    }

}
