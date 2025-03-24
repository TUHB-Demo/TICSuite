namespace TicTool.Classes;

public static class TIC80Parser
{
    public static TIC80File Parse(string filename)
    {
        return Parse(File.ReadAllBytes(filename));
    }

    public static TIC80File Parse(byte[] data)
    {
        var file = new TIC80File();
        var index = 0;
        while (index < data.Length)
        {
            var chunk = new TIC80Chunk
            {
                ChunkType = (ChunkType)(data[index] & 31),
                BankNumber = data[index] >> 5,
                Size = (ushort)((data[index + 2] << 8) + data[index + 1]),
                Reserved = data[index + 3]
            };
            Console.WriteLine($"Offset {index}, Chunk {chunk.ChunkType}, {chunk.Size} bytes");
            if (chunk.Size > 0)
                chunk.Data = data[(index + 4)..(index + 4 + chunk.Size)];
            file.Chunks.Add(chunk);
            index += chunk.Size + 4;
        }

        return file;
    }
}