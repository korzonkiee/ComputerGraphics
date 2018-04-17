﻿using AntiAliasing.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiAliasing
{
    public sealed class BitmapProcessor
    {
        private readonly Bitmap bitmap;

        public BitmapProcessor(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        /// <summary>
        /// Sets pixel to specified color.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, byte color)
        {
            BitmapData bitmapData = bitmap.Lock();
            bitmapData.SetPixel(bitmap, x, y, 255);
            bitmap.Unlock(bitmapData);
        }

        /// <summary>
        /// Draws line using Digital Differential Analyzer algorithm.
        /// </summary>
        /// <param name="x1">x1</param>
        /// <param name="y1">y1</param>
        /// <param name="x2">x2</param>
        /// <param name="y2">y2</param>
        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            BitmapData bitmapData = bitmap.Lock();

            float dy = y2 - y1;
            float dx = x2 - x1;
            float m = dy / dx;
            float y = y1;
            for (int x = x1; x <= x2; ++x)
            {
                bitmapData.SetPixel(bitmap, x, (int)Math.Round(y), 0);
                y += m;
            }
            bitmap.Unlock(bitmapData);
        }

        /// <summary>
        /// Draws a circle using Midpoint Circle Algorithm.
        /// </summary>
        /// <param name="r">radius</param>
        public void DrawCircle(int r, int x_center, int y_center)
        {
            BitmapData bitmapData = bitmap.Lock();

            int dE = 3;
            int dSE = 5 - 2 * r;
            int d = 1 - r;
            int x = 0;
            int y = r;
            bitmapData.SetPixel(bitmap, x, y, 0);
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

                bitmapData.SetPixel(bitmap, x + x_center, y + y_center, 0);
                bitmapData.SetPixel(bitmap, x + x_center, -y + y_center, 0);
                bitmapData.SetPixel(bitmap, -x + x_center, y + y_center, 0);
                bitmapData.SetPixel(bitmap, -x + x_center, -y + y_center, 0);
                bitmapData.SetPixel(bitmap, y + x_center, x + y_center, 0);
                bitmapData.SetPixel(bitmap, -y + x_center, x + y_center, 0);
                bitmapData.SetPixel(bitmap, y + x_center, -x + y_center, 0);
                bitmapData.SetPixel(bitmap, -y + x_center, -x + y_center, 0);
            }

            bitmap.Unlock(bitmapData);
        }

        /// <summary>
        /// Sets each pixel to white color.
        /// </summary>
        public void ClearBitmap()
        {
            BitmapData bitmapData = bitmap.Lock();
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    bitmapData.SetPixel(bitmap, x, y, 255);
                }
            }
            bitmap.Unlock(bitmapData);
        }
    }
}
