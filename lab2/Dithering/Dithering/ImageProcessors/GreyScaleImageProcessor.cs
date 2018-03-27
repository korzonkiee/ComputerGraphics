using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dithering.ImageProcessors
{
    public class GreyScaleImageProcessor : ParallelImageProcessor
    {
        protected override unsafe void ProcessPixel(byte* r, byte* g, byte* b, int yPos, int xPos)
        {
            byte grey = (byte)(0.3 * (*r) + 0.6 * (*g) + 0.1 * (*b));

            *r = grey;
            *g = grey;
            *b = grey;
        }
    }
}
