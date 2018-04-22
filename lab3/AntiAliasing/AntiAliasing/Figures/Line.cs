using AntiAliasing.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = AntiAliasing.Figures.Color;

namespace AntiAliasing.Figures
{
    public sealed class Line : Figure
    {
        public int X1 { get; }
        public int Y1 { get; }
        public int X2 { get; }
        public int Y2 { get; }
        public Color Color { get; }

        public Line(int x1, int y1, int x2, int y2, Color color)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Color = color;
        }

        /// <summary>
        /// Renders a line using Digital Differential Analyzer algorithm.
        /// </summary>
        protected override void NormalRender(BitmapData bitmapData)
        {
            float dy = Y2 - Y1;
            float dx = X2 - X1;
            float m = dy / dx;
            float y = Y1;

            int start = X1, end = X2;
            if (X2 < X1)
            {
                start = X2;
                end = X1;
            }

            for (int x = start; x <= end; x++)
            {
                bitmapData.SetPixel(x, (int)Math.Round(y), Color);
                y += m;
            }
        }

        /// <summary>
        /// Draws anti-aliased line using Fast Antialiased Line Generation — Xiaolin Wu.
        /// </summary>
        protected override void AntiAliasingRender(BitmapData bitmapData)
        {

            byte lineColor = 0;
            byte bgColor = 255;

            float dy = Y2 - Y1;
            float dx = X2 - X1;
            float m = dy / dx;
            float y = Y1;

            int start = X1, end = X2;
            if (X2 < X1)
            {
                start = X2;
                end = X1;
            }

            for (int x = start; x <= end; ++x)
            {
                byte c1 = (byte)(lineColor * (1 - Modf(y)) + bgColor * Modf(y));
                byte c2 = (byte)(lineColor * Modf(y) + bgColor * (1 - Modf(y)));

                bitmapData.SetPixel(x, (int)Math.Floor(y), new Color() { R = c1, G = c1, B = c1 });
                bitmapData.SetPixel(x, (int)Math.Floor(y) + 1, new Color() { R = c2, G = c2, B = c2 });

                y += m;
            }
        }

        private double Modf(double y)
        {
            return y - Math.Truncate(y);
        }
    }
}
