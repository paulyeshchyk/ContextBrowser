using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: semantic, build, contextInfo
public class SemanticDeclarationParser<TContext> : ISemanticDeclarationParser<TContext>
    where TContext : IContextWithReferences<TContext>
{
    protected readonly ISemanticTreeModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper> _semanticModelBuilder;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly ISemanticSyntaxRouter<TContext> _router;
    private readonly IContextCollector<TContext> _collector;

    public SemanticDeclarationParser(
        ISemanticTreeModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper> modelBuilder,
        IAppLogger<AppLevel> logger,
        ISemanticSyntaxRouter<TContext> router,
        IContextCollector<TContext> collector)
    {
        _semanticModelBuilder = modelBuilder;
        _logger = logger;
        _router = router;
        _collector = collector;
    }

    // context: semantic, build
    public IEnumerable<TContext> ParseFiles(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Parsing files: phase 1", LogLevelNode.Start);

        var compilationMap = _semanticModelBuilder.BuildCompilationMap(codeFiles, options, cancellationToken);
        foreach (var mapItem in compilationMap)
        {
            ParseDeclarations(options, mapItem, cancellationToken);
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return _collector.GetAll();
    }

#warning tobe checked
    public void RenewContextInfoList(IEnumerable<TContext> contextInfoList)
    {
    }

    private void ParseDeclarations(SemanticOptions options, CompilationMap mapItem, CancellationToken cancellationToken)
    {
        var tree = mapItem.SyntaxTree;
        var model = mapItem.SemanticModel;

        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Parsing files: phase 1 - {tree.FilePath}", LogLevelNode.Start);

        var availableSyntaxies = tree.GetAvailableSyntaxies(options, cancellationToken);

        if (availableSyntaxies.Any())
        {
            _router.Route(availableSyntaxies, model, options, cancellationToken);
        }
        else
        {
            _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Model has no members: {tree.FilePath}");
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, string.Empty, LogLevelNode.End);
    }
}
