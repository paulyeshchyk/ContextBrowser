using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Roslyn;
using ContextBrowser.Extensions;

namespace ContextBrowser.ContextKit.Parser;

// context: parser, directory, csharp, build
public class ContextParser
{
    private const string TargetExtension = ".cs";

    // context: read, directory
    // layer: 900
    public static List<ContextInfo> Parse(string rootPath)
    {
        var pathType = PathAnalyzer.GetPathType(rootPath);
        return pathType switch
        {
            PathAnalyzer.PathType.File => ParseFile(rootPath),
            PathAnalyzer.PathType.Directory => ParseDirectory(rootPath),
            _ => throw new NotImplementedException()
        };
    }

    // context: read, directory
    private static List<ContextInfo> ParseDirectory(string rootPath)
    {
        var files = Directory.GetFiles(rootPath, $"*{TargetExtension}", SearchOption.AllDirectories);

        // 1. Первый проход: собираем все context'ы (типы и методы), без ссылок
        var contextCollector = new ContextInfoCollector<ContextInfo>();
        var processor = new ContextInfoCommentProcessor<ContextInfo>(new ContextClassifier());
        var factory = new ContextInfoFactory<ContextInfo>();

        var parser1 = new RoslynParser<ContextInfo>(contextCollector, factory, processor);

        foreach(var file in files)
        {
            parser1.ParseFile(file, RoslynParserOptions.Default, parseReferences: false);
        }

        var allContexts = contextCollector.Collection;

        // 2. Второй проход: строим ссылки на основе уже собранных
        var referenceCollector = new ContextInfoReferenceCollector<ContextInfo>(allContexts);
        var parser2 = new RoslynParser<ContextInfo>(referenceCollector, factory, processor);

        foreach(var file in files)
        {
            parser2.ParseFile(file, RoslynParserOptions.Default, parseContexts: false, parseReferences: true);
        }

        return allContexts;
    }

    private static List<ContextInfo> ParseFile(string filePath)
    {
        var collector = new ContextInfoCollector<ContextInfo>();
        var processor = new ContextInfoCommentProcessor<ContextInfo>(new ContextClassifier());
        var factory = new ContextInfoFactory<ContextInfo>();

        var parser = new RoslynParser<ContextInfo>(collector, factory, processor);
        parser.ParseFile(filePath);

        return collector.Collection;
    }
}
