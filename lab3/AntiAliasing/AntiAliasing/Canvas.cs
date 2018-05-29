using AntiAliasing.Extensions;
using AntiAliasing.Figures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Image = System.Windows.Controls.Image;
using Rectangle = AntiAliasing.Figures.Rectangle;
using Point = System.Windows.Point;
using Color = AntiAliasing.Figures.Color;
using System.Windows.Media.Imaging;
using System.IO;

namespace AntiAliasing
{
    /// <summary>
    /// Canvas which make use of System.Windows.Controls.Image
    /// as an plane on which System.Drawing.Bitmap is rendered.
    /// </summary>
    public class Canvas
    {
        private readonly Image image;
        private Bitmap bitmap;

        private readonly Renderer renderer;

        private List<Figure> figures = new List<Figure>();

        private ClippingRectangle clippingRect;
        private Polygon lastPolygonUnderCursor;

        private int lineThickness = 1;

        public Canvas(Image image)
        {
            this.image = image;
            this.bitmap = new Bitmap((int)image.Width, (int)image.Height);

            this.renderer = new Renderer(bitmap);
            this.renderer.RenderCompleted += OnRenderCompleted;

            renderer.Clear();
        }

        private void OnRenderCompleted(object sender, Figure figure)
        {
            if (figure != null)
                figures.Add(figure);

            image.UpdateBitmap(bitmap);
        }

        public void Clear()
        {
            figures.Clear();
            renderer.Clear();
        }

        public void SetPixel(int x, int y)
        {
            renderer.SetPixel(x, y, Colors.Black);
        }

        public void DrawCircle(int x, int y, int r)
        {
            var circle = new Circle(x, y, r, Colors.Black);
            renderer.Render(circle);
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            var line = new Line(x1, y1, x2, y2, Colors.Black, lineThickness);
            renderer.Render(line);
        }

        public void DrawClippingRect(int x_min, int y_min, int x_max, int y_max)
        {
            clippingRect = new ClippingRectangle(x_min, y_min, x_max, y_max, Colors.Black, lineThickness);
            renderer.Render(clippingRect);
        }

        public void DrawPolygon(List<Point> vertices)
        {
            var polygon = new Figures.Polygon(vertices, Colors.Black, lineThickness);
            renderer.Render(polygon);
        }

        public bool IsPointInsideSomePolygon(double x, double y)
        {
            lastPolygonUnderCursor = null;

            foreach (var polygon in figures.Where(f => f is Polygon))
            {
                var p = polygon as Polygon;
                if (p.ContainsPoint(x, y))
                {
                    lastPolygonUnderCursor = p;
                    return true;
                }
            }

            return false;
        }

        public void FillRectangleUnderCursor()
        {
            if (lastPolygonUnderCursor != null)
            {
                renderer.Fill(lastPolygonUnderCursor);
            }
        }

        public void ClipToRect()
        {
            if (clippingRect == null)
                return;

            var outsideLines = new List<Line>();
            clippingRect.OutsideLine += (s, e) => { outsideLines.Add(e); };

            foreach (var line in figures.Where(f => f is Line))
            {
                clippingRect.ClipFigure(line);
            }

            figures = figures.Except(outsideLines).ToList();

            Redraw();
        }

        public void EnableAntiAliasing()
        {
            renderer.EnableAntiAliasing();
            Redraw();
        }

        public void DisableAntiAliasing()
        {
            renderer.DisableAntiAliasing();
            Redraw();
        }

        public void EnableSuperSampling()
        {
            renderer.EnableSuperSampling(figures);
        }

        public void DisableSuperSampling()
        {
            Redraw();
        }

        public void SetLineThickness(int t)
        {
            lineThickness = t;
        }

        private void Redraw()
        {
            renderer.Clear();
            renderer.Render(figures);
        }

        public void RenderImage(Bitmap bmp)
        {
            this.bitmap = bmp;
            image.UpdateBitmap(bmp);
        }

        public void FillNeighbours(Point p, Color boundary, Color @new)
        {
            int point_x = (int)p.X;
            int point_y = (int)p.Y;

            var data = bitmap.Lock();
            var queue = new Queue<Tuple<int, int>>(data.Width * data.Height);

            bool[,] visited = new bool[data.Width, data.Height];

            queue.Enqueue(new Tuple<int, int>(point_x, point_y));

            while (queue.Count > 0)
            {
                Tuple<int, int> tuple = queue.Dequeue();


                if (tuple.Item1 < 0 || tuple.Item1 >= data.Width ||
                    tuple.Item2 < 0 || tuple.Item2 >= data.Height)
                {
                    continue;
                }

                var currentPixel = data.GetPixel(tuple.Item1, tuple.Item2);
                if (currentPixel != boundary && !visited[tuple.Item1, tuple.Item2])
                {
                    data.SetPixel(tuple.Item1, tuple.Item2, @new);
                    visited[tuple.Item1, tuple.Item2] = true;

                    queue.Enqueue(new Tuple<int, int>(tuple.Item1 - 1, tuple.Item2));
                    queue.Enqueue(new Tuple<int, int>(tuple.Item1 + 1, tuple.Item2));
                    queue.Enqueue(new Tuple<int, int>(tuple.Item1, tuple.Item2 - 1));
                    queue.Enqueue(new Tuple<int, int>(tuple.Item1, tuple.Item2 + 1));
                }
            }            

            bitmap.Unlock(data);
            image.UpdateBitmap(bitmap);
        }
    }
}
