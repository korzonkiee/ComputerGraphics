using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalFilteringEditor
{
    public static class FuctionFilters
    {
        public static Bitmap PerformBrightnessCorrection(Bitmap bitmap, int c)
        {
            Color color;
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    color = bitmap.GetPixel(i, j);
                    color = FitPixelInDomain(color.R + c, color.G + c, color.B + c);
                    bitmap.SetPixel(i, j, color);
                }
            }
            return bitmap;
        }

        private static Color FitPixelInDomain(int r, int g, int b)
        {
            r = (r > 255) ? 255 : r;
            r = (r < 0) ? 0 : r;
            g = (g > 255) ? 255 : g;
            g = (g < 0) ? 0 : g;
            b = (b > 255) ? 255 : b;
            b = (b < 0) ? 0: b;

            return Color.FromArgb(r, g, b);
        }
    }
}
