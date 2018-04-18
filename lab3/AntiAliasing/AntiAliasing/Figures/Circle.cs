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
    public sealed class Circle : Figure
    {
        public int X_Center { get; }
        public int Y_Center { get; }
        public int R { get; }
        public Color Color { get; }

        public Circle(int x_center, int y_center, int r, Color color)
        {
            X_Center = x_center;
            Y_Center = y_center;
            R = r;
            Color = color;
        }


        /// <summary>
        /// Renders a circle using Midpoint Circle Algorithm.
        /// </summary>
        protected override void NormalRender(BitmapData bitmapData)
        {
            int dE = 3;
            int dSE = 5 - 2 * R;
            int d = 1 - R;
            int x = 0;
            int y = R;

            bitmapData.SetPixel(x + X_Center, y + Y_Center, Color);
            bitmapData.SetPixel(x + X_Center, -y + Y_Center, Color);
            bitmapData.SetPixel(-y + X_Center, x + Y_Center, Color);
            bitmapData.SetPixel(y + X_Center, x + Y_Center, Color);

            while (y > x)
            {
                //move to E
                if (d < 0)
                {
                    d += dE;
                    dE += 2;
                    dSE += 2;
                }
                //move to SE
                else
                {
                    d += dSE;
                    dE += 2;
                    dSE += 4;
                    --y;
                }
                ++x;

                bitmapData.SetPixel(x + X_Center, y + Y_Center, Color);
                bitmapData.SetPixel(x + X_Center, -y + Y_Center, Color);
                bitmapData.SetPixel(-x + X_Center, y + Y_Center, Color);
                bitmapData.SetPixel(-x + X_Center, -y + Y_Center, Color);
                bitmapData.SetPixel(y + X_Center, x + Y_Center, Color);
                bitmapData.SetPixel(-y + X_Center, x + Y_Center, Color);
                bitmapData.SetPixel(y + X_Center, -x + Y_Center, Color);
                bitmapData.SetPixel(-y + X_Center, -x + Y_Center, Color);
            }
        }

        /// <summary>
        /// Draws anti-aliased circle using Fast Antialiased Line Generation — Xiaolin Wu.
        /// </summary>
        protected override void AntiAliasingRender(BitmapData bitmapData)
        {
            byte lineColor = 0;
            byte bgColor = 255;

            int x_ceil = R;
            int y = 0;

            while (x_ceil > y)
            {
                double x_real = Math.Sqrt(R * R - y * y);
                x_ceil = (int)Math.Ceiling(x_real);

                double error = Error(x_ceil, x_real);

                byte color_r = (byte)(lineColor * (1 - error) + bgColor * error);
                byte color_l = (byte)(lineColor * error + bgColor * (1 - error));

                var colorR = new Color() { R = color_r, G = color_r, B = color_r };
                var colorL = new Color() { R = color_l, G = color_l, B = color_l };

                bitmapData.SetPixel(x_ceil + X_Center, y + Y_Center, colorR);
                bitmapData.SetPixel(x_ceil - 1 + X_Center, y + Y_Center, colorL);

                bitmapData.SetPixel(x_ceil + X_Center, -y + Y_Center, colorR);
                bitmapData.SetPixel(x_ceil - 1 + X_Center, -y + Y_Center, colorL);

                bitmapData.SetPixel(-x_ceil + X_Center, y + Y_Center, colorL);
                bitmapData.SetPixel(-x_ceil - 1 + X_Center, y + Y_Center, colorR);

                bitmapData.SetPixel(-x_ceil + X_Center, -y + Y_Center, colorL);
                bitmapData.SetPixel(-x_ceil - 1 + X_Center, -y + Y_Center, colorR);

                bitmapData.SetPixel(y + X_Center, x_ceil + Y_Center, colorR);
                bitmapData.SetPixel(y - 1 + X_Center, x_ceil + Y_Center, colorL);

                bitmapData.SetPixel(-y + X_Center, x_ceil + Y_Center, colorL);
                bitmapData.SetPixel(-y - 1 + X_Center, x_ceil + Y_Center, colorR);

                bitmapData.SetPixel(y + X_Center, -x_ceil + Y_Center, colorR);
                bitmapData.SetPixel(y - 1 + X_Center, -x_ceil + Y_Center, colorL);

                bitmapData.SetPixel(-y + X_Center, -x_ceil + Y_Center, colorL);
                bitmapData.SetPixel(-y - 1 + X_Center, -x_ceil + Y_Center, colorR);

                ++y;
            }
        }

        // Calculates error between ceiled x and real x.
        private double Error(int x_ceil, double x_real)
        {
            return x_ceil - x_real;
        }
    }
}