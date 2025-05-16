using PluginContracts;
using System.Drawing;
using System.Linq;
using System;

namespace MedianFilter
{
    [PluginInfo("Медианный фильтр (усиленный)", "Даниил", "1.1")]
    public class MedianFilter : IImageFilter
    {
        public string Name => "Median Filter (5x5)";
        public string Author => "Даниил";

        // Можно легко изменить размер ядра:
        private const int KernelSize = 5;
        private const int KernelRadius = KernelSize / 2;

        public Bitmap Apply(Bitmap image)
        {
            Bitmap result = new Bitmap(image.Width, image.Height);

            for (int y = KernelRadius; y < image.Height - KernelRadius; y++)
            {
                for (int x = KernelRadius; x < image.Width - KernelRadius; x++)
                {
                    int[] rList = new int[KernelSize * KernelSize];
                    int[] gList = new int[KernelSize * KernelSize];
                    int[] bList = new int[KernelSize * KernelSize];
                    int k = 0;

                    for (int j = -KernelRadius; j <= KernelRadius; j++)
                    {
                        for (int i = -KernelRadius; i <= KernelRadius; i++)
                        {
                            Color neighbor = image.GetPixel(x + i, y + j);
                            rList[k] = neighbor.R;
                            gList[k] = neighbor.G;
                            bList[k] = neighbor.B;
                            k++;
                        }
                    }

                    int r = rList.OrderBy(v => v).ElementAt(rList.Length / 2);
                    int g = gList.OrderBy(v => v).ElementAt(gList.Length / 2);
                    int b = bList.OrderBy(v => v).ElementAt(bList.Length / 2);

                    result.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return result;
        }

        public Version Version => new Version(1, 1);
    }
}