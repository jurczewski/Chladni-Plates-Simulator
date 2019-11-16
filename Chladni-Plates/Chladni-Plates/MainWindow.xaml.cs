using MathNet.Numerics.LinearAlgebra;
using System;
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
            var width = Convert.ToInt32(PixelBox.Width);
            var height = Convert.ToInt32(PixelBox.Height);
            //var bitmap = new Bitmap(width, height);
            //BitmapOperations.FillBitmapWithColor(Color.LightGray, ref bitmap);
            //PixelBox.Source = BitmapOperations.BitmapToImageSource(bitmap);

            Algorithm();
        }

        #region Handlers
        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            var width = Convert.ToInt32(PixelBox.Width);
            var height = Convert.ToInt32(PixelBox.Height);
            var numberOfParticles = Convert.ToInt32(ParticlesSlider.Value);

            using var bitmap = BitmapOperations.RandomPixelsBitmap(width, height, numberOfParticles);
            PixelBox.Source = BitmapOperations.BitmapToImageSource(bitmap);
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            using var bitmap = BitmapOperations.BitmapImage2Bitmap((BitmapImage)PixelBox.Source);
            var path = Directory.GetCurrentDirectory();
            bitmap.Save(path + $"/Chladni-Plates-Frequency-{FrequencySlider.Value}Hz-Particles{ParticlesSlider.Value}.png");
        }
        #endregion

        public void Algorithm()
        {
            int size = Plate.Size;

            var S = Matrix<double>.Build.Dense(size, size);
            var M = Matrix<double>.Build.Dense(size, size);

            for (int i = 1; i <= Plate.NumberOfTriangles; i++)
            {
                var trianglePoints = Plate.GetTrianglePoints(i);
                for(int j=0; j<3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        var valueFromS = S.At(trianglePoints[j].I, trianglePoints[k].I);
                        valueFromS += Plate.Stiffness.At(j, k);
                        S.At(trianglePoints[j].I, trianglePoints[k].I, valueFromS);

                        var valueFromM = M.At(trianglePoints[j].I, trianglePoints[k].I);
                        valueFromM += Plate.Mass.At(j, k);
                        M.At(trianglePoints[j].I, trianglePoints[k].I, valueFromM);
                    }
                }
            }

            var centerTriagnle = size / 2 * size / 2 + size;
            S = S.RemoveColumn(centerTriagnle);
            S = S.RemoveRow(centerTriagnle);
            M = M.RemoveColumn(centerTriagnle);
            M = M.RemoveRow(centerTriagnle);
        }
    }
}
