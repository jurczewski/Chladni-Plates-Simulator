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
        public Matrix<double> AllValues { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            var size = Plate.Size = 25;

            FrequencySlider.Minimum = 0;
            FrequencySlider.Maximum = size - 1;

            AllValues = Plate.RunAlgorithm();

            //Set bg to gray
            var bitmap = new Bitmap(size, size);
            BitmapOperations.FillBitmapWithColor(Color.LightGray, ref bitmap);
            PixelBox.Source = BitmapOperations.BitmapToImageSource(bitmap);
        }

        #region Handlers
        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            var size = Plate.Size;

            var frequency = Convert.ToInt32(FrequencySlider.Value);
            var oneColumn = AllValues.Column(frequency);

            var bitmap = new Bitmap(size, size);

            var max = oneColumn.AbsoluteMaximum();
            oneColumn = oneColumn.Multiply(1 / max);
            oneColumn = oneColumn.PointwiseAbs();

            BitmapOperations.WritePixelsToBitmap(size, size, ref oneColumn, ref bitmap);
            PixelBox.Source = BitmapOperations.BitmapToImageSource(bitmap);
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            using var bitmap = BitmapOperations.BitmapImage2Bitmap((BitmapImage)PixelBox.Source);
            var path = Directory.GetCurrentDirectory();
            bitmap.Save(path + $"/Chladni-Plates-Frequency-{FrequencySlider.Value}Hz.png");
        }
        #endregion        
    }
}