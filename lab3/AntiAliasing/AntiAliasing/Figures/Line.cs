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

        public void UpdateStart(double x0, double y0)
        {
            this.x0 = x0;
            this.y0 = y0;
        }

        public void UpdateEnd(double x1, double y1)
        {
            this.x1 = x1;
            this.y1 = y1;
        }

        public Line(double x0, double y0, double x1, double y1, Color color, int thickness = 5)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.x1 = x1;
            this.y1 = y1;

            this.thickness = thickness;

            Color = color;
        }

        public override Figure SuperSampled()
        {
            int _thick = 1;

            if (thickness == 1)
                _thick = 3;
            else if (thickness == 3)
                _thick = 5;
            else if (thickness == 5)
                _thick = 7;
            else if (thickness == 7)
                _thick = 9;

            return new Line(x0 * 2, y0 * 2, x1 * 2, y1 * 2, Color, _thick);
        }

        /// <summary>
        /// Renders a line using Digital Differential Analyzer algorithm.
        /// </summary>
        public override void NormalRender(BitmapData bitmapData)
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
        public override void AntiAliasingRender(BitmapData bitmap)
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

        public void Clip(double tE, double tL)
        {
            double Δx = this.x1 - this.x0;
            double Δy = this.y1 - this.y0;

            double a = Δy / Δx;

            x0 = x0 + tE * Δx;
            x1 = x0 + tL * Δx;

            y0 = y0 + a * (tE * Δx);
            y1 = y0 + a * (tL * Δx);
        }

        private double Modf(double y)
        {
            return y - Math.Truncate(y);
        }
    }
}
