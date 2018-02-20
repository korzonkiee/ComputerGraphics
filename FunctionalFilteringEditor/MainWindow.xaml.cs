using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FunctionalFilteringEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Line xAxis;
        private Line yAxis;

        public MainWindow()
        {
            InitializeComponent();

            drawAxis();
        }

        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var menuItem = new MenuItem();
            menuItem.Header = "MenuItem";
            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItem);
            canvas.ContextMenu = contextMenu;
        }

        private void addTextBoxMenuItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void drawAxis()
        {
            xAxis = new Line();
            yAxis = new Line();

            xAxis.Stroke = Brushes.Black;
            yAxis.Stroke = Brushes.Black;

            xAxis.X1 = 0;
            xAxis.X2 = 255;
            xAxis.Y1 = 0;
            xAxis.Y2 = 0;

            yAxis.X1 = 0;
            yAxis.X2 = 0;
            yAxis.Y1 = 0;
            yAxis.Y2 = 255;

            xAxis.StrokeThickness = 3;
            yAxis.StrokeThickness = 3;

            canvas.Children.Add(xAxis);
            canvas.Children.Add(yAxis);
        }
    }
}
