using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiAliasing.Figures
{
    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;


        public static bool operator ==(Color b, Color c)
        {
            return b.R == c.R && b.G == c.G && b.B == c.B;
        }

        public static bool operator !=(Color b, Color c)
        {
            return !(b == c);
        }
    }
    
    public static class Colors
    {
        public static Color Black = new Color
        {
            R = 0,
            G = 0,
            B = 0
        };

        public static Color Orange = new Color
        {
            R = 255,
            G = 127,
            B = 39
        };

        public static Color Grey = new Color
        {
            R = 127,
            G = 127,
            B = 127
        };

        public static Color White = new Color
        {
            R = 255,
            G = 255,
            B = 255
        };
    }
}
