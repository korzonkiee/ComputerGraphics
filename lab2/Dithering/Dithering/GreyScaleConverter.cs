using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dithering
{
    public class GreyScaleConverter
    {
        public void Convert(Bitmap bitmap)
        {
            unsafe
            {
                BitmapData bitmapData = bitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadWrite,
                    bitmap.PixelFormat);

                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(bitmap.PixelFormat) >> 3;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;


                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentRow = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        byte oldBlue = currentRow[x];
                        byte oldGreen = currentRow[x + 1];
                        byte oldRed = currentRow[x + 2];

                        byte grey = (byte)(0.3 * oldRed + 0.6 * oldGreen + 0.1 * oldBlue);

                        currentRow[x] = grey;
                        currentRow[x + 1] = grey;
                        currentRow[x + 2] = grey;
                    }
                });

                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}
