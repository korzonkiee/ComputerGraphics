using AntiAliasing.Extensions;
using AntiAliasing.Figures;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Image = System.Windows.Controls.Image;

namespace AntiAliasing
{
    /// <summary>
    /// Canvas which make use of System.Windows.Controls.Image
    /// as an plane on which System.Drawing.Bitmap is rendered.W
    /// </summary>
    public class Canvas
    {
        private readonly Image image;
        private readonly Bitmap bitmap;

        private readonly Renderer renderer;

        private List<Figure> figures = new List<Figure>();

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
            var line = new Line(x1, y1, x2, y2, Colors.Black);
            renderer.Render(line);
        }

        public void EnableAntiAliasing()
        {
            renderer.EnableAntiAliasing();
            renderer.Clear();
            renderer.Render(figures);
        }

        public void DisableAntiAliasing()
        {
            renderer.DisableAntiAliasing();
            renderer.Clear();
            renderer.Render(figures);
        }
    }
}
