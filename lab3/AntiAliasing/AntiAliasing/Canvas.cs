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

namespace AntiAliasing
{
    /// <summary>
    /// Canvas which make use of System.Windows.Controls.Image
    /// as an plane on which System.Drawing.Bitmap is rendered.
    /// </summary>
    public class Canvas
    {
        private readonly Image image;
        private readonly Bitmap bitmap;

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
                //lastPolygonUnderCursor.Fill();
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
    }
}
