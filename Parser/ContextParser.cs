using ContextBrowser.Extensions;
using ContextBrowser.model;
using ContextBrowser.Parser.Roslyn;

namespace ContextBrowser.Parser;

// context: parser, file, directory, csharp, build
public class ContextParser
{
    private const string TargetExtension = ".cs";

    // context: read, file, directory
    // layer: 900
    public static List<ContextInfo> Parse(string rootPath)
    {
        var pathType = PathAnalyzer.GetPathType(rootPath);
        return pathType switch
        {
            PathAnalyzer.PathType.File => ParseFile(rootPath),
            PathAnalyzer.PathType.Directory => ParseDirectory(rootPath),
            PathAnalyzer.PathType.SymbolicLink => throw new NotImplementedException(),
            PathAnalyzer.PathType.NonExistentPath => throw new NotImplementedException(),
            PathAnalyzer.PathType.PlainText => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };
    }

    // context: read, directory
    private static List<ContextInfo> ParseDirectory(string rootPath)
    {
        var results = new List<ContextInfo>();
        var files = Directory.GetFiles(rootPath, $"*{TargetExtension}", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            results.AddRange(ParseFile(file));
        }

        return results;
    }

    private static List<ContextInfo> ParseFile(string filePath)
    {
        var collector = new ContextInfoCollector<ContextInfo>();
        var processor = new ContextInfoCommentProcessor<ContextInfo>();
        var factory = new ContextInfoFactory<ContextInfo>();

        var parser = new RoslynParser<ContextInfo>(collector, factory, processor);
        parser.ParseFile(filePath);

        return collector.Collection;
    }
}
