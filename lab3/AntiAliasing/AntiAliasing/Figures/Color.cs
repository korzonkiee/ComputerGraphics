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
    }
    
    public static class Colors
    {
        public static Color Black = new Color
        {
            R = 0,
            G = 0,
            B = 0
        };

        public static Color White = new Color
        {
            R = 255,
            G = 255,
            B = 255
        };
    }
}
