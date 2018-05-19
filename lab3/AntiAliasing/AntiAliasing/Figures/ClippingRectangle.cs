using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiAliasing.Figures
{
    public class ClippingRectangle : Rectangle
    {
        public event EventHandler<Line> OutsideLine;

        public ClippingRectangle(double x_min, double y_min, double x_max, double y_max, Color color, int thickness = 1)
            : base(x_min, y_min, x_max, y_max, color, thickness)
        { }

        public void ClipFigure(Figure figure)
        {
            // For now I can handle line clipping only
            if (!(figure is Line))
            {
                throw new InvalidOperationException("Clipping can be applied to Line only");
            }

            var line = figure as Line;
            ClipLineToBorderIfNecessary(line);
        }

        private void ClipLineToBorderIfNecessary(Line line)
        {
            double dx = line.x1 - line.x0;
            double dy = line.y1 - line.y0;

            double tE = 0, tL = 1;

            if (ClipT(dx, x_min - line.x0, ref tE, ref tL) &&
                  ClipT(-dx, line.x0 - x_max, ref tE, ref tL) &&
                  ClipT(dy, y_min - line.y0, ref tE, ref tL) &&
                  ClipT(-dy, line.y0 - y_max, ref tE, ref tL))
            {
                if (tL < 1)
                {
                    line.UpdateEnd(line.x0 + tL * dx, line.y0 + tL * dy);
                }

                if (tE > 0)
                {
                    line.UpdateStart(line.x0 + tE * dx, line.y0 + tE * dy);
                }
            }
            else
            {
                OutsideLine?.Invoke(this, line);
            }
        }

        private double CalculatePParam(Line line, int index)
        {
            double Δx = line.x1 - line.x0;
            double Δy = line.y1 - line.y0;

            switch (index)
            {
                case 0:
                    return -Δx;
                case 1:
                    return Δx;
                case 2:
                    return -Δy;
                case 3:
                    return Δy;
                default:
                    throw new InvalidOperationException("Invalid index");
            }
        }

        private double CalculateQParam(Line line, Line border, int index)
        {
            switch (index)
            {
                case 0:
                    return line.x0 - border.x0;
                case 1:
                    return border.x1 - line.x0;
                case 2:
                    return line.y0 - border.y0;
                case 3:
                    return border.y1 - line.y0;
                default:
                    throw new InvalidOperationException("Invalid index");
            }
        }

        private bool ClipT(double p, double q, ref double tE, ref double tL)
        {
            if (p == 0)
            {
                if (q > 0)
                    return false;
                return true;
            }

            double t = q / p;
            if (p > 0)
            {
                if (t > tL)
                    return false;
                if (t > tE)
                    tE = t;
            }
            else
            {
                if (t < tE)
                    return false;
            if (t < tL)
                    tL = t;
            }

            return true;
        }
    }
}
