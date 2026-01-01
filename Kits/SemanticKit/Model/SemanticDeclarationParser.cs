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
    private readonly IContextCollector<TContext> _collector;

    private readonly IAppOptionsStore _optionsStore;

    private readonly ISemanticSyntaxRouterBuilderRegistry<TContext> _semanticSyntaxRouterBuilderRegistry;

    public SemanticDeclarationParser(
        ISemanticTreeModelBuilder<ISyntaxTreeWrapper, ISemanticModelWrapper> modelBuilder,
        IAppLogger<AppLevel> logger,
        IContextCollector<TContext> collector,
        ISemanticSyntaxRouterBuilderRegistry<TContext> semanticSyntaxRouterBuilderRegistry,
        IAppOptionsStore optionsStore)
    {
        _semanticModelBuilder = modelBuilder;
        _logger = logger;
        _collector = collector;
        _semanticSyntaxRouterBuilderRegistry = semanticSyntaxRouterBuilderRegistry;
        _optionsStore = optionsStore;

    }

    // context: semantic, build
    public IEnumerable<TContext> ParseFiles(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // выбираем язык
        var semanticLanguage = _optionsStore.GetOptions<CodeParsingOptions>().SemanticLanguage;
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Selecting language - {semanticLanguage}");

        // создаём весь инструментарий парсинга выбранного языка
        var semanticRouterBuilder = _semanticSyntaxRouterBuilderRegistry.GetRouterBuilder(semanticLanguage);
        var semanticRouter = semanticRouterBuilder.CreateRouter();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Phase 1: Parsing files", LogLevelNode.Start);

        var compilationMap = _semanticModelBuilder.BuildCompilationMap(codeFiles, options, cancellationToken);
        foreach (var mapItem in compilationMap)
        {
            ParseDeclarations(semanticRouter, options, mapItem, cancellationToken);
        }

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, string.Empty, LogLevelNode.End);

        return _collector.GetAll();
    }

#warning tobe checked
    public void RenewContextInfoList(IEnumerable<TContext> contextInfoList)
    {
    }

    internal void ParseDeclarations(ISemanticSyntaxRouter<TContext> _router, SemanticOptions options, CompilationMap mapItem, CancellationToken cancellationToken)
    {
        var tree = mapItem.SyntaxTree;
        var model = mapItem.SemanticModel;

        cancellationToken.ThrowIfCancellationRequested();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, $"Phase 1: Parsing declarations - {tree.FilePath}", LogLevelNode.Start);

        var availableSyntaxies = tree.GetAvailableSyntaxies(options, cancellationToken).ToList();

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
