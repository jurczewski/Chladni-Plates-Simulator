using MathNet.Numerics.LinearAlgebra;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Chladni_Plates
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            FrequencySlider.Minimum = 300;
            FrequencySlider.Maximum = 6000;

            ParticlesSlider.Minimum = 1;
            ParticlesSlider.Maximum = 100000; //todo: consider width*height;

            //Set bg to gray
            //var width = Convert.ToInt32(PixelBox.Width);
            //var height = Convert.ToInt32(PixelBox.Height);
            //var bitmap = new Bitmap(width, height);
            //BitmapOperations.FillBitmapWithColor(Color.LightGray, ref bitmap);
            //PixelBox.Source = BitmapOperations.BitmapToImageSource(bitmap);
        }

        #region Handlers
        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            var width = 20;//Convert.ToInt32(PixelBox.Width);
            var height = 20;// Convert.ToInt32(PixelBox.Height);
            var numberOfParticles = Convert.ToInt32(ParticlesSlider.Value);

            Plate.Size = width;
            var res = Plate.RunAlgorithm();
            var bitmap = new Bitmap(width, height);

            var max = res.AbsoluteMaximum();
            res = res.Multiply(1 / max);
            res = res.PointwiseAbs();

            BitmapOperations.WritePixelsToBitmap(width, height, ref res, ref bitmap);
            PixelBox.Source = BitmapOperations.BitmapToImageSource(bitmap);
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            using var bitmap = BitmapOperations.BitmapImage2Bitmap((BitmapImage)PixelBox.Source);
            var path = Directory.GetCurrentDirectory();
            bitmap.Save(path + $"/Chladni-Plates-Frequency-{FrequencySlider.Value}Hz-Particles{ParticlesSlider.Value}.png");
        }
        #endregion        
    }
}