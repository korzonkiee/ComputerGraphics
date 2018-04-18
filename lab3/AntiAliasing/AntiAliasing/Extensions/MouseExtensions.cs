using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AntiAliasing.Extensions
{
    public static class Mouse
    {
        public static Point GetMousePosition(FrameworkElement relativeTo)
        {
            var point = System.Windows.Input.Mouse.GetPosition(relativeTo);
            return new Point()
            {
                X = point.X,
                Y = relativeTo.ActualHeight - point.Y
            };
        }
    }
}
