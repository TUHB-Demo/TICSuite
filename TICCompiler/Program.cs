using TICCompiler.Classes;

var parameters = Parameters.Parse(args);
if (parameters.Arguments.Count != 2)
{
  Console.WriteLine("TIC Compiler");
  Console.WriteLine("Usage: tcc [main.lua] [output.lua] [flags]");
  Console.WriteLine("");
  Console.WriteLine("By default, tcc will compile the entire tree of lua files referenced by main.lua into a single large [output.lua] file");
  Console.WriteLine("If a -w or -watch flag is added, the files will also be watched for changes and recompiled on the fly.");
  Environment.Exit(1);
}

try
{
  var processor = new LuaProcessor();
  var result = processor.Process(parameters.Arguments[0]);
  File.WriteAllText(parameters.Arguments[1], result);
}
catch (Exception e)
{
  Console.WriteLine($"ERROR: {e.Message}");
}



