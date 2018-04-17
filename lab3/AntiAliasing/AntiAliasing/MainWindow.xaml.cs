using AntiAliasing.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

namespace AntiAliasing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Bitmap canvas;
        private readonly BitmapProcessor bitmapProcessor;

        public MainWindow()
        {
            InitializeComponent();

            this.canvas = new Bitmap((int)Canvas.Width, (int)Canvas.Height);
            this.bitmapProcessor = new BitmapProcessor(canvas);

            bitmapProcessor.ClearBitmap();
            bitmapProcessor.DrawLine(0, 0, (int)Canvas.Width, (int)Canvas.Height);

            Canvas.ShowBitmap(canvas);
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            BitmapData bitmapData = canvas.Lock();

            for (int y = 0; y < 50; y++)
            {
                for (int x = 0; x < 50; x++)
                {
                    bitmapData.SetPixel(canvas, x, y, 255);
                }
            }

            canvas.Unlock(bitmapData);
        }

        public void ClearBitmap()
        {
            bitmapProcessor.ClearBitmap();
        }
    }
}
