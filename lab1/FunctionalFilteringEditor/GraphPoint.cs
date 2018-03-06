using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FunctionalFilteringEditor
{
    public class GraphPoint
    {
        public GraphPoint(bool canRemove = true)
        {
            CanRemove = canRemove;
        }

        public static GraphPoint Create(double x, double y, bool canRemove = true)
        {
            return new GraphPoint(canRemove)
            {
                UIElement = CreateEllipse(),
                PositionX = x,
                PositionY = y
            };
        }

        public Ellipse UIElement { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public bool CanRemove { get; private set; }

        private static Ellipse CreateEllipse()
        {
            var ellipse = new Ellipse();
            ellipse.Height = 8;
            ellipse.Width = 8;
            ellipse.Fill = Brushes.Red;

            return ellipse;
        }
    }

    public class GraphPointComparer : IComparer<GraphPoint>
    {
        public int Compare(GraphPoint a, GraphPoint b)
        {
            if ((a.PositionX == b.PositionX))
                return 0;
            if ((a.PositionX < b.PositionX))
                return -1;

            return 1;
        }
    }
}
