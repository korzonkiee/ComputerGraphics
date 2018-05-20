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
        public List<Point> Vertices { get; private set; }

        public Color Color { get; }
        public int Thickness { get; private set; }

        public Polygon(List<Point> vertices, Color color, int thickness = 1)
        {
            this.Vertices = new List<Point>(vertices);

            Color = color;
            Thickness = thickness;
        }

        public override void AntiAliasingRender(BitmapData bitmapData)
        {
            for (int i = 0; i < Vertices.Count - 1; i++)
            {
                new Line(Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X, Vertices[i + 1].Y, Color, Thickness)
                    .AntiAliasingRender(bitmapData);
            }

            new Line(Vertices[Vertices.Count - 1].X, Vertices[Vertices.Count - 1].Y, Vertices[0].X, Vertices[0].Y, Color, Thickness)
                .AntiAliasingRender(bitmapData);
        }

        public override void NormalRender(BitmapData bitmapData)
        {
            for (int i = 0; i < Vertices.Count - 1; i++)
            {
                new Line(Vertices[i].X, Vertices[i].Y, Vertices[i + 1].X, Vertices[i + 1].Y, Color, Thickness)
                    .NormalRender(bitmapData);
            }

            new Line(Vertices[Vertices.Count - 1].X, Vertices[Vertices.Count - 1].Y, Vertices[0].X, Vertices[0].Y, Color, Thickness)
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

            var _ssVertices = Vertices
                .Select(v =>
                {
                    v.X = v.X * 2;
                    v.Y = v.Y * 2;

                    return v;
                }).ToList();

            return new Polygon(_ssVertices, Color, _thick);
        }

        public void Fill()
        {
            var indices = Vertices
                .OrderBy(v => v.Y)
                .Select(v => Vertices.IndexOf(v))
                .ToArray();

            var AET = new List<Point>();

            int k = 0;
            int N = Vertices.Count;
            int i = indices[k];

            int ymin = (int) Vertices[indices[0]].Y;
            int ymax = (int) Vertices[indices[N - 1]].Y;

            for (int y = ymin; y <= ymax;)
            {
                while (Vertices[i].Y == y)
                {
                    if (Vertices[i - 1].Y > Vertices[i].Y)
                    {
                        AET.Add(Vertices[i], Vertices[i - 1]);
                    }

                    if (Vertices[i + 1].Y > Vertices[i].Y)
                    {

                        AET.Add(Vertices[i], Vertices[i + 1]);
                    }

                    ++k;
                    i = indices[k];
                }

                // sort AET by x value
                // fill pixels between pairs of intersections
                ++y;
                // remove from AET edges for which ymax = y
                foreach (var edge in AET)
                {
                    x += 1 / m
                }
            }
}
        }
    }
}
