using ContextBrowser.ContextKit.Model;
using ContextBrowser.ContextKit.Parser;
using ContextBrowser.Extensions;
using ContextBrowser.LoggerKit;

namespace ContextBrowser.SourceKit.Roslyn;

// context: csharp, parser, directory, contextInfo, build

public class RoslynContextParser
{
    private const string TargetExtension = ".cs";

    // context: csharp, read, directory, ContextInfo
    // layer: 900
    public static List<ContextInfo> Parse(string rootPath, IEnumerable<string> assembliesPaths, OnWriteLog? onWriteLog = null)
    {
        var pathType = PathAnalyzer.GetPathType(rootPath);
        return pathType switch
        {
            PathAnalyzer.PathType.File => ParseFile(rootPath, onWriteLog),
            PathAnalyzer.PathType.Directory => ParseDirectory(rootPath, assembliesPaths, onWriteLog),
            _ => throw new NotImplementedException()
        };
    }

    // context: read, directory, csharp, contextInfo
    public static List<ContextInfo> ParseDirectory(string rootPath, IEnumerable<string> assembliesPaths, OnWriteLog? onWriteLog = null)
    {
        var files = Directory.GetFiles(rootPath, $"*{TargetExtension}", SearchOption.AllDirectories);

        var semanticModelStorage = new RoslynCodeSemanticTreeModelStorage();

        // 1. Первый проход: собираем все context'ы (типы и методы), без ссылок
        var contextCollector = new ContextInfoCollector<ContextInfo>();
        var processor = new ContextInfoCommentProcessor<ContextInfo>(new ContextClassifier());
        var factory = new ContextInfoFactory<ContextInfo>();
        var modelBuilder = new RoslynCodeSemanticTreeModelBuilder(semanticModelStorage);
        RoslynPhaseParserContextBuilder<ContextInfo> parser1 = new(contextCollector, factory, processor, modelBuilder, onWriteLog);

        parser1.ParseFiles(files, RoslynCodeParserOptions.Default(assembliesPaths), RoslynContextParserOptions.ContextsOptions);

        var allContexts = contextCollector.Collection;

        // 2. Второй проход: строим ссылки на основе уже собранных
        var referenceCollector = new ContextInfoReferenceCollector<ContextInfo>(allContexts);
        var semanticInvocationResolver = new RoslynCodeSemanticInvocationResolver(semanticModelStorage);

        var parser2 = new RoslynPhaseParserReferenceResolver<ContextInfo>(referenceCollector, modelBuilder, semanticInvocationResolver, onWriteLog);

        foreach(var file in files)
        {
            parser2.ParseFile(file, RoslynCodeParserOptions.Default(assembliesPaths), RoslynContextParserOptions.ReferencesOptions);
        }

        return allContexts;
    }

    protected static List<ContextInfo> ParseFile(string filePath, OnWriteLog? onWriteLog = null)
    {
        var collector = new ContextInfoCollector<ContextInfo>();
        //var processor = new ContextInfoCommentProcessor<ContextInfo>(new ContextClassifier());
        //var factory = new ContextInfoFactory<ContextInfo>();
        //var semanticTreeStorage = new RoslynCodeTreeModelStorage();
        //var semanticModelBuilder = new RoslynCodeSemanticTreeModelBuilder(semanticTreeStorage);
        //var parser = new RoslynCodeParser<ContextInfo>();
        //parser.ParseFile(filePath, RoslynCodeParserOptions.Default(), RoslynContextParserOptions.ContextsOptions);

        return collector.Collection;
    }
}

//context: csharp, model
public record struct RoslynContextParserOptions
{
    public RoslynContextParserOptions()
    {
    }

    public bool ParseContexts { get; set; } = true;

    public bool ParseReferences { get; set; } = true;

    public static RoslynContextParserOptions ContextsOptions = new() { ParseContexts = true, ParseReferences = false };
    public static RoslynContextParserOptions ReferencesOptions = new() { ParseContexts = false, ParseReferences = true };
}
