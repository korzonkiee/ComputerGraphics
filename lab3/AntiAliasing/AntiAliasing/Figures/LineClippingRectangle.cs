using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiAliasing.Figures
{
    public sealed class LineClippingRectangle : Rectangle
    {
        private readonly IEnumerable<Line> linesOnCanvas;

        public LineClippingRectangle(double x_min, double y_min,double x_max, double y_max,
            Color color, IEnumerable<Line> linesOnCanvas, int thickness = 1)
            : base(x_min, y_min, x_max, y_max, color, thickness)
        {
            this.linesOnCanvas = linesOnCanvas;
        }

        public void 
    }
}
