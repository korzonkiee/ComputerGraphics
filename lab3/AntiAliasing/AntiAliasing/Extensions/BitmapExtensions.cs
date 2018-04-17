using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiAliasing.Extensions
{
    public static class BitmapExtensions
    {
        public static void SetPixel(this BitmapData bitmapData, Bitmap bitmap, int x, int y, byte val)
        {
            unsafe
            {
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(bitmap.PixelFormat) >> 3;
                int widthInBytes = bitmapData.Width * bytesPerPixel;

                y = bitmapData.Height - y - 1;

                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                byte* row = ptrFirstPixel + (y * bitmapData.Stride);
                int col = x * bytesPerPixel;

                row[col + 2] = val;
                row[col + 1] = val;
                row[col] = val;
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
