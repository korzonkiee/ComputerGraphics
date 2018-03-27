using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dithering.ColorQuantization
{
    public struct Color
    {
        public byte R, G, B;
    }

    public static class ColorExtensions
    {
        public static double DistanceTo(this Color color, Color to)
        {
            return Math.Sqrt(
                Math.Pow(color.R - to.R, 2) +
                Math.Pow(color.G - to.G, 2) +
                Math.Pow(color.B - to.B, 2)
                );
        }
    }
}
