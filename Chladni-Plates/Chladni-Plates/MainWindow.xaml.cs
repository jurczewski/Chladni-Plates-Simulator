using MathNet.Numerics.LinearAlgebra;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Chladni_Plates
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double[,] AllValues { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            var size = Algorithm.Size = 50;

            FrequencySlider.Minimum = 0;
            FrequencySlider.Maximum = size - 1;

            Algorithm.SolveSystem();
            AllValues = Algorithm.EigenVectors;

            //Set bg to gray
            var bitmap = new Bitmap(size, size);
            BitmapOperations.FillBitmapWithColor(Color.LightGray, ref bitmap);
            PixelBox.Source = BitmapOperations.BitmapToImageSource(bitmap);
        }

        #region Handlers
        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            var size = Algorithm.Size;

            var frequency = Convert.ToInt32(FrequencySlider.Value);
            
            var oneColumn = CustomArray<double>.GetColumn(AllValues, frequency);

            var bitmap = new Bitmap(size, size);

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