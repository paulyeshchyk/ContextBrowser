using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using ContextBrowser.Extensions;

namespace ContextBrowser.SourceKit.Roslyn;

// context: parser, directory, ContextInfo, build
public class RoslynContextParser
{
    private const string TargetExtension = ".cs";

    // context: read, directory, ContextInfo
    // layer: 900
    public static List<ContextInfo> Parse(string rootPath, IEnumerable<string> assembliesPaths)
    {
        var pathType = PathAnalyzer.GetPathType(rootPath);
        return pathType switch
        {
            PathAnalyzer.PathType.File => ParseFile(rootPath),
            PathAnalyzer.PathType.Directory => ParseDirectory(rootPath, assembliesPaths),
            _ => throw new NotImplementedException()
        };
    }

    // context: read, directory
    private static List<ContextInfo> ParseDirectory(string rootPath, IEnumerable<string> assembliesPaths)
    {
        var files = Directory.GetFiles(rootPath, $"*{TargetExtension}", SearchOption.AllDirectories);

        // 1. Первый проход: собираем все context'ы (типы и методы), без ссылок
        var contextCollector = new ContextInfoCollector<ContextInfo>();
        var processor = new ContextInfoCommentProcessor<ContextInfo>(new ContextClassifier());
        var factory = new ContextInfoFactory<ContextInfo>();

        var parser1 = new RoslynCodeParser<ContextInfo>(contextCollector, factory, processor);

        foreach(var file in files)
        {
            parser1.ParseFile(file, RoslynCodeParserOptions.Default(assembliesPaths), RoslynContextParserOptions.ContextsOptions);
        }

        var allContexts = contextCollector.Collection;

        // 2. Второй проход: строим ссылки на основе уже собранных
        var referenceCollector = new ContextInfoReferenceCollector<ContextInfo>(allContexts);
        var parser2 = new RoslynCodeParser<ContextInfo>(referenceCollector, factory, processor);

        foreach(var file in files)
        {
            parser2.ParseFile(file, RoslynCodeParserOptions.Default(assembliesPaths), RoslynContextParserOptions.ReferencesOptions);
        }

        return allContexts;
    }

    private static List<ContextInfo> ParseFile(string filePath)
    {
        var collector = new ContextInfoCollector<ContextInfo>();
        var processor = new ContextInfoCommentProcessor<ContextInfo>(new ContextClassifier());
        var factory = new ContextInfoFactory<ContextInfo>();

        var parser = new RoslynCodeParser<ContextInfo>(collector, factory, processor);
        parser.ParseFile(filePath, RoslynCodeParserOptions.Default(), RoslynContextParserOptions.ContextsOptions);

        return collector.Collection;
    }
}

public record struct RoslynContextParserOptions
{
    public RoslynContextParserOptions()
    {
    }

    public bool ParseContexts { get; set; } = true;

    public bool ParseReferences { get; set; } = true;

    public static RoslynContextParserOptions ContextsOptions = new RoslynContextParserOptions() { ParseContexts = true, ParseReferences = false };
    public static RoslynContextParserOptions ReferencesOptions = new RoslynContextParserOptions() { ParseContexts = false, ParseReferences = true };
}
