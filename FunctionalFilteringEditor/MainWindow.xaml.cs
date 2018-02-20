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

        public MainWindow()
        {
            InitializeComponent();

            this.dragger = new Dragger(canvas);
            dragger.DragUpdated += (sender, args) => { UpdateGraph(); };

            ConvertImageToGrayScaleImage();

            drawAxis();
            drawInitialGraphPoints();
            UpdateGraph();
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
            var graphPoint1 = GraphPoint.Create(0, 20, false);
            var graphPoint2 = GraphPoint.Create(255, 40, false);

            placeEllipseElement(graphPoint1.UIElement, 0, 20);
            placeEllipseElement(graphPoint2.UIElement, 255, 40);

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

        private void ConvertImageToGrayScaleImage()
        {
            Image grayImage = new Image();
            BitmapImage bmpImage = new BitmapImage();
            bmpImage.BeginInit();
            bmpImage.UriSource = new Uri(@"C:\Users\Korzonkie\Desktop\Lenna.png", UriKind.RelativeOrAbsolute);
            bmpImage.EndInit();
            FormatConvertedBitmap grayBitmap = new FormatConvertedBitmap();
            grayBitmap.BeginInit();
            grayBitmap.Source = bmpImage;
            grayBitmap.DestinationFormat = PixelFormats.Gray8;
            grayBitmap.EndInit();

            grayImage.Source = grayBitmap;
            imageContainer.Children.Add(grayImage);
        }
    }
}
