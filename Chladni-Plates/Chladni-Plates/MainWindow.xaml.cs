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
            var width = Convert.ToInt32(PixelBox.Width);
            var height = Convert.ToInt32(PixelBox.Height);
            var bitmap = new Bitmap(width, height);
            FillBitmapWithColor(Color.LightGray, ref bitmap);
            PixelBox.Source = BitmapToImageSource(bitmap);
        }

        private Bitmap RandomColorPixelBitmap(int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            var random = new Random();

            //create random pixels
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    //generate random ARGB
                    var a = random.Next(256);
                    var r = random.Next(256);
                    var g = random.Next(256);
                    var b = random.Next(256);

                    //set
                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }

            return bitmap;
        }

        private Bitmap RandomPixelsBitmap(int width, int height, int numberOfParticles)
        {
            var bitmap = new Bitmap(width, height);
            FillBitmapWithColor(Color.LightGray, ref bitmap);

            var random = new Random();

            for (int i = 0; i < numberOfParticles; i++)
            {
                var x = random.Next(0, width);
                var y = random.Next(0, height);
                bitmap.SetPixel(x, y, Color.Black);
            }

            return bitmap;
        }

        private void FillBitmapWithColor(Color color, ref Bitmap bitmap)
        {
            using Graphics gfx = Graphics.FromImage(bitmap);
            using SolidBrush brush = new SolidBrush(color);
            gfx.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using MemoryStream memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Position = 0;
            var bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            return bitmapimage;
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using MemoryStream outStream = new MemoryStream();
            var enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapImage));
            enc.Save(outStream);
            var bitmap = new Bitmap(outStream);

            return new Bitmap(bitmap);
        }

        #region Handlers
        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            var width = Convert.ToInt32(PixelBox.Width);
            var height = Convert.ToInt32(PixelBox.Height);
            var numberOfParticles = Convert.ToInt32(ParticlesSlider.Value);

            using var bitmap = RandomPixelsBitmap(width, height, numberOfParticles);
            PixelBox.Source = BitmapToImageSource(bitmap);
        }

        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            using var bitmap = BitmapImage2Bitmap((BitmapImage)PixelBox.Source);
            var path = Directory.GetCurrentDirectory();
            bitmap.Save(path + $"/Chladni-Plates-Frequency-{FrequencySlider.Value}Hz-Particles{ParticlesSlider.Value}.png");
        }
        #endregion


    }
}
