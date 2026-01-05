using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model.Options;

namespace SemanticKit.Model;

// context: semantic, build, contextInfo
public class SemanticFileParser<TContext, TSyntaxTreeWrapper> : ISemanticFileParser<TContext>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    protected readonly ISemanticTreeModelBuilder<TSyntaxTreeWrapper, ISemanticModelWrapper> _semanticModelBuilder;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextCollector<TContext> _collector;
    private readonly ISemanticDeclarationParser<TContext, TSyntaxTreeWrapper> _declarationParser;

    private readonly IAppOptionsStore _optionsStore;

    private readonly ISemanticSyntaxRouterBuilderRegistry<TContext> _semanticSyntaxRouterBuilderRegistry;

    public SemanticFileParser(
        ISemanticTreeModelBuilder<TSyntaxTreeWrapper, ISemanticModelWrapper> modelBuilder,
        IAppLogger<AppLevel> logger,
        IContextCollector<TContext> collector,
        ISemanticSyntaxRouterBuilderRegistry<TContext> semanticSyntaxRouterBuilderRegistry,
        IAppOptionsStore optionsStore,
        ISemanticDeclarationParser<TContext, TSyntaxTreeWrapper> declarationParser)
    {
        _semanticModelBuilder = modelBuilder;
        _logger = logger;
        _collector = collector;
        _semanticSyntaxRouterBuilderRegistry = semanticSyntaxRouterBuilderRegistry;
        _optionsStore = optionsStore;
        _declarationParser = declarationParser;
    }

    // context: semantic, build, compilationFlow
    public async Task<IEnumerable<TContext>> ParseFilesAsync(IEnumerable<string> codeFiles, SemanticOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // выбираем язык
        var semanticLanguage = _optionsStore.GetOptions<CodeParsingOptions>().SemanticLanguage;
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Selecting language - {semanticLanguage}");

        // создаём весь инструментарий парсинга выбранного языка
        var semanticRouterBuilder = _semanticSyntaxRouterBuilderRegistry.GetRouterBuilder(semanticLanguage);
        var semanticRouter = semanticRouterBuilder.CreateRouter();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, "Compilation preparations", LogLevelNode.Start);

        //собираем все компиляции
        var compilationMap = await _semanticModelBuilder.BuildCompilationMapAsync(codeFiles, options, cancellationToken).ConfigureAwait(false);

        var tasks = compilationMap.Select(mapItem => _declarationParser.Parse(semanticRouter, options, mapItem, cancellationToken));
        await Task.WhenAll(tasks);

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Compilation preparations done", LogLevelNode.End);

        return _collector.GetAll();
    }

#warning tobe checked
    public void RenewContextInfoList(IEnumerable<TContext> contextInfoList)
    {
    }
}
