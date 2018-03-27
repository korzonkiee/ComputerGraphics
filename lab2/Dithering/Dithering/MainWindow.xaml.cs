using Dithering.ColorQuantization;
using Dithering.ImageProcessors;
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
        private readonly GreyScaleImageProcessor greyScaleImageProcessor;
        private readonly DitheringImageProcessor ditheringImageProcessor;
        private readonly OctreeQuantImageProcessor octreeQuantImageProcessor;
        private readonly PopularityQuantImageProcessor popularityQuantImageProcessor;

        private const int DefaultGreyLevel = 2;
        private const int DefaultDitherMatrix = 2;
        private const int DefaultPaletteSize = 8;

        private Bitmap originalBitmap;
        private Bitmap resultBitmap;

        public MainWindow()
        {
            InitializeComponent();

            this.greyScaleImageProcessor = new GreyScaleImageProcessor();
            this.ditheringImageProcessor = new DitheringImageProcessor();
            this.octreeQuantImageProcessor = new OctreeQuantImageProcessor();
            this.popularityQuantImageProcessor = new PopularityQuantImageProcessor();

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
            greyScaleImageProcessor.Process(originalBitmap);

            int greyLevels;
            int.TryParse(GreyLevelsTB.Text, out greyLevels);
            if (greyLevels < 2)
                greyLevels = DefaultGreyLevel;

            ditheringImageProcessor.GreyLevels = greyLevels;
            ditheringImageProcessor.DitheringType = DitheringType.Random;
            ditheringImageProcessor.Process(resultBitmap);

            LoadBitmapIntoContainer(resultBitmap);
        }

        private void OnApplyOrdererdDithering(object sender, RoutedEventArgs e)
        {
            greyScaleImageProcessor.Process(originalBitmap);

            int ditherMatrixSize;
            int.TryParse(DitherMatrixSizeTB.Text, out ditherMatrixSize);
            if (ditherMatrixSize < 2)
                ditherMatrixSize = DefaultDitherMatrix;

            int greyLevels;
            int.TryParse(GreyLevelsTB.Text, out greyLevels);
            if (greyLevels < 2)
                greyLevels = DefaultGreyLevel;

            ditheringImageProcessor.GreyLevels = greyLevels;
            ditheringImageProcessor.DitherMatrixSize = ditherMatrixSize;
            ditheringImageProcessor.DitheringType = DitheringType.Ordered;
            ditheringImageProcessor.Process(resultBitmap);

            LoadBitmapIntoContainer(resultBitmap);
        }

        private void OnApplyPopularityAlgorithm(object sender, RoutedEventArgs e)
        {
            int paletteSize;
            int.TryParse(ColorPalleteSizeTB.Text, out paletteSize);
            if (paletteSize < 2)
                paletteSize = DefaultPaletteSize;

            popularityQuantImageProcessor.PaletteSize = paletteSize;
            popularityQuantImageProcessor.Process(resultBitmap);

            LoadBitmapIntoContainer(resultBitmap);
        }

        private void OnApplyOctreeColorAlgorithm(object sender, RoutedEventArgs e)
        {
            int paletteSize;
            int.TryParse(ColourPaletteMaxSizeTB.Text, out paletteSize);
            if (paletteSize < 2)
                paletteSize = DefaultPaletteSize;

            octreeQuantImageProcessor.PaletteSize = paletteSize;
            octreeQuantImageProcessor.Process(resultBitmap);

            LoadBitmapIntoContainer(resultBitmap);
        }
    }
}
