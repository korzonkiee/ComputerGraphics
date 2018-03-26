using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dithering
{
    public class DitheringProcessor
    {
        private readonly Random random;

        public DitheringProcessor()
        {
            this.random = new Random();
        }

        public void ApplyRandomDithering(Bitmap bitmap, int greyLevels)
        {
            byte[] levels = CalcLevels(greyLevels);

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
                        float intensity = currentRow[x] / 255f;
                        
                        double rand = random.NextDouble();

                        float greyLevelIntensity = (greyLevels - 1) * intensity;

                        int upperGreyLevel = (int)Math.Ceiling(greyLevelIntensity);
                        int bottomGreyLevel = (int)Math.Floor(greyLevelIntensity);

                        int greyLevel = (intensity < rand) ? bottomGreyLevel : upperGreyLevel;

                        currentRow[x] = levels[greyLevel];
                        currentRow[x + 1] = levels[greyLevel];
                        currentRow[x + 2] = levels[greyLevel];
                    }
                });

                bitmap.UnlockBits(bitmapData);
            }
        }

        public void ApplyOrdererdDithering(Bitmap bitmap, int size, int greyLevels)
        {
            byte[] levels = CalcLevels(greyLevels);
            int[,] dither = CalcDitherMatrix(size);
            float[,] bayer = CalcBayerMatrix(dither);

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
                        byte grey = currentRow[x];

                        float intensity = (float)grey / 255f;
                        int greyLevel = (int)Math.Floor((greyLevels - 1) * intensity);

                        float re = (greyLevels - 1) * intensity - greyLevel;
                        int actualX = x / bytesPerPixel;
                        if (re >= bayer[actualX % size, y % size])
                            greyLevel++;

                        byte output = levels[greyLevel];

                        currentRow[x] = output;
                        currentRow[x + 1] = output;
                        currentRow[x + 2] = output;
                    }
                });

                bitmap.UnlockBits(bitmapData);
            }
        }

        private float[,] CalcBayerMatrix(int[,] dither)
        {
            int size = dither.GetLength(0);
            float[,] bayer = new float[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    bayer[i, j] = (1f / (size * size)) * dither[i, j];
                }
            }

            return bayer;
        }

        private int[,] CalcDitherMatrix(int size)
        {
            if (size == 2)
            {
                return new int[2, 2]
                {
                    { 1, 3 },
                    { 4, 2 }
                };
            }

            if (size == 3)
            {
                return new int[3, 3]
                {
                    { 3, 7, 4 },
                    { 6, 1, 9 },
                    { 2, 8, 5 }
                };
            }

            int[,] dither = new int[size, size];

            int recDitherSize = size / 2;
            int[,] recDither = CalcDitherMatrix(recDitherSize);

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    int add = 0;

                    if (r < recDitherSize && c < recDitherSize)
                        add = 1;
                    else if (r < recDitherSize && c >= recDitherSize)
                        add = 3;
                    else if (r >= recDitherSize && c < recDitherSize)
                        add = 4;
                    else if (r >= recDitherSize && c >= recDitherSize)
                        add = 2;

                    dither[r, c] = 4 * (recDither[r % recDitherSize, c % recDitherSize] - 1) + add;
                }
            }

            return dither;
        }

        private byte[] CalcLevels(int levelsSize)
        {
            byte[] levels = new byte[levelsSize];

            for (int i = 0; i < levelsSize - 1; i++)
            {
                levels[i] = (byte)(0 + i * (255 / (levelsSize - 1)));
            }

            levels[levelsSize - 1] = 255;

            return levels;
        }
    }
}
