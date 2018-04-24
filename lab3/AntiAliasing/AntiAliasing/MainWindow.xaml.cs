using AntiAliasing.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Point = System.Windows.Point;
using Mouse = AntiAliasing.Extensions.Mouse;

namespace AntiAliasing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Canvas canvas;

        private bool isDrawingLine = false;
        private Point drawingLineStart;

        Point currentPoint = new Point();

        public MainWindow()
        {
            InitializeComponent();

            this.canvas = new Canvas(CanvasImage);
            Container.MouseRightButtonDown += Container_RightClick;
            Container.MouseMove += OnMouseMove;
            Container.MouseDown += OnMouseDown;
        }


        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                currentPoint = Mouse.GetMousePosition(CanvasImage);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var to = Mouse.GetMousePosition(CanvasImage);
                canvas.DrawLine((int) currentPoint.X, (int) currentPoint.Y, (int) to.X, (int) to.Y);

                currentPoint = to;
            }
        }

        private void Container_RightClick(object sender, MouseEventArgs e)
        {
            Container.ContextMenu = null;

            var p = Mouse.GetMousePosition(CanvasImage);

            var drawContextMenu = new ContextMenu();
            var drawLine = new MenuItem();
            var drawCircle = new MenuItem();

            if (!isDrawingLine)
            {
                drawLine.Header = "Start line";
                drawLine.Click += (s, ee) =>
                {
                    drawingLineStart = p;

                    isDrawingLine = true;
                };
            }
            else
            {
                drawCircle.IsEnabled = false;

                drawLine.Header = "End line";

                drawLine.Click += (s, ee) =>
                {
                    canvas.DrawLine((int)drawingLineStart.X, (int)drawingLineStart.Y,
                        (int)p.X, (int)p.Y);

                    isDrawingLine = false;
                };
            }

            drawCircle.Header = "Draw circle";
            drawCircle.Click += (s, ee) =>
            {
                canvas.DrawCircle((int)p.X, (int)p.Y, 50);
            };

            drawContextMenu.Items.Add(drawLine);
            drawContextMenu.Items.Add(new Separator());
            drawContextMenu.Items.Add(drawCircle);

            Container.ContextMenu = drawContextMenu;
        }

        private void MenuItem_ClearClick(object sender, RoutedEventArgs e)
        {
            canvas.Clear();
        }

        private void MenuItem_AntiAliasingChecked(object sender, RoutedEventArgs e)
        {
            canvas.EnableAntiAliasing();
        }

        private void MenuItem_AntiAliasingUnchecked(object sender, RoutedEventArgs e)
        {
            canvas.DisableAntiAliasing();
        }
    }
}
