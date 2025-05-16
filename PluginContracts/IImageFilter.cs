using System.Drawing;
using System;
namespace PluginContracts
{
    public interface IImageFilter
    {
        string Name { get; }
        string Author { get; }
        Version Version { get; }

        Bitmap Apply(Bitmap image);
    }
}
