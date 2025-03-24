using TicTool.Classes;

if (args.Length > 0 && string.Equals(args[0], "-sprite", StringComparison.InvariantCultureIgnoreCase))
{
  if (args.Length < 3)
  {
    Console.WriteLine("USAGE: TicTool -sprite source.png target.lua namespace.variable");
    return;
  }

  var sourceFilename = args[1];
  var targetFilename = args[2];
  var variable = args[3];

  var imageData = TIC80ImageConverter.ProcessImage(sourceFilename);
  TIC80Tool.SaveData(imageData, variable, targetFilename);
}


/* EXTRACT
var filename = @"C:\Users\paul-\AppData\Roaming\com.nesbox.tic\TIC-80\purpleVortex7.tic";

var file = TIC80Parser.Parse(filename);
var tool = new TIC80Tool(file);

tool.SaveData(ChunkType.Sprites, "Sprites.Presents", @"C:\Projects\Demoscene\PurpleVortex\Source\Sprites\credits.lua");
tool.SaveData(ChunkType.Tiles, "Sprites.Credits", @"C:\Projects\Demoscene\PurpleVortex\Source\Sprites\presents.lua");
tool.SaveMusicData("Music.Main", @"C:\Projects\Demoscene\PurpleVortex\Source\Music\main.lua");

*/

//tool.LoadImage(@"C:\Scratch\tuhblogo.png");

//TIC80Writer.Write(file, filename);
//TIC80Writer.Write(file, Path.ChangeExtension(filename, ".parsed.tic"));

//Console.WriteLine($"{file.Chunks.Count} chunks found:");
//foreach (var chunk in file.Chunks)
//{
//    Console.WriteLine($"- {chunk.ChunkType}");
//}

