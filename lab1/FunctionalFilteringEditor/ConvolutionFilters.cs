using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalFilteringEditor
{
    public static class ConvolutionFilters
    {
        public static Bitmap PerformGaussianBlur(Bitmap bitmap)
        {
            int w = bitmap.Width;
            int h = bitmap.Height;

            int ks = 3;
            int krad = 1;
            int kdiv = 16;

            int[,] kernel = CreateDefaultGaussianKernel();

            var copy = (Bitmap)bitmap.Clone();

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int kspx = x - krad;
                    int kspy = y - krad;
                    int sum = 0;

                    for (int ky = kspy, ky_i = 0 ; ky < kspy + ks; ky++, ky_i++)
                    {
                        for (int kx = kspx, kx_i = 0; kx < kspx + ks; kx++, kx_i++)
                        {
                            var ki = GetKernelIndex(kx, ky, w, h);
                            sum += kernel[kx_i, ky_i] * bitmap.GetPixel(ki.Item1, ki.Item2).R;
                        }
                    }

                    int msum = sum / kdiv;
                    copy.SetPixel(x, y, Color.FromArgb(msum, msum, msum));
                }
            }

            return copy;
        }

        public static Bitmap PerformEdgeDetection(Bitmap bitmap)
        {
            int w = bitmap.Width;
            int h = bitmap.Height;

            int ks = 3;
            int krad = 1;
            int offset = 127;

            int[,] kernel = CreateDefaultEdgeDetectionKernel();

            var copy = (Bitmap)bitmap.Clone();

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int kspx = x - krad;
                    int kspy = y - krad;
                    int sum = 0;

                    for (int ky = kspy, ky_i = 0; ky < kspy + ks; ky++, ky_i++)
                    {
                        for (int kx = kspx, kx_i = 0; kx < kspx + ks; kx++, kx_i++)
                        {
                            var ki = GetKernelIndex(kx, ky, w, h);
                            sum += kernel[kx_i, ky_i] * bitmap.GetPixel(ki.Item1, ki.Item2).R;
                        }
                    }

                    sum += offset;

                    sum = sum < 0 ? 0 : sum;
                    sum = sum > 255 ? 255 : sum;
                    
                    copy.SetPixel(x, y, Color.FromArgb(sum, sum, sum));
                }
            }

            return copy;
        }

        public static Bitmap PerformSharpening(Bitmap bitmap)
        {
            int w = bitmap.Width;
            int h = bitmap.Height;

            int ks = 3;
            int krad = 1;

            int[,] kernel = CreateDefaultSharpeningKernel();

            var copy = (Bitmap)bitmap.Clone();

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int kspx = x - krad;
                    int kspy = y - krad;
                    int sum = 0;

                    for (int ky = kspy, ky_i = 0; ky < kspy + ks; ky++, ky_i++)
                    {
                        for (int kx = kspx, kx_i = 0; kx < kspx + ks; kx++, kx_i++)
                        {
                            var ki = GetKernelIndex(kx, ky, w, h);
                            sum += kernel[kx_i, ky_i] * bitmap.GetPixel(ki.Item1, ki.Item2).R;
                        }
                    }

                    sum = sum < 0 ? 0 : sum;
                    sum = sum > 255 ? 255 : sum;

                    copy.SetPixel(x, y, Color.FromArgb(sum, sum, sum));
                }
            }

            return copy;
        }

        public static Bitmap PerformEmbossing(Bitmap bitmap)
        {
            int w = bitmap.Width;
            int h = bitmap.Height;

            int ks = 3;
            int krad = 1;

            int[,] kernel = CreateDefaultEmbossingKernel();

            var copy = (Bitmap)bitmap.Clone();

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int kspx = x - krad;
                    int kspy = y - krad;
                    int sum = 0;

                    for (int ky = kspy, ky_i = 0; ky < kspy + ks; ky++, ky_i++)
                    {
                        for (int kx = kspx, kx_i = 0; kx < kspx + ks; kx++, kx_i++)
                        {
                            var ki = GetKernelIndex(kx, ky, w, h);
                            sum += kernel[kx_i, ky_i] * bitmap.GetPixel(ki.Item1, ki.Item2).R;
                        }
                    }

                    sum = sum < 0 ? 0 : sum;
                    sum = sum > 255 ? 255 : sum;

                    copy.SetPixel(x, y, Color.FromArgb(sum, sum, sum));
                }
            }

            return copy;
        }

        public static Bitmap PerformDithering(Bitmap bitmap)
        {
            
        }

        public static Bitmap PerformBoxFilter(Bitmap bitmap, int ks)
        {
            int krad = ks / 2;
            int w = bitmap.Width;
            int h = bitmap.Height;

            Bitmap copy = (Bitmap) bitmap.Clone();
            Color color;
            for (int y = 0; y < h; y++)
            {
                int acc = 0;
                for (int k = -krad; k < krad + 1; k++)
                {
                    var kpos = GetKernelIndex(k, y, w, h);
                    acc += bitmap.GetPixel(kpos.Item1, kpos.Item2).R;
                }

                var p = acc / ks;
                copy.SetPixel(0, y, Color.FromArgb(p, p, p));

                for (int x = 1; x < w; x++)
                {
                    var kpos = GetKernelIndex(x - krad - 1, y, w, h);
                    acc -= bitmap.GetPixel(kpos.Item1, kpos.Item2).R;
                    kpos = GetKernelIndex(x + krad, y, w, h);
                    acc += bitmap.GetPixel(kpos.Item1, kpos.Item2).R;
                    p = acc / ks;
                    copy.SetPixel(x, y, Color.FromArgb(p, p, p));
                }
            }

            for (int x = 0; x < w; x++)
            {
                var acc = 0;
                for (int k = -krad; k < krad + 1; k++)
                {
                    var kpos = GetKernelIndex(x, k, w, h);
                    acc += copy.GetPixel(kpos.Item1, kpos.Item2).R;
                }
                var p = acc / ks;
                bitmap.SetPixel(x, 0, Color.FromArgb(p, p, p));

                for (int y = 1; y < h; y++)
                {
                    var kpos = GetKernelIndex(x, y - krad - 1, w, h);
                    acc -= copy.GetPixel(kpos.Item1, kpos.Item2).R;

                    kpos = GetKernelIndex(x, y + krad, w, h);
                    acc += copy.GetPixel(kpos.Item1, kpos.Item2).R;

                    p = acc / ks;
                    bitmap.SetPixel(x, y, Color.FromArgb(p, p, p));
                }

            }
            return bitmap;
        }

        private static int[,] CreateDefaultGaussianKernel()
        {
            return new int[,]
                {   { 1, 2, 1 },
                    { 2, 4, 2 },
                    { 1, 2, 1 },
                };
        }

        private static int[,] CreateDefaultEdgeDetectionKernel()
        {
            return new int[,]
                {   { -1, -1, -1 },
                    { -1, 8, -1 },
                    { -1, -1, -1 },
                };
        }

        private static int[,] CreateDefaultSharpeningKernel()
        {
            return new int[,]
                {   { 0, -1, 0 },
                    { -1, 5, -1 },
                    { 0, -1, 0 },
                };
        }

        private static int[,] CreateDefaultEmbossingKernel()
        {
            return new int[,]
                {   { -2, -1, 0 },
                    { -1, 1, 1 },
                    { 0, 1, 2 },
                };
        }

        private static Tuple<int, int> GetKernelIndex(int kx, int ky, int width, int height)
        {
            if (kx < 0 && ky< 0)
            {
                kx = ky = 0;
            }
            else if (kx > width - 1 && ky< 0)
            {
                kx = width - 1;
                ky = 0;
            }
            else if (kx< 0 && ky > height - 1)
            {
                kx = 0;
                ky = height - 1;
            }
            else if (kx > width - 1 && ky > height - 1)
            {
                kx = width - 1;
                ky = height - 1;
            }
            else if (kx< 0)
            {
                kx = 0;
            }
            else if (ky< 0)
            {
                ky = 0;
            }
            else if (kx > width - 1)
            {
                kx = width - 1;
            }
            else if (ky > height - 1)
            {
                ky = height - 1;
            }

            return Tuple.Create(kx, ky);
        }
    }
}
