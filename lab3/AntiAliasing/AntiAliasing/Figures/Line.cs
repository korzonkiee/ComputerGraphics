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
        public double x0 { get; private set; }
        public double y0 { get; private set; }
        public double x1 { get; private set; }
        public double y1 { get; private set; }

        public int thickness { get; private set; }

        public Color Color { get; }

        public Line(double x0, double y0, double x1, double y1, Color color, int thickness = 5)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.x1 = x1;
            this.y1 = y1;

            this.thickness = thickness;

            Color = color;
        }

        /// <summary>
        /// Renders a line using Digital Differential Analyzer algorithm.
        /// </summary>
        protected override void NormalRender(BitmapData bitmapData)
        {
            double _y0 = y0;
            double _x0 = x0;
            double _y1 = y1;
            double _x1 = x1;

            bool steep = Math.Abs(_y1 - _y0) > Math.Abs(_x1 - _x0);
            double temp;
            if (steep)
            {
                temp = _x0; _x0 = _y0; _y0 = temp;
                temp = _x1; _x1 = _y1; _y1 = temp;
            }

            if (_x0 > _x1)
            {
                temp = _x0; _x0 = _x1; _x1 = temp;
                temp = _y0; _y0 = _y1; _y1 = temp;
            }

            double dy = _y1 - _y0;
            double dx = _x1 - _x0;
            double m = dy / dx;
            double y = _y0;

            for (int x = (int)_x0; x <= _x1; x++)
            {
                if (steep)
                {
                    bitmapData.SetPixel((int)Math.Round(y), x, Color);
                    for (int t = 0; t <= thickness / 2; t++)
                    {
                        bitmapData.SetPixel((int)Math.Round(y) - t, x, Color);
                        bitmapData.SetPixel((int)Math.Round(y) + t, x, Color);
                    }
                }
                else
                {
                    bitmapData.SetPixel(x, (int)Math.Round(y), Color);
                    for (int t = 0; t <= thickness / 2; t++)
                    {
                        bitmapData.SetPixel(x, (int)Math.Round(y) - t, Color);
                        bitmapData.SetPixel(x, (int)Math.Round(y) + t, Color);
                    }
                }

                y += m;
            }
        }

        /// <summary>
        /// Draws anti-aliased line using Fast Antialiased Line Generation — Xiaolin Wu.
        /// </summary>
        protected override void AntiAliasingRender(BitmapData bitmap)
        {
            double _y0 = y0;
            double _x0 = x0;
            double _y1 = y1;
            double _x1 = x1;

            byte lineColor = 0;
            byte bgColor = 255;

            bool steep = Math.Abs(_y1 - _y0) > Math.Abs(_x1 - _x0);
            double temp;
            if (steep)
            {
                temp = _x0; _x0 = _y0; _y0 = temp;
                temp = _x1; _x1 = _y1; _y1 = temp;
            }

            if (_x0 > _x1)
            {
                temp = _x0; _x0 = _x1; _x1 = temp;
                temp = _y0; _y0 = _y1; _y1 = temp;
            }

            double dy = _y1 - _y0;
            double dx = _x1 - _x0;
            double m = dy / dx;
            double y = _y0;

            if (steep)
            {
                for (int x = (int)_x0; x <= (int)_x1; x++)
                {
                    byte c1 = (byte)(lineColor * (1 - Modf(y)) + bgColor * Modf(y));
                    byte c2 = (byte)(lineColor * Modf(y) + bgColor * (1 - Modf(y)));

                    bitmap.SetPixel((int)Math.Floor(y), x, new Color() { R = c1, G = c1, B = c1 });
                    bitmap.SetPixel((int)Math.Floor(y) + 1, x, new Color() { R = c2, G = c2, B = c2 });

                    y += m;
                }
            }
            else
            {
                for (int x = (int)_x0; x <= (int)_x1; x++)
                {
                    byte c1 = (byte)(lineColor * (1 - Modf(y)) + bgColor * Modf(y));
                    byte c2 = (byte)(lineColor * Modf(y) + bgColor * (1 - Modf(y)));

                    bitmap.SetPixel(x, (int)Math.Floor(y), new Color() { R = c1, G = c1, B = c1 });
                    bitmap.SetPixel(x, (int)Math.Floor(y) + 1, new Color() { R = c2, G = c2, B = c2 });

                    y += m;
                }
            }
        }

        int ipart(double x) { return (int)x; }

        int round(double x) { return ipart(x + 0.5); }

        double fpart(double x)
        {
            if (x < 0) return (1 - (x - Math.Floor(x)));
            return (x - Math.Floor(x));
        }

        double rfpart(double x)
        {
            return 1 - fpart(x);
        }

        private double Modf(double y)
        {
            return y - Math.Truncate(y);
        }

        private void plot(BitmapData bitmap, double x, double y, double c)
        {
            int alpha = (int)(c * 255);
            if (alpha > 255) alpha = 255;
            if (alpha < 0) alpha = 0;

            bitmap.SetPixel((int)x, (int)y, (byte)alpha);
        }
    }
}
