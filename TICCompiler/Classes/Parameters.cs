namespace TICCompiler.Classes;

public class Parameters
{
  public List<string> Arguments { get; } = new();
  public List<string> Flags { get; set; } = new();

  public static Parameters Parse(string[] args)
  {
    var result = new Parameters();
    foreach (var arg in args)
    {
      if (arg.StartsWith("-") && args.Length > 1)
        result.Flags.Add(arg.Substring(1,0));
      else
        result.Arguments.Add(arg);
    }
    return result;
  }

  public bool ContainsFlag(string flag)
  {
    return Flags.Any(f => string.Equals(f, flag, StringComparison.InvariantCultureIgnoreCase));
  }
}