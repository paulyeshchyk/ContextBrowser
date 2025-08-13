using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynKit.Model;
using RoslynKit.Model.ModelBuilder;
using RoslynKit.Syntax;

namespace RoslynKit.Phases;

// context: csharp, build, contextInfo
public class RoslynPhaseParserContextBuilder<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly ISemanticModelBuilder<SyntaxTree, SemanticModel> _semanticModelBuilder;
    protected readonly IContextCollector<TContext> _collector;
    private readonly OnWriteLog? _onWriteLog;
    private readonly SyntaxRouter<TContext> _router;
    private readonly RoslynCodeParserOptions _options;

    public RoslynPhaseParserContextBuilder(
        IContextCollector<TContext> collector,
        ISemanticModelBuilder<SyntaxTree, SemanticModel> modelBuilder,
        OnWriteLog? onWriteLog,
        RoslynCodeParserOptions options,
        SyntaxRouter<TContext> router)
    {
        _semanticModelBuilder = modelBuilder;
        _collector = collector;
        _onWriteLog = onWriteLog;
        _options = options;
        _router = router;
    }

    // context: csharp, build
    public void ParseFiles(IEnumerable<string> codeFiles, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, "Parsing files: phase 1", LogLevelNode.Start);

        var models = _semanticModelBuilder.BuildModels(codeFiles, _options, cancellationToken);
        foreach (var (tree, model) in models)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ParseDeclarations(_options, tree, model, cancellationToken);
        }
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, string.Empty, LogLevelNode.End);
    }

    private void ParseDeclarations(RoslynCodeParserOptions options, SyntaxTree tree, SemanticModel model, CancellationToken cancellationToken)
    {
        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, $"Parsing files: phase 1 - {tree.FilePath}", LogLevelNode.Start);

        var root = tree.GetCompilationUnitRoot(cancellationToken);
        var availableSyntaxies = root.GetMemberDeclarationSyntaxies(options);

        if(availableSyntaxies.Any())
        {
            _router.Route(availableSyntaxies, model);
        }
        else
        {
            _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Dbg, $"Model has no members: {tree.FilePath}");
        }


        _onWriteLog?.Invoke(AppLevel.Roslyn, LogLevel.Cntx, string.Empty, LogLevelNode.End);
    }
}
