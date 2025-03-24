using System.Text;
using System.Text.RegularExpressions;

namespace TICCompiler.Classes;

public class LuaProcessor
{
  public string Process(string fileName)
  {
    var context = new ProcessingContext();
    Process(fileName, context);

    var sb = new StringBuilder();
    foreach (var item in context.Dependencies)
    {
      sb.Append(item.Content);
      if (!item.Content.EndsWith("\r\n"))
        sb.Append("\r\n");
    }
    return sb.ToString();
  }

  private static void Process(string filename, ProcessingContext context)
  {
    if (context.PrioritizeDependency(filename))
      return;

    var directory = Path.GetDirectoryName(filename) ?? "";
    var lines = File.ReadAllLines(filename).ToList();
    var processedLines = new List<string>();
    foreach (var rawLine in lines)
    {
      var line = rawLine.Replace("  ", " ").TrimEnd();
      
      var pattern = @"require\s*\((['""])(.*?)\1\)";
      var match = Regex.Match(line, pattern);

      if (match.Success)
      {
        var requiredFilename = Path.Combine(directory, match.Groups[2].Value).ToLower();
        Process(requiredFilename, context);
      }
      else
        processedLines.Add(line);
    }
    var content = string.Join("\r\n", processedLines);
    context.RegisterDependency(filename, content);
  }

  // ----------------------------------------------------------------------------------------------------

  private class ProcessingContext
  {
    public List<ProcessingContextItem> Dependencies { get; } = new();

    public bool PrioritizeDependency(string filename)
    {
      var index = Dependencies.FindIndex(i => i.Filename == filename);
      if (index != -1)
      {
        // If found, move the item to the front
        Dependencies.RemoveAt(index);
        Dependencies.Insert(0, Dependencies[index]);
      }

      return index != -1;
    }

    public void RegisterDependency(string filename, string content)
    {
      var item = new ProcessingContextItem
      {
        Filename = filename,
        Content = content
      };
      Dependencies.Add(item);
    }
  }

  private class ProcessingContextItem
  {
    public string Filename { get; init; }
    public string Content { get; init; }
  }
}