using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;

namespace Chladni_Plates
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int NUMBER_OF_PARTICLES = 2000;

        public MainWindow()
        {
            InitializeComponent();

            FrequencySlider.Minimum = 300;
            FrequencySlider.Maximum = 6000;

            int width = Convert.ToInt32(PixelBox.Width);
            int height = Convert.ToInt32(PixelBox.Height);

            //var bitmap = RandomColorPixelBitmap(width, height);
            var bitmap = RandomPixelsBitmap(width, height);

            PixelBox.Source = BitmapToImageSource(bitmap);
        }

        private Bitmap RandomColorPixelBitmap(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            Random random = new Random();

            //create random pixels
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //generate random ARGB
                    int a = random.Next(256);
                    int r = random.Next(256);
                    int g = random.Next(256);
                    int b = random.Next(256);

                    //set
                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }

            return bitmap;
        }

        private Bitmap RandomPixelsBitmap(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            FillBitmapWithColor(Color.LightGray, ref bitmap);

            Random random = new Random();

            for (int i = 0; i < NUMBER_OF_PARTICLES; i++)
            {
                int x = random.Next(0, width);
                int y = random.Next(0, height);
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
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            return bitmapimage;
        }
    }
}
