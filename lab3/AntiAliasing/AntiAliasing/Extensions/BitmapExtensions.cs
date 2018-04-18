using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Color = AntiAliasing.Figures.Color;

namespace AntiAliasing.Extensions
{
    public static class BitmapExtensions
    {
        public static void SetPixel(this BitmapData bitmapData, int x, int y, Color color)
        {
            if (x < 0 || x >= bitmapData.Width ||
                y < 0 || y >= bitmapData.Height)
            {
                return;
            }

            unsafe
            {
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(PixelFormat.Format32bppArgb) >> 3;
                int widthInBytes = bitmapData.Width * bytesPerPixel;

                y = bitmapData.Height - y - 1;

                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                byte* row = ptrFirstPixel + (y * bitmapData.Stride);
                int col = x * bytesPerPixel;

                row[col + 2] = color.R;
                row[col + 1] = color.G;
                row[col] = color.B;
            }
        }

        public static BitmapData Lock(this Bitmap bitmap)
        {
            return bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
        }
        
        public static void Unlock(this Bitmap bitmap, BitmapData bitmapData)
        {
            bitmap.UnlockBits(bitmapData);
        }
    }
}
