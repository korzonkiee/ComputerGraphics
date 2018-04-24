using AntiAliasing.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

using Figure = AntiAliasing.Figures.Figure;
using Color = AntiAliasing.Figures.Color;
using AntiAliasing.Figures;
using System.IO;

namespace AntiAliasing
{
    public sealed class Renderer
    {
        private readonly Bitmap bitmap;
        private RenderMode renderMode = RenderMode.Normal;

        public event EventHandler<Figure> RenderCompleted;

        public Renderer(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        /// <summary>
        /// Renders a figure on a canvas.
        /// </summary>
        public void Render(Figure figure)
        {
            BitmapData bitmapData = bitmap.Lock();
            figure.Render(bitmapData, renderMode);
            bitmap.Unlock(bitmapData);

            RenderCompleted?.Invoke(this, figure);
        }

        /// <summary>
        /// Renders set of figures on a canvas.
        /// </summary>
        public void Render(IList<Figure> figures)
        {
            BitmapData bitmapData = bitmap.Lock();
            foreach (var figure in figures)
            {
                figure.Render(bitmapData, renderMode);
            }
            bitmap.Unlock(bitmapData);

            RenderCompleted?.Invoke(this, null);
        }

        public void SetPixel(int x, int y, Color color)
        {
            BitmapData bitmapData = bitmap.Lock();
            bitmapData.SetPixel(x, y, color);
            bitmap.Unlock(bitmapData);

            RenderCompleted?.Invoke(this, null);
        }

        /// <summary>
        /// Sets each pixel on a canvas to white color.
        /// </summary>
        public void Clear()
        {
            BitmapData bitmapData = bitmap.Lock();
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    bitmapData.SetPixel(x, y, Colors.White);
                }
            }
            bitmap.Unlock(bitmapData);

            RenderCompleted?.Invoke(this, null);
        }

        public void EnableSuperSampling(IList<Figure> figures)
        {
            BitmapData bitmapData = bitmap.Lock();

            var _bitmap = new Bitmap(bitmap.Width * 2, bitmap.Height * 2);
            using (Graphics graph = Graphics.FromImage(_bitmap))
            {
                Rectangle ImageSize = new Rectangle(0, 0, bitmap.Width * 2, bitmap.Height * 2);
                graph.FillRectangle(Brushes.White, ImageSize);
            }
            var _bitmapData = _bitmap.Lock();

            foreach (var figure in figures)
            {
                var ssFigure = figure.SuperSampled();
                ssFigure.Render(_bitmapData);
            }

            for (int i = 0; i < _bitmapData.Width; i += 2)
            {
                for (int j = 0; j < _bitmapData.Height; j += 2)
                {
                    var bottomLeft = _bitmapData.GetPixel(i, j);
                    var bottomRight = _bitmapData.GetPixel(i + 1, j);
                    var topLeft = _bitmapData.GetPixel(i, j + 1);
                    var topRight = _bitmapData.GetPixel(i + 1, j + 1);

                    var avg = (bottomLeft.R + bottomRight.R + topLeft.R + topRight.R) / 4;

                    bitmapData.SetPixel(i / 2, j / 2, (byte)avg);
                }
            }

            _bitmap.Unlock(_bitmapData);
            bitmap.Unlock(bitmapData);

            RenderCompleted?.Invoke(this, null);
        }

        public void EnableAntiAliasing()
        {
            renderMode = RenderMode.AntiAliasing;
        }

        public void DisableAntiAliasing()
        {
            renderMode = RenderMode.Normal;
        }
    }
}
