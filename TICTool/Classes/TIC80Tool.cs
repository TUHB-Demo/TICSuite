using System.Drawing;
using System.Text;

namespace TicTool.Classes;

public class TIC80Tool
{
  private readonly TIC80File _file;

  public TIC80Tool(TIC80File file)
  {
    _file = file;
  }

  public void LoadImage(string filename)
  {
    var bitmap = (Bitmap)Image.FromFile(filename, true);
    var palette = bitmap.Palette.Entries.ToList();

    var data = new byte[240 * 136 / 2];
    for (var y = 0; y < Math.Min(bitmap.Height, 136); y++)
    {
      for (var x = 0; x < Math.Min(bitmap.Width, 240); x++)
      {
        var color = bitmap.GetPixel(x, y);
        var indexedValue = FindColorIndex(color, palette);

        var index = y * 240 + x;
        var halfIndex = index / 2;
        var value = data[halfIndex];
        if (index % 2 == 0)
          value = (byte)((value & 0xF0) + indexedValue);
        else
          value = (byte)((value & 0x0F) + (indexedValue << 4));
        data[halfIndex] = value;
      }
    }


    //if (bitmap.PixelFormat != PixelFormat.Format4bppIndexed)
    //    bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), PixelFormat.Format4bppIndexed);

    //var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
    //var bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly,bitmap.PixelFormat);

    //var ptr = bmpData.Scan0;
    //var length  = Math.Abs(bmpData.Stride) * bitmap.Height;
    //var data = new byte[length];
    //System.Runtime.InteropServices.Marshal.Copy(ptr, data, 0, length);

    SetChunkData(ChunkType.Screen, data);

    //bitmap.UnlockBits(bmpData);

    foreach (var entry in palette)
    {
      Console.Write($"{entry.R},{entry.G},{entry.B},");
    }
  }

  private static byte FindColorIndex(Color color, List<Color> palette)
  {
    for (byte index = 0; index < palette.Count; index++)
    {
      if (palette[index] == color)
        return index;
    }

    if (palette.Count < 16)
    {
      palette.Add(color);
      return (byte)(palette.Count - 1);
    }


    throw new Exception($"Color {color} could not be matched in the first 16 palette entries");
  }

  private void SetChunkData(ChunkType chunkType, byte[] data)
  {
    var chunk = _file.Chunks.FirstOrDefault(c => c.ChunkType == chunkType);
    if (chunk == null)
    {
      chunk = new TIC80Chunk
      {
        ChunkType = chunkType,
        Data = data,
        Size = (ushort)data.Length
      };
      _file.Chunks.Add(chunk);
    }

    chunk.Data = data;
  }

  public void SaveData(ChunkType chunkType, string variable, string filename)
  {
    var sb = new StringBuilder();
    AppendDataType(variable, 1, sb);
    AppendChunkData(chunkType, -1, variable, sb);
    File.WriteAllText(filename, sb.ToString());
  }

  public static void SaveData(byte[] data, string variable, string filename)
  {
    var sb = new StringBuilder();
    AppendDataType(variable, 1, sb);
    AppendData(data, variable, sb);
    File.WriteAllText(filename, sb.ToString());
  }

  public void SaveMusicData(string variable, string filename)
  {
    var sb = new StringBuilder();
    AppendDataType(variable, 1, sb);
    AppendDataType(variable, 2, sb);

    AppendChunkData(ChunkType.Waveforms, -1, variable + ".Waveforms", sb);
    AppendChunkData(ChunkType.Samples, -1, variable + ".Samples", sb);
    AppendChunkData(ChunkType.Patterns, -1, variable + ".Patterns", sb);
    AppendChunkData(ChunkType.Tracks, -1, variable + ".Tracks", sb);

    File.WriteAllText(filename, sb.ToString());
  }

  private static void AppendDataType(string variable, int nrParts, StringBuilder sb)
  {
    string dataType;
    var parts = variable.Split('.');
    if (parts.Length == 2)
    {
      dataType = string.Join('.', parts.Take(nrParts));
    }
    else
      throw new Exception("Currently only a.b variable format is supported");

    sb.AppendLine($"{dataType} = {dataType} or {{}}");
  }

  private void AppendChunkData(ChunkType chunkType, int length, string variable, StringBuilder sb)
  {
    var chunk = _file.Chunks.FirstOrDefault(c => c.ChunkType == chunkType);
    if (chunk == null)
      throw new Exception($"Chunk {chunkType} not found in .tic file");
    var data = chunk.Data;
    if (length > 0)
      data = data.Take(length).ToArray();
    AppendData(data, variable, sb);
  }

  private static void AppendData(byte[] data, string variable, StringBuilder sb)
  {
    sb.Append($"{variable}={{");
    sb.Append(string.Join(",", data.Select(b => Convert.ToString(b))));
    sb.AppendLine("}");
  }
}