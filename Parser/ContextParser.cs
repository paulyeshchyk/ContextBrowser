using ContextBrowser.exporter;
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
        var files = Directory.GetFiles(rootPath, $"*{TargetExtension}", SearchOption.AllDirectories);

        var collector = new ContextInfoCollector<ContextInfo>();
        var processor = new ContextInfoCommentProcessor<ContextInfo>(new ContextClassifier());
        var factory = new ContextInfoFactory<ContextInfo>();
        var parser = new RoslynParser<ContextInfo>(collector, factory, processor);

        // Первый проход — сбор всех типов и методов
        foreach(var file in files)
        {
            parser.ParseFile(file, RoslynParserOptions.Default, parseReferences: false);
        }

        // Второй проход — построение ссылок между сущностями
        foreach(var file in files)
        {
            parser.ParseFile(file, RoslynParserOptions.Default, parseContexts: false);
        }

        return collector.Collection;
    }

    // context: read, directory
    private static List<ContextInfo> ParseFile(string filePath)
    {
        var collector = new ContextInfoCollector<ContextInfo>();
        var processor = new ContextInfoCommentProcessor<ContextInfo>(new ContextClassifier());
        var factory = new ContextInfoFactory<ContextInfo>();

        var parser = new RoslynParser<ContextInfo>(collector, factory, processor);
        parser.ParseFile(filePath);

        return collector.Collection;
    }


    private static List<ContextInfo> FirstPass(string[] files)
    {
        var collector = new ContextInfoCollector<ContextInfo>();
        var processor = new ContextInfoCommentProcessor<ContextInfo>(new ContextClassifier());
        var factory = new ContextInfoFactory<ContextInfo>();

        var parser = new RoslynParser<ContextInfo>(collector, factory, processor);

        foreach(var file in files)
        {
            parser.ParseFile(file, RoslynParserOptions.Default, parseReferences: false); // только контексты
        }

        return collector.Collection;
    }

    private static void SecondPass(string[] files, ContextInfoCollector<ContextInfo> collector)
    {
        var processor = new ContextInfoCommentProcessor<ContextInfo>(new ContextClassifier());
        var factory = new ContextInfoFactory<ContextInfo>();
        var parser = new RoslynParser<ContextInfo>(collector, factory, processor);

        foreach(var file in files)
        {
            parser.ParseFile(file, RoslynParserOptions.Default, parseContexts: false); // только ссылки
        }
    }
}
