using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TicTool.Classes;

public static class TIC80ImageConverter
{
  public static byte[] ProcessImage(string filename)
  {
    using var bitmap = new Bitmap(filename);
    var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
    var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

    var ptr = data.Scan0;
    var size = Math.Abs(data.Stride) * bitmap.Height;
    var pixelValues = new byte[size];

    Marshal.Copy(ptr, pixelValues, 0, size);

    var alignedWidth = (bitmap.Width + 7) & ~7;
    var alignedHeight = (bitmap.Height + 7) & ~7;

    if (alignedWidth > 128 || alignedHeight > 128)
      throw new Exception("Image is too large");

    var nrTilesX = alignedWidth / 8;
    var nrTilesY = alignedHeight / 8;
    var bytes = new byte[nrTilesX * nrTilesY * 4 * 8];
    var byteIndex = 0;
    byte byteValue = 0;

    for (var tileY = 0; tileY < nrTilesY; tileY++)
    {
      for (var tileX = 0; tileX < nrTilesX; tileX++)
      {
        for (var y = 0; y < 8; y++)
        {
          var sourceY = tileY * 8 + y;
          for (var x = 0; x < 8; x++)
          {
            if (x % 2 == 0)
              byteValue = 0;

            var sourceX = tileX * 8 + x;
            byte value = 0;
            if (sourceY < bitmap.Height || sourceX < bitmap.Width)
            {
              int index = sourceY * data.Stride + sourceX * 4;

              byte b = pixelValues[index + 0];
              byte g = pixelValues[index + 1];
              byte r = pixelValues[index + 2];
              byte a = pixelValues[index + 3];

              var brightness = 0.299 * r + 0.587 * g + 0.114 * b;
              brightness *= a / 255.0;
              value = (byte)(brightness / 16);
            }

            if (x % 2 == 1)
              value = (byte)(value << 4);
            byteValue |= value;
            if (x % 2 == 1)
              bytes[byteIndex++] = byteValue;
          }
        }

      }
    }

    bitmap.UnlockBits(data);
    return bytes;
  }
}