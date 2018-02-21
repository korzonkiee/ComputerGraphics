using System;
using System.Collections.Generic;
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
using Color = System.Drawing.Color;
using Point = System.Windows.Point;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace FunctionalFilteringEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dragger dragger;

        private Line xAxis;
        private Line yAxis;

        private List<GraphPoint> graphPoints = new List<GraphPoint>();
        private List<Line> graphLines = new List<Line>();

        private byte[] colorOuputFunction = new byte[256];

        Bitmap gImage;
        Image image = new Image();

        public MainWindow()
        {
            InitializeComponent();

            gImage = LoadGrayScaleImage();
            ShowGrayScaleImage(gImage);

            this.dragger = new Dragger(canvas);
            dragger.DragUpdated += (sender, args) =>
            {
                UpdateGraph();
                RecalculateColorOutputFunction();
            };

            drawAxis();
            drawInitialGraphPoints();
            UpdateGraph();
            RecalculateColorOutputFunction();
            UpdateImage();
        }

        private Bitmap LoadGrayScaleImage()
        {
            Bitmap bmap = new Bitmap(@"C:\Users\Korzonkie\Desktop\Lenna.png");
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    int pix = (int)(0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B);
                    bmap.SetPixel(i, j, Color.FromArgb(pix, pix, pix));
                }
            }
            return bmap;
        }

        private void ShowGrayScaleImage(Bitmap bitmap)
        {
            imageContainer.Children.Clear();
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                image.Source = bitmapImage;
                imageContainer.Children.Add(image);
            }
        }

        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvas.ContextMenu = null;

            Point p = Mouse.GetPosition(canvas);
            var copiedCollection = new List<GraphPoint>(graphPoints);
            foreach (var gp in copiedCollection)
            {
                if (gp.CanRemove && p.X <= gp.PositionX + GraphConsts.PointPadding && p.X >= gp.PositionX - GraphConsts.PointPadding &&
                    p.Y <= gp.PositionY + GraphConsts.PointPadding && p.Y >= gp.PositionY - GraphConsts.PointPadding)
                {
                    var removePointMenuItem = new MenuItem();
                    removePointMenuItem.Header = "Remove point";
                    var removeContextMenu = new ContextMenu();
                    removeContextMenu.Closed += (s, ee) =>
                    {
                        canvas.ContextMenu = null;
                    };

                    removePointMenuItem.Click += (s, ee) =>
                    {
                        canvas.Children.Remove(gp.UIElement);
                        graphPoints.Remove(gp);
                        UpdateGraph();
                        RecalculateColorOutputFunction();
                    };
                    removeContextMenu.Items.Add(removePointMenuItem);
                    canvas.ContextMenu = removeContextMenu;

                    return;
                }
            }

            var addPointMenuItem = new MenuItem();
            addPointMenuItem.Header = "Add point";
            var addContextMenu = new ContextMenu();
            addContextMenu.Closed += (s, ee) =>
            {
                canvas.ContextMenu = null;
            };

            addPointMenuItem.Click += (s, ee) =>
            {
                var graphPoint = GraphPoint.Create(p.X, p.Y);
                graphPoints.Add(graphPoint);

                dragger.EnableDraggingOnElement(graphPoint, graphPoints);
                placeEllipseElement(graphPoint.UIElement, p.X, p.Y);
                canvas.Children.Add(graphPoint.UIElement);

                UpdateGraph();
                RecalculateColorOutputFunction();
            };
            addContextMenu.Items.Add(addPointMenuItem);
            canvas.ContextMenu = addContextMenu;
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = Mouse.GetPosition(canvas);
        }

        private void drawAxis()
        {
            xAxis = new Line();
            yAxis = new Line();

            xAxis.Stroke = Brushes.Black;
            yAxis.Stroke = Brushes.Black;

            xAxis.X1 = 0;
            xAxis.X2 = 255;
            xAxis.Y1 = 0;
            xAxis.Y2 = 0;

            yAxis.X1 = 0;
            yAxis.X2 = 0;
            yAxis.Y1 = 0;
            yAxis.Y2 = 255;

            xAxis.StrokeThickness = 3;
            yAxis.StrokeThickness = 3;

            canvas.Children.Add(xAxis);
            canvas.Children.Add(yAxis);
        }

        private void drawInitialGraphPoints()
        {
            var graphPoint1 = GraphPoint.Create(0, 0, false);
            var graphPoint2 = GraphPoint.Create(255, 255, false);

            placeEllipseElement(graphPoint1.UIElement, 0, 0);
            placeEllipseElement(graphPoint2.UIElement, 255, 255);

            canvas.Children.Add(graphPoint1.UIElement);
            canvas.Children.Add(graphPoint2.UIElement);

            graphPoints.Add(graphPoint1);
            graphPoints.Add(graphPoint2);
        }

        private void placeEllipseElement(Ellipse ellipse, double x, double y)
        {
            Canvas.SetLeft(ellipse, x - ellipse.Width / 2);
            Canvas.SetTop(ellipse, y - ellipse.Height / 2);
        }

        private void UpdateGraph()
        {
            foreach (var line in graphLines)
            {
                canvas.Children.Remove(line);
            }

            graphLines.Clear();

            graphPoints.Sort(new GraphPointComparer());

            for (int i = 0; i < graphPoints.Count - 1; i++)
            {
                var gp = graphPoints[i];
                var ngp = graphPoints[i + 1];

                var line = new Line();
                line.Stroke = Brushes.Red;
                line.X1 = gp.PositionX;
                line.X2 = ngp.PositionX;
                line.Y1 = gp.PositionY;
                line.Y2 = ngp.PositionY;
                line.StrokeThickness = 1;

                canvas.Children.Add(line);
                graphLines.Add(line);
            }
        }

        private void RecalculateColorOutputFunction()
        {
            for (int i = 0; i < graphPoints.Count - 1; i++)
            {
                var gp1 = graphPoints[i];
                var gp2 = graphPoints[i + 1];

                var a = (gp2.PositionY - gp1.PositionY) / (gp2.PositionX - gp1.PositionX);
                var b = gp1.PositionY - a * gp1.PositionX;

                for (int j = (int) gp1.PositionX; j < (int) gp2.PositionX; j++)
                {
                    colorOuputFunction[j] = (byte)(a * j + b);
                }
            }
        }

        private void UpdateImage()
        {
            Color c;
            Bitmap bitmap = (Bitmap)gImage.Clone();
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    c = bitmap.GetPixel(i, j);
                    int pix = colorOuputFunction[c.R];
                    bitmap.SetPixel(i, j, Color.FromArgb(pix, pix, pix));
                }
            }

            ShowGrayScaleImage(bitmap);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateImage();
        }
    }
}
