using AntiAliasing.Extensions;
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

        private double _y_min = double.MaxValue;
        private double _y_max = double.MinValue;

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

        public void Fill(BitmapData bitmapData)
        {
            var et = CreateEdgeTable();
            var aet = new List<Edge>();
            bool isSet = false;

            double y = _y_min;
            while (y < _y_max)
            {
                try
                {
                    aet.AddRange(et[y]);
                }
                catch { }
                aet = aet.OrderBy(e => e.X_Min).ToList();

                for (int i = 0; i < aet.Count; i+=2)
                {
                    var el = aet[i];
                    var er = aet[i + 1];

                    for(int x = (int) el.X_Min; x < Math.Ceiling(er.X_Min); x++)
                    {
                        bitmapData.SetPixel(x, (int)y, 0);
                    }
                }
                y++;
                aet = aet.Where(e => e.Y_Max != y).ToList();
                for (int i = 0; i < aet.Count; i++)
                {
                    aet[i].X_Min += aet[i].X_Change;
                }
            }
        }

        public class Edge
        {
            public double X_Min { get; set; }
            public double Y_Min { get; set; }
            public double Y_Max { get; set; }
            public double X_Change { get; set; }
        }

        private Dictionary<double, List<Edge>> CreateEdgeTable()
        {
            var et = new Dictionary<double, List<Edge>>();

            for (int i = 0; i < Vertices.Count - 1; i++)
            {
                AddEdge(et, (int) Vertices[i].X, (int) Vertices[i].Y, (int) Vertices[i + 1].X, (int) Vertices[i + 1].Y);
            }

            AddEdge(et, (int) Vertices[Vertices.Count - 1].X, (int) Vertices[Vertices.Count - 1].Y, (int) Vertices[0].X, (int) Vertices[0].Y);
            return et;
        }

        private void AddEdge(Dictionary<double, List<Edge>> et, int p1_x, int p1_y, int p2_x, int p2_y)
        {
            double dy = (p2_y - p1_y);
            double dx = (p2_x - p1_x);

            if (dy == 0)
                return;

            double change = dx / dy;

            double y_max;
            double y_min;
            double x_min;

            if (p1_y > p2_y)
            {
                y_max = p1_y;
                y_min = p2_y;
                x_min = p2_x;
            }
            else
            {
                y_max = p2_y;
                y_min = p1_y;
                x_min = p1_x;
            }

            if (y_max > _y_max)
                _y_max = y_max;
            if (y_min < _y_min)
                _y_min = y_min;

            var edge = new Edge()
            {
                X_Min = x_min,
                Y_Max = y_max,
                X_Change = change
            };

            try
            {
                var edgeBucket = et[y_min];
                edgeBucket.Add(edge);
            }
            catch (KeyNotFoundException e)
            {
                var edgeBucket = new List<Edge>() { edge };
                et[y_min] = edgeBucket;
            }
        }
    }
}
