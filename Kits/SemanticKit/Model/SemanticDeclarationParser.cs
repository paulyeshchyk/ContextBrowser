using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Collector;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: semantic, build, contextInfo
public class SemanticDeclarationParser<TContext> : ISemanticDeclarationParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly ISemanticTreeModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper> _semanticModelBuilder;
    private readonly OnWriteLog? _onWriteLog;
    private readonly ISemanticSyntaxRouter<TContext> _router;
    private readonly SemanticOptions _options;
    private readonly ContextInfoCollector<TContext> _collector;

    public SemanticDeclarationParser(
        ISemanticTreeModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper> modelBuilder,
        OnWriteLog? onWriteLog,
        SemanticOptions options,
        ISemanticSyntaxRouter<TContext> router,
        ContextInfoCollector<TContext> collector)
    {
        _semanticModelBuilder = modelBuilder;
        _onWriteLog = onWriteLog;
        _options = options;
        _router = router;
        _collector = collector;
    }

    // context: semantic, build
    public IEnumerable<TContext> ParseFiles(IEnumerable<string> codeFiles, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, "Parsing files: phase 1", LogLevelNode.Start);

        var compilationMap = _semanticModelBuilder.BuildCompilationMap(codeFiles, _options, cancellationToken);
        foreach (var mapItem in compilationMap)
        {
            ParseDeclarations(_options, mapItem, cancellationToken);
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return _collector.GetAll();
    }

#warning tobe checked
    public void RenewContextInfoList(IEnumerable<TContext> contextInfoList)
    {
        //пока ничего не делаем здесь
    }

    private void ParseDeclarations(SemanticOptions options, CompilationMap mapItem, CancellationToken cancellationToken)
    {
        var tree = mapItem.SyntaxTree;
        var model = mapItem.SemanticModel;

        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Parsing files: phase 1 - {tree.FilePath}", LogLevelNode.Start);

        var availableSyntaxies = tree.GetAvailableSyntaxies(options, cancellationToken);

        if (availableSyntaxies.Any())
        {
            _router.Route(availableSyntaxies, model, cancellationToken);
        }
        else
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Model has no members: {tree.FilePath}");
        }

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
