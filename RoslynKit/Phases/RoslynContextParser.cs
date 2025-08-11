using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ContextKit.Model.Factory;
using RoslynKit.Model;
using RoslynKit.Model.ModelBuilder;
using RoslynKit.Model.Resolver;
using RoslynKit.Model.Storage;
using RoslynKit.Phases.Validation;
using RoslynKit.Syntax.Parser.Comment;
using RoslynKit.Syntax.Parser.Comment.Processors;
using RoslynKit.Syntax.Parser.Comment.Strategies;

namespace RoslynKit.Phases;

// context: csharp, parser, directory, contextInfo, build
public sealed class RoslynContextParser
{
    private const string TargetExtension = ".cs";

    // context: csharp, read, directory, contextInfo
    // layer: 900
    public static List<ContextInfo> Parse(RoslynOptions options, IContextClassifier contextClassifier, OnWriteLog? onWriteLog, CancellationToken cancellationToken)
    {
        var pathType = PathAnalyzer.GetPathType(options.theSourcePath);
        return pathType switch
        {
            PathAnalyzer.PathType.File => ParseFile(options.theSourcePath, options.roslynCodeparserOptions, contextClassifier, onWriteLog, cancellationToken),
            PathAnalyzer.PathType.Directory => ParseDirectory(options.theSourcePath, options.roslynCodeparserOptions, contextClassifier, onWriteLog, cancellationToken),
            PathAnalyzer.PathType.NonExistentPath => throw new ArgumentException($"File not found {nameof(options.theSourcePath)}"),
            _ => throw new NotImplementedException()
        };
    }

    // context: read, directory, csharp, contextInfo
    public static List<ContextInfo> ParseDirectory(string rootPath, RoslynCodeParserOptions options, IContextClassifier contextClassifier, OnWriteLog? onWriteLog, CancellationToken cancellationToken)
    {
        var files = Directory.GetFiles(rootPath, $"*{TargetExtension}", SearchOption.AllDirectories);

        return ParseFilesList(options, contextClassifier, onWriteLog, files, cancellationToken);
    }

    // context: read, directory, csharp, contextInfo
    internal static List<ContextInfo> ParseFilesList(RoslynCodeParserOptions options, IContextClassifier contextClassifier, OnWriteLog? onWriteLog, string[] files, CancellationToken cancellationToken)
    {
        var semanticModelStorage = new RoslynCodeSemanticTreeModelStorage();

        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, "Parsing files", LogLevelNode.Start);

        var contextCollector = new ContextInfoCollector<ContextInfo>();
        var modelBuilder = new RoslynCodeSemanticTreeModelBuilder(semanticModelStorage);

        Phase1(options, contextClassifier, onWriteLog, files, semanticModelStorage, modelBuilder, contextCollector, cancellationToken);

        Phase2(options, onWriteLog, files, semanticModelStorage, modelBuilder, contextCollector, cancellationToken);

        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, string.Empty, LogLevelNode.End);

        return contextCollector.Collection.ToList();
    }

    // context: build, directory, csharp, contextInfo
    internal static void Phase2(RoslynCodeParserOptions options, OnWriteLog? onWriteLog, string[] files, RoslynCodeSemanticTreeModelStorage semanticModelStorage, RoslynCodeSemanticTreeModelBuilder modelBuilder, ContextInfoCollector<ContextInfo> contextCollector, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, "Parsing files: phase 2", LogLevelNode.Start);

        // 2. Второй проход: строим ссылки на основе уже собранных
        var referenceCollector = new ContextInfoReferenceCollector<ContextInfo>(contextCollector.Collection);
        var semanticInvocationResolver = new RoslynCodeSemanticInvocationResolver(semanticModelStorage);

        var factory = new ContextInfoFactory<ContextInfo>();
        var referenceParser = new RoslynPhaseParserReferenceResolver<ContextInfo>(referenceCollector, factory, modelBuilder, semanticInvocationResolver, options, onWriteLog);
        referenceParser.ParseFiles(files, cancellationToken);

        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, string.Empty, LogLevelNode.End);
    }

    // context: build, directory, csharp, contextInfo
    internal static void Phase1(RoslynCodeParserOptions options, IContextClassifier contextClassifier, OnWriteLog? onWriteLog, string[] files, RoslynCodeSemanticTreeModelStorage semanticModelStorage, RoslynCodeSemanticTreeModelBuilder modelBuilder, ContextInfoCollector<ContextInfo> contextCollector, CancellationToken cancellationToken)
    {
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, "Parsing files: phase 1", LogLevelNode.Start);

        List<ICommentParsingStrategy<ContextInfo>> strategies = new() {
            new CoverageStrategy<ContextInfo>(),
            new ContextValidationDecorator<ContextInfo>(contextClassifier,new ContextStrategy<ContextInfo>(contextClassifier), onWriteLog),
        };

        // 1. Первый проход: собираем все context'ы (типы и методы), без ссылок
        var processor = new ContextInfoCommentProcessor<ContextInfo>(strategies);
        var factory = new ContextInfoFactory<ContextInfo>();

        var dependenciesFactory = new RoslynPhaseParserDependenciesFactory<ContextInfo>(contextCollector, factory, processor, options, onWriteLog);

        var router = dependenciesFactory.CreateRouter();

        var phaseParser = new RoslynPhaseParserContextBuilder<ContextInfo>(
            contextCollector,
            modelBuilder,
            onWriteLog,
            options,
            router);

        phaseParser.ParseFiles(files, cancellationToken);
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, string.Empty, LogLevelNode.End);
    }

    // context: read, directory, csharp, contextInfo
    internal static List<ContextInfo> ParseFile(string filePath, RoslynCodeParserOptions option, IContextClassifier contextClassifier, OnWriteLog? onWriteLog, CancellationToken cancellationToken)
    {
        return ParseFilesList(option, contextClassifier, onWriteLog, new[] { filePath }, cancellationToken);
    }
}
