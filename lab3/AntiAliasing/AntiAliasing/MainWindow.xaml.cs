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
using Microsoft.Win32;

namespace AntiAliasing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Canvas canvas;

        private bool imageLoaded = false;

        private bool isDrawingLine = false;
        private bool isDrawingRect = false;
        private bool isDrawingPolygon = false;

        private bool showFillPolygonMenu = false;

        private List<Point> polygonVertices = new List<Point>();

        private Point drawingLineStart;
        private Point drawingRectStart;

        Point currentPoint = new Point();

        public MainWindow()
        {
            InitializeComponent();

            this.canvas = new Canvas(CanvasImage);
            Container.MouseRightButtonDown += Container_RightClick;
            Container.MouseLeftButtonDown += Container_LeftClick;
            Container.MouseMove += OnMouseMove;
            Container.MouseDown += OnMouseDown;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                currentPoint = Mouse.GetMousePosition(CanvasImage);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawingPolygon)
                return;

            if (e.LeftButton == MouseButtonState.Pressed && !imageLoaded)
            {
                var to = Mouse.GetMousePosition(CanvasImage);
                canvas.DrawLine((int) currentPoint.X, (int) currentPoint.Y, (int) to.X, (int) to.Y);

                currentPoint = to;
            }
        }

        private void Container_LeftClick(object sender, MouseButtonEventArgs e)
        {
            if (isDrawingPolygon)
            {
                var p = Mouse.GetMousePosition(CanvasImage);
                polygonVertices.Add(p);
            }

            if (imageLoaded)
            {
                var p = Mouse.GetMousePosition(CanvasImage);
                canvas.FillNeighbours(p, AntiAliasing.Figures.Colors.Black, AntiAliasing.Figures.Colors.Orange);
            }
        }

        private void Container_RightClick(object sender, MouseEventArgs e)
        {
            Container.ContextMenu = null;

            var p = Mouse.GetMousePosition(CanvasImage);
            
            if (canvas.IsPointInsideSomePolygon(p.X, p.Y))
                showFillPolygonMenu = true;
            else
                showFillPolygonMenu = false;

            var drawContextMenu = new ContextMenu();
            var drawLine = new MenuItem();
            var drawRect = new MenuItem();
            var drawPolygon = new MenuItem();
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
                drawPolygon.IsEnabled = false;
                drawRect.IsEnabled = false;

                drawLine.Header = "End line";

                drawLine.Click += (s, ee) =>
                {
                    canvas.DrawLine((int)drawingLineStart.X, (int)drawingLineStart.Y,
                        (int)p.X, (int)p.Y);

                    isDrawingLine = false;
                };
            }

            if (!isDrawingRect)
            {
                drawRect.Header = "Start clipping rect";
                drawRect.Click += (s, ee) =>
                {
                    drawingRectStart = p;

                    isDrawingRect = true;
                };
            }
            else
            {
                drawLine.IsEnabled = false;
                drawPolygon.IsEnabled = false;
                drawCircle.IsEnabled = false;

                drawRect.Header = "End clipping rect";

                drawRect.Click += (s, ee) =>
                {
                    canvas.DrawClippingRect(
                        (int)Math.Min(drawingRectStart.X, p.X), (int)Math.Min(drawingRectStart.Y, p.Y),
                        (int)Math.Max(drawingRectStart.X, p.X), (int)Math.Max(drawingRectStart.Y, p.Y));

                    isDrawingRect = false;
                };
            }

            if (!isDrawingPolygon)
            {
                drawPolygon.Header = "Start polygon";
                drawPolygon.Click += (s, ee) =>
                {
                    isDrawingPolygon = true;
                    polygonVertices.Add(p);
                };
            }
            else
            {
                drawLine.IsEnabled = false;
                drawRect.IsEnabled = false;
                drawCircle.IsEnabled = false;

                drawPolygon.Header = "End polygon";

                drawPolygon.Click += (s, ee) =>
                {
                    isDrawingPolygon = false;

                    canvas.DrawPolygon(polygonVertices);
                    polygonVertices.Clear();
                };
            }


            drawCircle.Header = "Draw circle";
            drawCircle.Click += (s, ee) =>
            {
                canvas.DrawCircle((int)p.X, (int)p.Y, 50);
            };

            drawContextMenu.Items.Add(drawLine);
            drawContextMenu.Items.Add(drawRect);
            drawContextMenu.Items.Add(drawPolygon);
            drawContextMenu.Items.Add(new Separator());
            drawContextMenu.Items.Add(drawCircle);

            if (showFillPolygonMenu)
            {
                var fillPolygon = new MenuItem();
                fillPolygon.Header = "Fill polygon";
                fillPolygon.Click += (s, ee) =>
                {
                    canvas.FillRectangleUnderCursor();
                };

                drawContextMenu.Items.Add(new Separator());
                drawContextMenu.Items.Add(fillPolygon);
            }

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

        private void Thickness_1(object sender, RoutedEventArgs e)
        {
            canvas.SetLineThickness(1);
        }

        private void Thickness_3(object sender, RoutedEventArgs e)
        {
            canvas.SetLineThickness(3);
        }

        private void Thickness_5(object sender, RoutedEventArgs e)
        {
            canvas.SetLineThickness(5);
        }

        private void Thickness_7(object sender, RoutedEventArgs e)
        {
            canvas.SetLineThickness(7);
        }

        private void SuperSampleMenuItemCheck_Checked(object sender, RoutedEventArgs e)
        {
            canvas.EnableSuperSampling();
        }

        private void SuperSampleMenuItemCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            canvas.DisableSuperSampling();
        }

        private void ClipMenuItem_Click(object sender, RoutedEventArgs e)
        {
            canvas.ClipToRect();
        }

        private void MenuItem_LoadImageClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                Bitmap original = (Bitmap)System.Drawing.Image.FromFile(op.FileName);
                Bitmap resized = new Bitmap(original, new System.Drawing.Size((int) CanvasImage.Width, (int) CanvasImage.Height));
                canvas.RenderImage(resized);
            }

            imageLoaded = true;
        }
    }
}
