using System.Collections.Generic;
using System.Drawing;

namespace TileMapper
{
    public class Tile
    {
        public Tile()
        {
            Pixels = new List<Pixel>();
        }

        public string TileType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public List<Pixel> Pixels { get; set; }
        public System.Windows.Media.Imaging.CroppedBitmap CroppedBitmapImage { get; set; }
    }

    public class Pixel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; }
    }
}
