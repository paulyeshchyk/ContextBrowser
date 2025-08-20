using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using SemanticKit.Model;
using SemanticKit.Model.Options;

namespace RoslynKit.Route.Phases.Syntax;

// context: roslyn, build, contextInfo
public class CSharpPhaseParserContextBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly ISemanticModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper> _semanticModelBuilder;
    private readonly OnWriteLog? _onWriteLog;
    private readonly SyntaxRouter<TContext> _router;
    private readonly SemanticOptions _options;

    public CSharpPhaseParserContextBuilder(
        ISemanticModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper> modelBuilder,
        OnWriteLog? onWriteLog,
        SemanticOptions options,
        SyntaxRouter<TContext> router)
    {
        _semanticModelBuilder = modelBuilder;
        _onWriteLog = onWriteLog;
        _options = options;
        _router = router;
    }

    // context: roslyn, build
    public void ParseFiles(IEnumerable<string> codeFiles, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, "Parsing files: phase 1", LogLevelNode.Start);

        var models = _semanticModelBuilder.BuildModels(codeFiles, _options, cancellationToken);
        foreach (var (tree, model) in models)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ParseDeclarations(_options, tree, model, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }

    private void ParseDeclarations(SemanticOptions options, ISyntaxTreeWrapper tree, ISemanticModelWrapper model, CancellationToken cancellationToken)
    {
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
