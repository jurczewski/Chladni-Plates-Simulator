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

            //Algorithm.Size = 50;
            var size = Algorithm.Size;

            FrequencySlider.Minimum = 0;
            FrequencySlider.Maximum = Math.Min(30, size - 1);

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

            double[,] result = new double[size, size];
            double max = int.MinValue;
            for(int i=0; i<size; i++)
            {
                for(int j=0; j<size; j++)
                {
                    if(i*size+j < Algorithm.FixedVertices)
                    {
                        result[i, j] = Math.Abs(oneColumn[i * size + j]);
                    }else if (i*size+j == Algorithm.FixedVertices)
                    {
                        result[i, j] = 0;
                    }
                    else
                    {
                        result[i, j] = Math.Abs(oneColumn[i * size + j - 1]);
                    }
                    max = Math.Max(max, result[i, j]);
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    result[i, j] /= max;
                }
            }
            var toPrint = Algorithm.IncreaseSize(result, 3);

            var bitmap = new Bitmap(size, size);

            BitmapOperations.WritePixelsToBitmap(size, size, ref toPrint, ref bitmap);
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