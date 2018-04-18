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
            foreach(var figure in figures)
            {
                figure.Render(bitmapData, renderMode);
            }
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
