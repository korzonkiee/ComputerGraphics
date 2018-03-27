using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dithering
{
    public abstract class ImageProcessor
    {
        unsafe protected abstract void ProcessPixel(byte* r, byte* g, byte* b, int yPos, int xPos);
        public abstract void Process(Bitmap bitmap);
    }

    public abstract class ParallelImageProcessor : ImageProcessor
    {
        public override void Process(Bitmap bitmap)
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
                        ProcessPixel(&currentRow[x + 2], &currentRow[x + 1], &currentRow[x], y, x / bytesPerPixel);
                    }
                });

                bitmap.UnlockBits(bitmapData);
            }
        }
    }

    public abstract class SingleThreadImageProcessor : ImageProcessor
    {
        public override void Process(Bitmap bitmap)
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

                for (int y = 0; y < bitmapData.Height; y++)
                {
                    byte* currentRow = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        ProcessPixel(&currentRow[x + 2], &currentRow[x + 1], &currentRow[x], y, x / bytesPerPixel);
                    }
                }

                bitmap.UnlockBits(bitmapData);
            }
        }
    }
}
