using Contract;
using FontAwesome.Sharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ProjectPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();

        }
        ShapeFactory _factory = new ShapeFactory();
        StateShape _stateShape = new();
        bool _isSaved = false;
        string _drawPath = string.Empty;
        bool _isUndo = false;
        string filePath = "shapeList.json";
        private bool _isEditMode = false;

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

            };
            if (File.Exists(filePath))
            {
                string[] content = File.ReadAllLines(filePath);
                if (content.Length > 0)
                {
                    string json = content[0];
                    string background = (content.Length > 1) ? content[1] : string.Empty;

                    _drawPath = background;

                    drawingArea.Children.Clear();
                    string tmp = _stateShape.Choice.ToString();
                    _stateShape = new StateShape();
                    _stateShape.Choice = tmp;

                    List<IShape> containers = JsonConvert.DeserializeObject<List<IShape>>(json, settings);

                    foreach (var item in containers)
                    {
                        _stateShape.Shapes.Add(item);
                        drawingArea.Children.Add(item.Draw());
                    }

                    if (!string.IsNullOrEmpty(_drawPath))
                    {
                        ImageBrush brush = new ImageBrush();
                        brush.ImageSource = new BitmapImage(new Uri(_drawPath, UriKind.Absolute));
                        drawingArea.Background = brush;
                    }
                }
            }       
            var abilities = new List<IShape>();

            // Do tim cac kha nang
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            var fis = (new DirectoryInfo(folder)).GetFiles("*.dll");

            foreach (var fi in fis)
            {
                var assembly = Assembly.LoadFrom(fi.FullName);
                var types = assembly.GetTypes();

                foreach (var type in types)
                {
                    if (type.IsClass & (!type.IsAbstract))
                    {
                        if (typeof(IShape).IsAssignableFrom(type))
                        {
                            var shape = Activator.CreateInstance(type) as IShape;
                            abilities.Add(shape!);
                        }
                    }
                }
            }

            _factory = new ShapeFactory();
            foreach (var ability in abilities)
            {
                _factory.Prototypes.Add(ability.Name, ability);

                var button = new Fluent.Button()
                {
                    Width = 55,
                    Height = 40,
                    Icon = ability.Preview,
                    Tag = ability.Name
                };
                button.Click += (sender, args) =>
                {
                    var control = (Button)sender;
                    _stateShape.Choice = (string)control.Tag;
                };
                iconListView.Items.Add(button);
            };

            if (abilities.Count > 0)
            {
                _stateShape.Choice = abilities[0].Name;
            }
        }
        
        private void RibbonWindow_Closed(object sender, EventArgs e)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new FilteredContractResolver() // Sử dụng ContractResolver tùy chỉnh
            };
            var serializedShapes = JsonConvert.SerializeObject(_stateShape.Shapes, settings);

            File.WriteAllText(filePath, serializedShapes);
        }

        private Shape _selectedShape = null;

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {            
            if (e.LeftButton == MouseButtonState.Pressed && _stateShape.Choice == "Brushes")
            {
                Point position = e.GetPosition(drawingArea);

                FreehandDrawing freehandDrawing = new FreehandDrawing();
                freehandDrawing.Points.Add(position);

                freehandDrawing.Configuration = _stateShape.Clone() as StateShape;

                _stateShape.Shapes.Add(freehandDrawing); 
                drawingArea.Children.Add(freehandDrawing.Draw());
            }
            _stateShape.IsDrawn = true;
            _stateShape.Start = e.GetPosition(drawingArea);
            drawingArea.Children.Add(new UIElement());
            _isUndo = false;
            _stateShape.Buffer = [];
            ChangeIconColor(redoButton, Brushes.Gray);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (e.LeftButton == MouseButtonState.Pressed && _stateShape.Choice == "Brushes")
            {
                Point position = e.GetPosition(drawingArea); 

                FreehandDrawing freehandDrawing = _stateShape.Shapes.LastOrDefault() as FreehandDrawing;

                if (freehandDrawing != null)
                {
                    freehandDrawing.Points.Add(position);
                }
            }
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_stateShape.IsDrawn)
                {
                    _stateShape.End = e.GetPosition(drawingArea);
                    Title = $"{_stateShape.Start.X}, {_stateShape.Start.Y} => {_stateShape.End.X}, {_stateShape.End.Y}";
                    IShape preview = _factory.Create(_stateShape.Choice);

                    preview.Points.Add(_stateShape.Start);
                    preview.Points.Add(_stateShape.End);
                    preview.Configuration = (StateShape)_stateShape.Clone();

                    drawingArea.Children.RemoveAt(drawingArea.Children.Count - 1);
                    drawingArea.Children.Add(preview.Draw());

                    _stateShape.IsMoved = true;
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_stateShape.IsMoved)
            {
                IShape shape = _factory.Create(_stateShape.Choice);
                shape.Points.Add(_stateShape.Start);
                shape.Points.Add(_stateShape.End);
                shape.Configuration = (StateShape)_stateShape.Clone();
                _stateShape.Shapes.Add(shape);
                _stateShape.IsDrawn = false;
                drawingArea.Children.Add(shape.Draw());
                _isSaved = false;
                ChangeIconColor(undoButton, Brushes.CornflowerBlue);
                _stateShape.IsMoved = false;
            }
        }
        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            if (_stateShape.Shapes.Count == 0 || drawingArea.Children.Count == 0)
            {
                return;
            }
            if (!_isSaved)
            {
                var result = MessageBox.Show("Do you want to save changes?", "Paint", MessageBoxButton.YesNoCancel);
                if (MessageBoxResult.Yes == result)
                {
                    SaveJosn();
                }
                else if (MessageBoxResult.No == result)
                {
                    drawingArea.Background = Brushes.White;
                    drawingArea.Children.Clear();
                    _stateShape.Shapes.Clear();
                    ChangeIconColor(undoButton, Brushes.Gray);
                    ChangeIconColor(undoButton, Brushes.Gray);
                    _isUndo = false;
                }
                else if (MessageBoxResult.Cancel == result)
                {
                    return;
                }
            }
            else
            {
                drawingArea.Background = Brushes.White;
                drawingArea.Children.Clear();
                _stateShape.Shapes.Clear();
                ChangeIconColor(undoButton, Brushes.Gray);
                ChangeIconColor(redoButton, Brushes.Gray);
                _isUndo = false;
            }
        }

        private void OpenJSon()
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "JSON (*.json)|*.json",
            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // or ReferenceLoopHandling.Serialize

                };

                string[] content = File.ReadAllLines(openFileDialog.FileName);
                string json = content[0];
                string background = (content.Length > 1) ? content[1] : string.Empty;

                string tmp = _stateShape.Choice.ToString();
                _stateShape = new StateShape();
                _stateShape.Choice = tmp;
                _drawPath = background;

                drawingArea.Children.Clear();

                List<IShape> containers = JsonConvert.DeserializeObject<List<IShape>>(json, settings);

                foreach (var item in containers)
                {
                    _stateShape.Shapes.Add(item);
                }

                if (!string.IsNullOrEmpty(_drawPath))
                {
                    ImageBrush brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri(_drawPath, UriKind.Absolute));
                    drawingArea.Background = brush;
                }
            }
            foreach (var shape in _stateShape.Shapes)
            {
                drawingArea.Children.Add(shape.Draw());
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isSaved == false)
            {
                var result = MessageBox.Show("Do you want to save changes?", "Paint", MessageBoxButton.YesNoCancel);

                if (MessageBoxResult.Yes == result)
                {
                    SaveJosn();
                }
                else if (MessageBoxResult.No == result || MessageBoxResult.Cancel == result)
                {
                    drawingArea.Children.Clear();
                    _stateShape.Shapes.Clear();
                    ChangeIconColor(undoButton, Brushes.Gray);
                    ChangeIconColor(redoButton, Brushes.Gray);
                    _isUndo = false;
                    OpenJSon();
                }
            }
            else
            {
                drawingArea.Children.Clear();
                _stateShape.Shapes.Clear();
                ChangeIconColor(undoButton, Brushes.Gray);
                ChangeIconColor(redoButton, Brushes.Gray);
                _isUndo = false;
                OpenJSon();
            }
        }

        private void SaveJosn()
        {
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog()
            {
                Filter = "JSON (*.json)|*.json",
            };
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new FilteredContractResolver() // Sử dụng ContractResolver tùy chỉnh
            };

            var serializedShapes = JsonConvert.SerializeObject(_stateShape.Shapes, settings);

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, serializedShapes);
                _isSaved = true;
            }
            
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveJosn();
        }

        public class FilteredContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var properties = base.CreateProperties(type, memberSerialization);

                // Loại bỏ thuộc tính Preview khi serialize
                properties = properties.Where(p => p.PropertyName != "Preview").ToList();

                return properties;
            }
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog()
            {
                Filter = "PNG (*.png)|*.png",
            };

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)drawingArea.Width, (int)drawingArea.Height, 100, 100, PixelFormats.Pbgra32);

                drawingArea.Measure(new Size((int)drawingArea.Width, (int)drawingArea.Height));
                drawingArea.Arrange(new Rect(new Size((int)drawingArea.Width, (int)drawingArea.Height)));

                renderBitmap.Render(drawingArea);

                PngBitmapEncoder img = new PngBitmapEncoder();
                img.Frames.Add(BitmapFrame.Create(renderBitmap));

                using (FileStream file = File.Create(saveFileDialog.FileName))
                {
                    img.Save(file);
                }
            }
            _isSaved = true;
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "PNG (*.png)|*.png",

            };

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _drawPath = openFileDialog.FileName;

                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                drawingArea.Background = brush;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isSaved == false)
            {
                SaveJosn();
            }
            this.Close();
        }

        private void EditMode_Click(object sender, RoutedEventArgs e)
        {            
            if (_isEditMode)
            {
                EditMode.Header = "Edit";
            }
                
            else EditMode.Header = "Draw";
        }
        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
        }
        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
        }
        private void ChangeIconColor(Fluent.Button button, SolidColorBrush color)
        {
            var iconImage = button.LargeIcon as FontAwesome.Sharp.IconImage;
            if (iconImage != null)
            {
                iconImage.Foreground = color;
            }
        }
        private void Redraw()
        {
            drawingArea.Children.Clear();
            foreach (var tmp in _stateShape.Shapes)
            {
                drawingArea.Children.Add(tmp.Draw());
            }
            if (_stateShape.Shapes.Count == 0)
            {
                ChangeIconColor(undoButton, Brushes.Gray);
            }
            if (_isUndo)
            {
                ChangeIconColor(redoButton, Brushes.CornflowerBlue);
            }
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_stateShape.Shapes.Count == 0)
            {
                return;
            }
            var index = _stateShape.Shapes.Count - 1;
            _stateShape.Buffer.Add(_stateShape.Shapes[index]);
            _stateShape.Shapes.RemoveAt(index);
            _isUndo = true;
            ChangeIconColor(undoButton, Brushes.CornflowerBlue);
            Redraw();
        }
        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_stateShape.Buffer.Count == 0)
            {
                return;
            }
            if (_isUndo)
            {
                var index = _stateShape.Buffer.Count - 1;
                _stateShape.Shapes.Add(_stateShape.Buffer[index]);
                _stateShape.Buffer.RemoveAt(index);

                Redraw();
                if (_stateShape.Buffer.Count == 0)
                {
                    _isUndo = false;
                    ChangeIconColor(redoButton, Brushes.Gray);
                    ChangeIconColor(undoButton, Brushes.CornflowerBlue);
                }
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void PencilButton_Click(object sender, RoutedEventArgs e)
        {


        }
        private readonly ScaleTransform scaleTransform = new ScaleTransform();       

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            if (zoomSlider.Value < 8 && zoomSlider.Value >= 4)
            {
                zoomSlider.Value += 1;
            }
            else
            {
                zoomSlider.Value += zoomSlider.Value; 
            }

        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (zoomSlider.Value < 8 && zoomSlider.Value >= 4)
            {
                zoomSlider.Value -= 1;
            }
            else
            {
                zoomSlider.Value -= zoomSlider.Value / 2;
            }

        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double zoomValue = e.NewValue;
            ScaleTransform scale = new ScaleTransform(zoomValue, zoomValue);
            drawingArea.LayoutTransform = scale;
        }
        private void DrawingArea_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                double zoomValue = zoomSlider.Value;

                if (e.Delta > 0)
                {
                    zoomValue += 0.1;
                }
                else
                {
                    zoomValue -= 0.1;
                }

                zoomSlider.Value = zoomValue;
            }
        }

        private void DrawGridLines()
        {

        }

        private void FillColorButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void Icon_Click(object sender, RoutedEventArgs e)
        {

        }
        private void FontButton_Click(object sender, RoutedEventArgs e)
        {
        }
        private void SizeChangedButton(object sender, SelectionChangedEventArgs e)
        {
            var index = sizeChangeComboBox.SelectedIndex;
            _stateShape.Thickness = (double)(index + 1);
        }

        private void FullChangedButton(object sender, EventArgs e)
        {
            int index = fullColorCombox.SelectedIndex;
            switch (index)
            {
                case 0:
                    _stateShape.ColorBrush = null;
                    break;
                case 1:
                    _stateShape.ColorBrush = _stateShape.ColorStroke;
                    break;
                default:
                    break;
            }
        }

        private void DashChangedButton(object sender, EventArgs e)
        {
            int index = dashChangeComboBox.SelectedIndex;
            switch (index)
            {
                case 0:
                    _stateShape.StrokeDash = null;
                    break;
                case 1:
                    _stateShape.StrokeDash = [1];
                    break;
                case 2:
                    _stateShape.StrokeDash = [6, 1];
                    break;
                case 3:
                    _stateShape.StrokeDash = [4, 1, 1, 1, 1, 1];
                    break;
                default:
                    break;
            }
        }

        private void BlackButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush(Colors.Black);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void GrayButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.Gray);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void BrownButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.Brown);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void RedButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.Red);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void OrangeButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.Orange);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void YellowButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush(Colors.Yellow);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void GreenButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.Green);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void DodgerblueButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.DodgerBlue);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void BlueButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.Blue);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void PurpleButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.Purple);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void WhiteButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.White);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void LightGrayButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.LightGray);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void PERUButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.Peru);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void PinkButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.Pink);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void GoldButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.Gold);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void WHEATButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.Wheat);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void LawnGreenButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.LawnGreen);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void PowderBlueButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.PowderBlue);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void CornflowerBlueButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.CornflowerBlue);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void MEDIUMPURPLEButton_Click(object sender, RoutedEventArgs e)
        {
            _stateShape.ColorStroke = new SolidColorBrush((Color)Colors.MediumPurple);
            if (_stateShape.ColorBrush != null)
            {
                _stateShape.ColorBrush = _stateShape.ColorStroke;
            }
        }

        private void EditColorButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorEdit = new System.Windows.Forms.ColorDialog();

            if (colorEdit.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _stateShape.ColorStroke = new SolidColorBrush(Color.FromArgb(colorEdit.Color.A, colorEdit.Color.R, colorEdit.Color.G, colorEdit.Color.B));
            }
        }
    }
}
