using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Import;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ContextKit.Model.Factory;
using RoslynKit.ContextData;
using RoslynKit.ContextData.Comment;
using RoslynKit.ContextData.Comment.Stategies;
using RoslynKit.Route.Assembly;
using RoslynKit.Route.Phases;
using RoslynKit.Route.Phases.ContextInfoBuilder;
using RoslynKit.Route.Phases.Invocations;
using RoslynKit.Route.Phases.Syntax;
using RoslynKit.Route.Phases.Syntax.Parsers;
using RoslynKit.Route.Tree;
using RoslynKit.Route.Wrappers.Extractor;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Route;

// context: roslyn, directory, contextInfo, build
public sealed class RoslynContextParser
{
    private readonly CodeParsingOptions _options;
    private readonly IContextClassifier _contextClassifier;
    private readonly OnWriteLog? _onWriteLog;
    private readonly ISyntaxTreeWrapperBuilder _treeWrapBuilder;

    public RoslynContextParser(ISyntaxTreeWrapperBuilder treeWrapBuilder, CodeParsingOptions options, IContextClassifier contextClassifier, OnWriteLog? onWriteLog)
    {
        _options = options;
        _contextClassifier = contextClassifier;
        _treeWrapBuilder = treeWrapBuilder;
        _onWriteLog = onWriteLog;
    }

    // context: roslyn, read, directory, contextInfo
    // layer: 900
    public Task<List<ContextInfo>> ParseAsync(string[] filePaths, CancellationToken cancellationToken)
    {
        var semanticModelStorage = new CSharpSemanticTreeModelStorage(0, _onWriteLog);

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, "Parsing files", LogLevelNode.Start);

        var contextCollector = new ContextInfoCollector<ContextInfo>();
        var compilationBuilder = new CSharpCompilationBuilder(_options.Semantic, _onWriteLog);
        var modelBuilder = new BaseTreeModelBuilder(compilationBuilder, semanticModelStorage, _treeWrapBuilder, _onWriteLog);

        Phase1(_options.Semantic, _contextClassifier, _onWriteLog, filePaths, semanticModelStorage, modelBuilder, contextCollector, cancellationToken);

        Phase2(_treeWrapBuilder, _options.Semantic, _onWriteLog, filePaths, semanticModelStorage, modelBuilder, contextCollector, cancellationToken);

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, string.Empty, LogLevelNode.End);

        var result = contextCollector.Collection.ToList();
        return Task.FromResult(result);
    }

    // context: build, directory, roslyn, contextInfo
    internal static void Phase2(ISyntaxTreeWrapperBuilder _treeWrapBuilder, SemanticOptions options, OnWriteLog? onWriteLog, string[] files, CSharpSemanticTreeModelStorage semanticModelStorage, BaseTreeModelBuilder modelBuilder, ContextInfoCollector<ContextInfo> contextCollector, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, "Parsing files: phase 2", LogLevelNode.Start);

        var factory = new ContextInfoFactory<ContextInfo>();

        // 2. Второй проход: строим ссылки на основе уже собранных
        var referenceCollector = new ContextInfoReferenceCollector<ContextInfo>(contextCollector.Collection);
        var semanticInvocationResolver = new CSharpInvocationSemanticResolver(semanticModelStorage);
        var invocationSyntaxExtractor = new RoslynInvocationSyntaxExtractor(semanticInvocationResolver, options, onWriteLog);
        var invocationReferenceBuilder = new CSharpInvocationReferenceBuilder<ContextInfo>(onWriteLog, factory, invocationSyntaxExtractor, options, referenceCollector);
        var referenceParser = new CSharpInvocationParser<ContextInfo>(collector: referenceCollector, factory: factory, modelBuilder: modelBuilder, invocationReferenceBuilder: invocationReferenceBuilder, treeWrapperBuilder: _treeWrapBuilder, options, onWriteLog);

        referenceParser.ParseFiles(files, cancellationToken);

        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, string.Empty, LogLevelNode.End);
    }

    // context: build, directory, roslyn, contextInfo
    internal static void Phase1(SemanticOptions options, IContextClassifier contextClassifier, OnWriteLog? onWriteLog, string[] files, CSharpSemanticTreeModelStorage semanticModelStorage, ISemanticModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper> modelBuilder, ContextInfoCollector<ContextInfo> contextCollector, CancellationToken cancellationToken)
    {
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, "Parsing files: phase 1", LogLevelNode.Start);

        List<ICommentParsingStrategy<ContextInfo>> strategies = new() {
            new CoverageStrategy<ContextInfo>(),
            new ContextValidationDecorator<ContextInfo>(contextClassifier,new ContextStrategy<ContextInfo>(contextClassifier), onWriteLog),
        };

        // 1. Первый проход: собираем все context'ы (типы и методы), без ссылок
        var processor = new ContextInfoCommentProcessor<ContextInfo>(strategies);
        var factory = new ContextInfoFactory<ContextInfo>();

        var dependenciesFactory = new CSharpPhaseParserDependenciesFactory<ContextInfo>(contextCollector, factory, processor, options, onWriteLog);

        var router = dependenciesFactory.CreateRouter();

        var phaseParser = new CSharpPhaseParserContextBuilder<ContextInfo>(
            modelBuilder: modelBuilder,
            onWriteLog,
            options,
            router);

        phaseParser.ParseFiles(files, cancellationToken);
        onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, string.Empty, LogLevelNode.End);
    }
}
