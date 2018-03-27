using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dithering.ImageProcessors
{
    public class DitheringImageProcessor : ParallelImageProcessor
    {
        private readonly Random random;
        private readonly object randomLock = new object();

        public int GreyLevels { get; set; } = 2;
        private byte[] levels;

        public int DitherMatrixSize { get; set; } = 3;
        private float[,] bayerMatrix;

        public DitheringType DitheringType { get; set; }

        public DitheringImageProcessor()
        {
            this.random = new Random();
        }

        public override void Process(Bitmap bitmap)
        {
            this.levels = CalcLevels(GreyLevels);

            int[,] dither = CalcDitherMatrix(DitherMatrixSize);
            bayerMatrix = CalcBayerMatrix(dither);

            base.Process(bitmap);
        }

        protected override unsafe void ProcessPixel(byte* r, byte* g, byte* b, int yPos, int xPos)
        {
            switch (DitheringType)
            {
                case DitheringType.Random:
                    RandomDithering(r, g, b);
                    break;
                case DitheringType.Ordered:
                    OrderedDithering(r, g, b, yPos, xPos);
                    break;
                default:
                    break;
            }
        }

        private unsafe void RandomDithering(byte* r, byte* g, byte *b) 
        {
            float intensity = *r / 255f;

            double rand = 0;
            lock (randomLock)
            {
                rand = random.NextDouble();
            }

            float greyLevelIntensity = (GreyLevels - 1) * intensity;

            int upperGreyLevel = (int)Math.Ceiling(greyLevelIntensity);
            int bottomGreyLevel = (int)Math.Floor(greyLevelIntensity);

            int greyLevel = (intensity < rand) ? bottomGreyLevel : upperGreyLevel;

            *b = levels[greyLevel];
            *g = levels[greyLevel];
            *r = levels[greyLevel];
        }

        private unsafe void OrderedDithering(byte* r, byte* g, byte* b, int yPos, int xPos)
        {
            byte grey = *r;

            float intensity = (float)grey / 255f;
            int greyLevel = (int)Math.Floor((GreyLevels - 1) * intensity);

            float re = (GreyLevels - 1) * intensity - greyLevel;
            if (re >= bayerMatrix[xPos % DitherMatrixSize, yPos % DitherMatrixSize])
                greyLevel++;

            byte output = levels[greyLevel];

            *b = output;
            *g = output;
            *r = output;
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
