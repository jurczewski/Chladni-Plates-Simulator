using MathNet.Numerics.LinearAlgebra;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Chladni_Plates
{
    public static class BitmapOperations
    {
        public static void WritePixelsToBitmap(int width, int height, ref Vector<double> values, ref Bitmap bitmap)
        {
            int idx = 0;
            //create random pixels
            for (var y = 0; y < height - 1; y++)
            {
                for (var x = 0; x < width - 1; x++)
                {
                    var a = 255;
                    var r = (int)(values[idx] * 255);
                    var g = (int)(values[idx] * 255);
                    var b = (int)(values[idx] * 255);

                    //set
                    bitmap.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                    idx++;
                }
            }
        }

        public static Bitmap RandomColorPixelBitmap(int width, int height)
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

        public static Bitmap RandomPixelsBitmap(int width, int height, int numberOfParticles)
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

        public static void FillBitmapWithColor(Color color, ref Bitmap bitmap)
        {
            using Graphics gfx = Graphics.FromImage(bitmap);
            using SolidBrush brush = new SolidBrush(color);
            gfx.FillRectangle(brush, 0, 0, bitmap.Width, bitmap.Height);
        }

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
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

        public static Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using MemoryStream outStream = new MemoryStream();
            var enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapImage));
            enc.Save(outStream);
            var bitmap = new Bitmap(outStream);

            return new Bitmap(bitmap);
        }
    }
}
