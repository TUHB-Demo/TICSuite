namespace TicTool.Classes;

public class TIC80File
{
    public List<TIC80Chunk> Chunks { get; set; } = new();
}

public class TIC80Chunk
{
    public int BankNumber { get; set; }
    public ChunkType ChunkType { get; set; }
    public ushort Size { get; set; }
    public byte Reserved { get; set; }
    public byte[] Data { get; set; }
}

// ReSharper disable UnusedMember.Global
public enum ChunkType
{
    Tiles = 1,
    Sprites = 2,
    Map = 4,
    Code = 5,
    Flags = 6,
    Samples = 9,
    Waveforms = 10,
    Palette = 12,
    Tracks = 14,
    Patterns = 15,
    CodeZip = 16,
    Default = 17,
    Screen = 18,
    Binary = 19
}