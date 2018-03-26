using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using Image = System.Windows.Controls.Image;

namespace Dithering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DitheringProcessor dithering;
        private readonly GreyScaleConverter greyScaleConverter;

        private const int DefaultGreyLevel = 2;
        private const int DefaultDitherMatrix = 2;

        private Bitmap originalBitmap;
        private Bitmap resultBitmap;

        public MainWindow()
        {
            InitializeComponent();

            this.dithering = new DitheringProcessor();
            this.greyScaleConverter = new GreyScaleConverter();

            GreyLevelsTB.Text = DefaultGreyLevel.ToString();
            DitherMatrixSizeTB.Text = DefaultDitherMatrix.ToString();
        }

        private void OnLoadImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                originalBitmap = new Bitmap(op.FileName);
                greyScaleConverter.Convert(originalBitmap);
                
                resultBitmap = (Bitmap) originalBitmap.Clone();

                LoadBitmapIntoContainer(originalBitmap, true);
                LoadBitmapIntoContainer(resultBitmap, false);
            }
        }

        private void LoadBitmapIntoContainer(Bitmap bitmap, bool isOriginal = false)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                if (isOriginal)
                    ImageContainer.Source = bitmapImage;
                else ResultContainer.Source = bitmapImage;
            }
        }

        private void OnApplyRandomDithering(object sender, RoutedEventArgs e)
        {

            int greyLevels;
            int.TryParse(GreyLevelsTB.Text, out greyLevels);
            if (greyLevels < 2)
                greyLevels = DefaultGreyLevel;

            dithering.ApplyRandomDithering(resultBitmap, greyLevels);
            LoadBitmapIntoContainer(resultBitmap);
        }

        private void OnApplyOrdererdDithering(object sender, RoutedEventArgs e)
        {
            int ditherMatrixSize;
            int.TryParse(DitherMatrixSizeTB.Text, out ditherMatrixSize);
            if (ditherMatrixSize < 2)
                ditherMatrixSize = DefaultDitherMatrix;

            int greyLevels;
            int.TryParse(GreyLevelsTB.Text, out greyLevels);
            if (greyLevels < 2)
                greyLevels = DefaultGreyLevel;

            dithering.ApplyOrdererdDithering(resultBitmap, ditherMatrixSize, greyLevels);
            LoadBitmapIntoContainer(resultBitmap);
        }

        private void OnApplyPopularityAlgorithm(object sender, RoutedEventArgs e)
        {

        }

        private void OnApplyOctreeColorAlgorithm(object sender, RoutedEventArgs e)
        {

        }
    }
}
