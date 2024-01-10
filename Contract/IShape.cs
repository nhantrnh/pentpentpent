using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Contract
{
    public interface IShape
    {
        string Name { get; }
        List<Point> Points { get; set; }
        UIElement Draw();
        IShape Clone();
        StateShape? Configuration { get; set; }
        BitmapImage? Preview { get; set; }
    }
}
