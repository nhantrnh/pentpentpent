using Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdornerLib
{
    public class RectResize : Adorner
    {
        VisualCollection AdornerVisuals;
        Thumb TopLeft;
        Thumb TopRight;
        Thumb BotRight;
        Thumb Center;
        Thumb Rotate;
        Thumb BotLeft;
        Rectangle thumbRect;
        IShape ishape;
        public RectResize(UIElement adornedElement, IShape ishape) : base(adornedElement)
        {
            this.ishape = ishape;
            AdornerVisuals = new VisualCollection(this);
            TopLeft = new Thumb() { Background = Brushes.Orange, Height = 10, Width = 10 };
            TopRight = new Thumb() { Background = Brushes.Orange, Height = 10, Width = 10 };
            BotRight = new Thumb() { Background = Brushes.Orange, Height = 10, Width = 10 };
            BotLeft = new Thumb() { Background = Brushes.Orange, Height = 10, Width = 10 };
            Rotate = new Thumb() { Background = Brushes.Orange, Height = 10, Width = 10 };
            thumbRect = new Rectangle() { Stroke = Brushes.Orange, StrokeThickness = 2, StrokeDashArray = { 3, 2 } };
            Center = new Thumb() { Background = Brushes.Orange, Height = 10, Width = 10 };

            TopLeft.DragDelta += DragDelta_TopLeft;
            BotRight.DragDelta += DragDelta_BottomRight;
            Center.DragDelta += DragDelta_Center;
            Rotate.DragDelta += DragDelta_Rotate;
            TopRight.DragDelta += DragDelta_TopRight;
            BotLeft.DragDelta += DragDelta_BotLeft;

            AdornerVisuals.Add(Rotate);
            AdornerVisuals.Add(TopRight);
            AdornerVisuals.Add(BotLeft);
            AdornerVisuals.Add(thumbRect);
            AdornerVisuals.Add(TopLeft);
            AdornerVisuals.Add(BotRight);
            AdornerVisuals.Add(Center);
        }

        private void DragDelta_BotLeft(object sender, DragDeltaEventArgs e)
        {
            var deltaTop = e.VerticalChange;
            var deltaLeft = e.HorizontalChange;
            var ele = (FrameworkElement)AdornedElement;

            Point centerPoint = Center.TranslatePoint(new Point(Center.Width / 2, Center.Height / 2), (Canvas)VisualTreeHelper.GetParent(ele));
            double centerX = centerPoint.X;
            double centerY = centerPoint.Y;
            var cx = Canvas.GetLeft(ele) + ele.Width / 2;
            var cy = Canvas.GetTop(ele) + ele.Height / 2;
            var dx = centerX - cx;
            var dy = centerY - cy;

            var currentLeft = Canvas.GetLeft(ele) + ele.Width;

            if (Canvas.GetLeft(ele) + e.HorizontalChange > currentLeft)
            {
                deltaLeft = 0;
            }

            // Lấy góc xoay hiện tại từ RenderTransform của phần tử
            var rotateTransform = ele.RenderTransform as RotateTransform;
            var angleInRadians = rotateTransform != null ? (rotateTransform.Angle * Math.PI / 180.0) : 0.0;

            // Tính toán thay đổi vị trí mới dựa trên góc xoay
            var rotatedDeltaLeft = deltaLeft * Math.Cos(angleInRadians);
            var rotatedDeltaTop = deltaLeft * Math.Sin(angleInRadians);

            Canvas.SetTop(ele, Canvas.GetTop(ele) + rotatedDeltaTop);
            Canvas.SetLeft(ele, Canvas.GetLeft(ele) + rotatedDeltaLeft);

            ele.Height = ele.Height + deltaTop < 0 ? 0 : ele.Height + deltaTop;
            ele.Width = ele.Width - deltaLeft < 0 ? 0 : ele.Width - deltaLeft;

            UpdatePointIshape(new Point() { X = Canvas.GetLeft(ele) + dx, Y = Canvas.GetTop(ele) + dy }, new Point() { X = Canvas.GetLeft(ele) + ele.Width + dx, Y = Canvas.GetTop(ele) + ele.Height + dy });
            this.ishape.centerX = ele.Width / 2;
            this.ishape.centerY = ele.Height / 2;
        }

        private void DragDelta_TopRight(object sender, DragDeltaEventArgs e)
        {
            var deltaTop = e.VerticalChange;
            var deltaLeft = e.HorizontalChange;
            var ele = (FrameworkElement)AdornedElement;

            var currentTop = Canvas.GetTop(ele) + ele.Height;

            if (Canvas.GetTop(ele) + e.VerticalChange > currentTop)
            {
                deltaTop = 0;
            }


            Point centerPoint = Center.TranslatePoint(new Point(Center.Width / 2, Center.Height / 2), (Canvas)VisualTreeHelper.GetParent(ele));

            double centerX = centerPoint.X;
            double centerY = centerPoint.Y;
            var cx = Canvas.GetLeft(ele) + ele.Width / 2;
            var cy = Canvas.GetTop(ele) + ele.Height / 2;
            var dx = centerX - cx;
            var dy = centerY - cy;



            // Lấy góc xoay hiện tại từ RenderTransform của phần tử
            var rotateTransform = ele.RenderTransform as RotateTransform;
            var angleInRadians = rotateTransform != null ? (rotateTransform.Angle * Math.PI / 180.0) : 0.0;

            // Tính toán thay đổi vị trí mới dựa trên góc xoay
            var rotatedDeltaLeft = -deltaTop * Math.Sin(angleInRadians);
            var rotatedDeltaTop = deltaTop * Math.Cos(angleInRadians);

            Canvas.SetTop(ele, Canvas.GetTop(ele) + rotatedDeltaTop);
            Canvas.SetLeft(ele, Canvas.GetLeft(ele) + (angleInRadians == 0 ? 0 : rotatedDeltaLeft));
            ele.Height = ele.Height - deltaTop < 0 ? 0 : ele.Height - deltaTop;
            ele.Width = ele.Width + deltaLeft < 0 ? 0 : ele.Width + deltaLeft;

            UpdatePointIshape(new Point() { X = Canvas.GetLeft(ele) + dx, Y = Canvas.GetTop(ele) + dy }, new Point() { X = Canvas.GetLeft(ele) + ele.Width + dx, Y = Canvas.GetTop(ele) + ele.Height + dy });
            this.ishape.centerX = ele.Width / 2;
            this.ishape.centerY = ele.Height / 2;
        }

        private void DragDelta_Rotate(object sender, DragDeltaEventArgs e)
        {
            if (this.ishape.Points != null)
            {
                var ele = (FrameworkElement)AdornedElement;
                Canvas.SetLeft(ele, this.ishape.Points[0].X);
                Canvas.SetTop(ele, this.ishape.Points[0].Y);
                double newAngle = this.ishape.Angle + e.HorizontalChange / 5;
                ele.RenderTransform = new RotateTransform(newAngle, this.ishape.centerX, this.ishape.centerY);
                this.ishape.Angle = newAngle;
            }
        }


        protected void UpdatePointIshape(Point a, Point b)
        {
            if (this.ishape.Points != null)
            {
                ishape.Points[0] = a;
                ishape.Points[1] = b;
            }
        }
        protected override Visual GetVisualChild(int index)
        {
            return AdornerVisuals[index];
        }

        protected override int VisualChildrenCount => AdornerVisuals.Count;

        protected override Size ArrangeOverride(Size finalSize)
        {
            TopLeft.Arrange(new Rect(-10, -10, 10, 10));
            TopRight.Arrange(new Rect(AdornedElement.DesiredSize.Width, -10, 10, 10));
            BotRight.Arrange(new Rect(AdornedElement.DesiredSize.Width, AdornedElement.DesiredSize.Height, 10, 10));
            BotLeft.Arrange(new Rect(-10, AdornedElement.DesiredSize.Height, 10, 10));
            thumbRect.Arrange(new Rect(-5, -5, AdornedElement.DesiredSize.Width + 10, AdornedElement.DesiredSize.Height + 10));
            Center.Arrange(new Rect(0, 0, AdornedElement.DesiredSize.Width, AdornedElement.DesiredSize.Height));
            Rotate.Arrange(new Rect(0, -AdornedElement.DesiredSize.Height / 2 - 20, AdornedElement.DesiredSize.Width, AdornedElement.DesiredSize.Height));

            return base.ArrangeOverride(finalSize);
        }
        private void DragDelta_Center(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;
            var deltaTop = e.VerticalChange;
            var deltaLeft = e.HorizontalChange;

            // Lấy góc xoay hiện tại từ RenderTransform của phần tử
            var rotateTransform = ele.RenderTransform as RotateTransform;
            var angleInRadians = rotateTransform != null ? (rotateTransform.Angle * Math.PI / 180.0) : 0.0;

            // Tính toán thay đổi vị trí mới dựa trên góc xoay
            var rotatedDeltaLeft = deltaLeft * Math.Cos(angleInRadians) - deltaTop * Math.Sin(angleInRadians);
            var rotatedDeltaTop = deltaLeft * Math.Sin(angleInRadians) + deltaTop * Math.Cos(angleInRadians);

            Canvas.SetTop(ele, rotatedDeltaTop + Canvas.GetTop(ele));
            Canvas.SetLeft(ele, rotatedDeltaLeft + Canvas.GetLeft(ele));
            UpdatePointIshape(new Point() { X = Canvas.GetLeft(ele), Y = Canvas.GetTop(ele) }, new Point() { X = Canvas.GetLeft(ele) + ele.Width, Y = Canvas.GetTop(ele) + ele.Height });
        }


        private void DragDelta_BottomRight(object sender, DragDeltaEventArgs e)
        {
            var deltaTop = e.VerticalChange;
            var deltaLeft = e.HorizontalChange;
            var ele = (FrameworkElement)AdornedElement;
            ele.Height = ele.Height + deltaTop < 0 ? 0 : ele.Height + deltaTop;
            ele.Width = ele.Width + deltaLeft < 0 ? 0 : ele.Width + deltaLeft;
            Point centerPoint = Center.TranslatePoint(new Point(Center.Width / 2, Center.Height / 2), (Canvas)VisualTreeHelper.GetParent(ele));
            double centerX = centerPoint.X;
            double centerY = centerPoint.Y;
            var cx = Canvas.GetLeft(ele) + ele.Width / 2;
            var cy = Canvas.GetTop(ele) + ele.Height / 2;
            var dx = centerX - cx;
            var dy = centerY - cy;
            UpdatePointIshape(new Point() { X = Canvas.GetLeft(ele) + dx, Y = Canvas.GetTop(ele) + dy }, new Point() { X = Canvas.GetLeft(ele) + ele.Width + dx, Y = Canvas.GetTop(ele) + ele.Height + dy });
            this.ishape.centerX = ele.Width / 2;
            this.ishape.centerY = ele.Height / 2;
        }

        private void DragDelta_TopLeft(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;
            var deltaTop = e.VerticalChange;
            var deltaLeft = e.HorizontalChange;
            var currentTop = Canvas.GetTop(ele) + ele.Height;
            var currentLeft = Canvas.GetLeft(ele) + ele.Width;

            if (Canvas.GetTop(ele) + e.VerticalChange > currentTop)
            {
                deltaTop = 0;
            }
            if (Canvas.GetLeft(ele) + e.HorizontalChange > currentLeft)
            {
                deltaLeft = 0;
            }

            // Lấy góc xoay hiện tại từ RenderTransform của phần tử
            var rotateTransform = ele.RenderTransform as RotateTransform;
            var angleInRadians = rotateTransform != null ? (rotateTransform.Angle * Math.PI / 180.0) : 0.0;

            // Tính toán thay đổi vị trí mới dựa trên góc xoay
            var rotatedDeltaLeft = deltaLeft * Math.Cos(angleInRadians) - deltaTop * Math.Sin(angleInRadians);
            var rotatedDeltaTop = deltaLeft * Math.Sin(angleInRadians) + deltaTop * Math.Cos(angleInRadians);

            Canvas.SetTop(ele, rotatedDeltaTop + Canvas.GetTop(ele));
            Canvas.SetLeft(ele, rotatedDeltaLeft + Canvas.GetLeft(ele));
            ele.Height = ele.Height - deltaTop < 0 ? 0 : ele.Height - deltaTop;
            ele.Width = ele.Width - deltaLeft < 0 ? 0 : ele.Width - deltaLeft;


            Point centerPoint = Center.TranslatePoint(new Point(Center.Width / 2, Center.Height / 2), (Canvas)VisualTreeHelper.GetParent(ele));

            double centerX = centerPoint.X;
            double centerY = centerPoint.Y;
            var cx = Canvas.GetLeft(ele) + ele.Width / 2;
            var cy = Canvas.GetTop(ele) + ele.Height / 2;

            var dx = centerX - cx;
            var dy = centerY - cy;

            UpdatePointIshape(new Point() { X = Canvas.GetLeft(ele) + dx, Y = Canvas.GetTop(ele) + dy }, new Point() { X = Canvas.GetLeft(ele) + ele.Width + dx, Y = Canvas.GetTop(ele) + ele.Height + dy });
            this.ishape.centerX = ele.Width / 2;
            this.ishape.centerY = ele.Height / 2;


        }
        
        Thumb thumb1;
        Thumb thumb2;
       
        private void DragDelta_Thumb2(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;
            ele.Height = ele.Height + e.VerticalChange < 0 ? 0 : ele.Height + e.VerticalChange;
            ele.Width = ele.Width + e.HorizontalChange < 0 ? 0 : ele.Width + e.HorizontalChange;
        }

        private void DragDelta_Thumb1(object sender, DragDeltaEventArgs e)
        {
            var ele = (FrameworkElement)AdornedElement;
            var deltaTop = e.VerticalChange;
            var deltaLeft = e.HorizontalChange;
            var currentTop = Canvas.GetTop(ele) + ele.Height;
            var currentLeft = Canvas.GetLeft(ele) + ele.Width;
            if (Canvas.GetTop(ele) + e.VerticalChange > currentTop)
            {
                deltaTop = 0;
            }
            if (Canvas.GetLeft(ele) + e.HorizontalChange > currentLeft)
            {
                deltaLeft = 0;
            }

            Canvas.SetTop(ele, deltaTop + Canvas.GetTop(ele));
            Canvas.SetLeft(ele, deltaLeft + Canvas.GetLeft(ele));
            ele.Height = ele.Height - deltaTop < 0 ? 0 : ele.Height - deltaTop;
            ele.Width = ele.Width - deltaLeft < 0 ? 0 : ele.Width - deltaLeft;

        }

    }

}
