using ContextBrowser.ContextKit.Parser;
using ContextBrowser.ContextKit.Parser.CommentParser;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Extensions;
using RoslynKit.Model;
using RoslynKit.Parser.Phases;
using RoslynKit.Parser.Semantic;

namespace RoslynKit.Parser.Content;

// context: csharp, parser, directory, contextInfo, build
public sealed class RoslynContextParser
{
    private const string TargetExtension = ".cs";

    // context: csharp, read, directory, contextInfo
    // layer: 900
    public static List<ContextInfo> Parse(string rootPath, RoslynCodeParserOptions options, OnWriteLog? onWriteLog, CancellationToken cancellationToken)
    {
        var pathType = PathAnalyzer.GetPathType(rootPath);
        return pathType switch
        {
            PathAnalyzer.PathType.File => ParseFile(rootPath, options, onWriteLog, cancellationToken),
            PathAnalyzer.PathType.Directory => ParseDirectory(rootPath, options, onWriteLog, cancellationToken),
            PathAnalyzer.PathType.NonExistentPath => throw new ArgumentException($"File not found {nameof(rootPath)}"),
            _ => throw new NotImplementedException()
        };
    }

    // context: read, directory, csharp, contextInfo
    public static List<ContextInfo> ParseDirectory(string rootPath, RoslynCodeParserOptions options, OnWriteLog? onWriteLog, CancellationToken cancellationToken)
    {
        var files = Directory.GetFiles(rootPath, $"*{TargetExtension}", SearchOption.AllDirectories);

        return ParseFilesList(options, onWriteLog, files, cancellationToken);
    }

    // context: read, directory, csharp, contextInfo
    internal static List<ContextInfo> ParseFilesList(RoslynCodeParserOptions options, OnWriteLog? onWriteLog, string[] files, CancellationToken cancellationToken)
    {
        var semanticModelStorage = new RoslynCodeSemanticTreeModelStorage();

        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, "Phase", LogLevelNode.Start);

        var contextCollector = new ContextInfoCollector<ContextInfo>();
        var modelBuilder = new RoslynCodeSemanticTreeModelBuilder(semanticModelStorage);

        Phase1(options, onWriteLog, files, semanticModelStorage, modelBuilder, contextCollector, cancellationToken);

        Phase2(options, onWriteLog, files, semanticModelStorage, modelBuilder, contextCollector, cancellationToken);

        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return contextCollector.Collection;
    }

    // context: build, directory, csharp, contextInfo
    internal static void Phase2(RoslynCodeParserOptions options, OnWriteLog? onWriteLog, string[] files, RoslynCodeSemanticTreeModelStorage semanticModelStorage, RoslynCodeSemanticTreeModelBuilder modelBuilder, ContextInfoCollector<ContextInfo> contextCollector, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, "Phase 2", LogLevelNode.Start);

        // 2. Второй проход: строим ссылки на основе уже собранных
        var referenceCollector = new ContextInfoReferenceCollector<ContextInfo>(contextCollector.Collection);
        var semanticInvocationResolver = new RoslynCodeSemanticInvocationResolver(semanticModelStorage);

        var factory = new ContextInfoFactory<ContextInfo>();
        var referenceParser = new RoslynPhaseParserReferenceResolver<ContextInfo>(referenceCollector, factory, modelBuilder, semanticInvocationResolver, onWriteLog);

        foreach(var file in files)
        {
            referenceParser.ParseFile(file, options, cancellationToken);
        }
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    // context: build, directory, csharp, contextInfo
    internal static void Phase1(RoslynCodeParserOptions options, OnWriteLog? onWriteLog, string[] files, RoslynCodeSemanticTreeModelStorage semanticModelStorage, RoslynCodeSemanticTreeModelBuilder modelBuilder, ContextInfoCollector<ContextInfo> contextCollector, CancellationToken cancellationToken)
    {
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, "Phase 1", LogLevelNode.Start);

        List<ICommentParsingStrategy<ContextInfo>> strategies = new() {
            new CoverageStrategy<ContextInfo>(),
            new ContextValidationDecorator<ContextInfo>(new ContextStrategy<ContextInfo>(new ContextClassifier()), onWriteLog),
        };

        // 1. Первый проход: собираем все context'ы (типы и методы), без ссылок
        var processor = new ContextInfoCommentProcessor<ContextInfo>(strategies);
        var factory = new ContextInfoFactory<ContextInfo>();
        var phaseParser = new RoslynPhaseParserContextBuilder<ContextInfo>(contextCollector, factory, processor, modelBuilder, onWriteLog);

        phaseParser.ParseFiles(files, options, cancellationToken);
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    // context: read, directory, csharp, contextInfo
    internal static List<ContextInfo> ParseFile(string filePath, RoslynCodeParserOptions option, OnWriteLog? onWriteLog, CancellationToken cancellationToken)
    {
        return ParseFilesList(option, onWriteLog, new[] { filePath }, cancellationToken);
    }
}
