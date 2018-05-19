using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiAliasing.Figures
{
    public class Rectangle : Figure
    {
        public double x_min { get; private set; }
        public double y_min { get; private set; }
        public double x_max { get; private set; }
        public double y_max { get; private set; }

        public Color Color { get; }
        public int thickness { get; private set; }

        // 0 - left, 1 - right, 2 - bottom, 3 - top
        protected Line[] Borders { get; private set; }

        public Rectangle(double x_min, double y_min, double x_max, double y_max, Color color, int thickness = 1)
        {
            this.x_min = x_min;
            this.y_min = y_min;
            this.x_max = x_max;
            this.y_max = y_max;

            Color = color;
            this.thickness = thickness;

            InitializeLines();
        }

        public override Figure SuperSampled()
        {
            int _thick = 1;

            if (thickness == 1)
                _thick = 3;
            else if (thickness == 3)
                _thick = 5;
            else if (thickness == 5)
                _thick = 7;
            else if (thickness == 7)
                _thick = 9;

            return new Rectangle(x_min * 2, y_min * 2, x_max * 2, y_max * 2, Color, _thick);
        }

        public override void AntiAliasingRender(BitmapData bitmapData)
        {
            foreach (var border in Borders)
            {
                border.AntiAliasingRender(bitmapData);
            }
        }

        public override void NormalRender(BitmapData bitmapData)
        {
            foreach (var border in Borders)
            {
                border.NormalRender(bitmapData);
            }
        }

        private void InitializeLines()
        {
            Borders = new Line[4]
            {
                new Line(x_min, y_min, x_min, y_max, Color, thickness),
                new Line(x_max, y_min, x_max, y_max, Color, thickness),
                new Line(x_min, y_min, x_max, y_min, Color, thickness),
                new Line(x_min, y_max, x_max, y_max, Color, thickness)
            };
        }
    }
}
