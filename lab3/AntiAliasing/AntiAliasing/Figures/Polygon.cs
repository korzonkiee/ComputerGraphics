using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AntiAliasing.Figures
{
    public sealed class Polygon : Figure
    {
        private readonly List<Point> vertices;

        public Color Color { get; }
        public int Thickness { get; private set; }

        public Polygon(List<Point> vertices, Color color, int thickness = 1)
        {
            this.vertices = vertices;

            Color = color;
            Thickness = thickness;
        }

        public override void AntiAliasingRender(BitmapData bitmapData)
        {
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                new Line(vertices[i].X, vertices[i].Y, vertices[i + 1].X, vertices[i + 1].Y, Color, Thickness)
                    .AntiAliasingRender(bitmapData);
            }

            new Line(vertices[vertices.Count - 1].X, vertices[vertices.Count - 1].Y, vertices[0].X, vertices[0].Y, Color, Thickness)
                .AntiAliasingRender(bitmapData);
        }

        public override void NormalRender(BitmapData bitmapData)
        {
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                new Line(vertices[i].X, vertices[i].Y, vertices[i + 1].X, vertices[i + 1].Y, Color, Thickness)
                    .NormalRender(bitmapData);
            }

            new Line(vertices[vertices.Count - 1].X, vertices[vertices.Count - 1].Y, vertices[0].X, vertices[0].Y, Color, Thickness)
                .NormalRender(bitmapData);
        }

        public override Figure SuperSampled()
        {
            int _thick = 1;

            if (Thickness == 1)
                _thick = 3;
            else if (Thickness == 3)
                _thick = 5;
            else if (Thickness == 5)
                _thick = 7;
            else if (Thickness == 7)
                _thick = 9;

            var _ssVertices = vertices
                .Select(v =>
                {
                    v.X = v.X * 2;
                    v.Y = v.Y * 2;

                    return v;
                }).ToList();

            return new Polygon(_ssVertices, Color, _thick);
        }
    }
}
