using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FunctionalFilteringEditor
{
    public class Dragger
    {
        private readonly Canvas canvas;

        public delegate void EventHandler(object sender, EventArgs args);
        public event EventHandler DragUpdated = delegate { };

        public Dragger(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void EnableDraggingOnElement(GraphPoint graphPoint, List<GraphPoint> graphPoints)
        {
            Nullable<Point> dragStart = null;

            MouseButtonEventHandler mouseDown = (sender, args) =>
            {
                var element = (UIElement)sender;
                dragStart = args.GetPosition(element);
                element.CaptureMouse();
            };
            MouseButtonEventHandler mouseUp = (sender, args) =>
            {
                var element = (UIElement)sender;
                dragStart = null;
                element.ReleaseMouseCapture();
            };
            MouseEventHandler mouseMove = (sender, args) =>
            {
                if (dragStart != null && args.LeftButton == MouseButtonState.Pressed)
                {
                    var element = (UIElement)sender;
                    var p2 = args.GetPosition(canvas);

                    var newX = p2.X - dragStart.Value.X;
                    var newY = p2.Y - dragStart.Value.Y;

                    if (newX <= 0 || newX >= 255 || newY <= 0 || newY >= 255)
                        return;

                    graphPoint.PositionX = newX;
                    graphPoint.PositionY = newY;

                    Canvas.SetLeft(element, newX);
                    Canvas.SetTop(element, newY);

                    DragUpdated(this, null);
                }
            };

            graphPoint.UIElement.MouseDown += mouseDown;
            graphPoint.UIElement.MouseMove += mouseMove;
            graphPoint.UIElement.MouseUp += mouseUp;
        }
    }
}
