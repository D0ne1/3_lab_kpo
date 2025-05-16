using PluginContracts;
using System.Drawing;
using System;

namespace GrayscaleFilter
{
    [PluginInfo("Оттенки серого", "Даниил", "1.0")]
    public class GrayscaleFilter : IImageFilter
    {
        public string Name => "Grayscale";  // Имя фильтра

        public string Author => "Даниил";  // Имя автора

        public Bitmap Apply(Bitmap image)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int gray = (int)((pixel.R + pixel.G + pixel.B) / 3.0);
                    result.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }

            return result;
        }

        public Version Version => new Version(1, 0);  // Версия фильтра
    }
}
