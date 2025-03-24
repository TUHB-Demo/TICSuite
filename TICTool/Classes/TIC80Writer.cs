namespace TicTool.Classes;

public static class TIC80Writer
{
    public static void Write(TIC80File file, string filename)
    {
        var bytes = new List<byte>();

        foreach (var chunk in file.Chunks)
        {
            bytes.Add((byte)((chunk.BankNumber << 5) + (byte)chunk.ChunkType));
            bytes.Add((byte)(chunk.Size & 255));
            bytes.Add((byte)(chunk.Size >> 8));
            bytes.Add(chunk.Reserved);
            if (chunk.Size > 0)
                bytes.AddRange(chunk.Data);
        }

        File.WriteAllBytes(filename, bytes.ToArray());
    }
}