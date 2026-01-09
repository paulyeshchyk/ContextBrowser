using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using LoggerKit;
using SemanticKit.Model;
using SemanticKit.Model.Options;
using SemanticKit.Parsers.File;
using SemanticKit.Parsers.Strategy.Declaration;
using SemanticKit.Parsers.Syntax;

namespace SemanticKit.Parsers.File;

// context: semantic, build, contextInfo
public class DeclarationFileParser<TContext, TSyntaxTreeWrapper> : IDeclarationFileParser<TContext>
    where TContext : IContextWithReferences<TContext>
    where TSyntaxTreeWrapper : ISyntaxTreeWrapper
{
    protected readonly ISemanticCompilationMapBuilder<TSyntaxTreeWrapper, ISemanticModelWrapper> _semanticModelBuilder;
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextCollector<TContext> _collector;
    private readonly IDeclarationParser<TContext, TSyntaxTreeWrapper> _declarationParser;
    private readonly IAppOptionsStore _optionsStore;
    private readonly ISyntaxRouterBuilderRegistry<TContext> _semanticSyntaxRouterBuilderRegistry;

    public DeclarationFileParser(
        ISemanticCompilationMapBuilder<TSyntaxTreeWrapper, ISemanticModelWrapper> modelBuilder,
        IAppLogger<AppLevel> logger,
        IContextCollector<TContext> collector,
        ISyntaxRouterBuilderRegistry<TContext> semanticSyntaxRouterBuilderRegistry,
        IAppOptionsStore optionsStore,
        IDeclarationParser<TContext, TSyntaxTreeWrapper> declarationParser)
    {
        _semanticModelBuilder = modelBuilder;
        _logger = logger;
        _collector = collector;
        _semanticSyntaxRouterBuilderRegistry = semanticSyntaxRouterBuilderRegistry;
        _optionsStore = optionsStore;
        _declarationParser = declarationParser;
    }

    // context: semantic, build, compilationFlow
    public async Task<IEnumerable<TContext>> ParseFilesAsync(IEnumerable<string> codeFiles, string compilationName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // выбираем язык
        var semanticLanguage = _optionsStore.GetOptions<CodeParsingOptions>().SemanticLanguage;
        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, $"Selecting language - {semanticLanguage}");

        // создаём весь инструментарий парсинга выбранного языка
        var semanticRouterBuilder = _semanticSyntaxRouterBuilderRegistry.GetRouterBuilder(semanticLanguage);
        var semanticRouter = semanticRouterBuilder.CreateRouter();

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Cntx, "Phase 1: Parsing declarations", LogLevelNode.Start);

        //собираем все компиляции
        var compilationMap = await _semanticModelBuilder.BuildCompilationMapAsync(codeFiles, compilationName, cancellationToken).ConfigureAwait(false);

        var tasks = compilationMap.Select(async mapItem => await _declarationParser.ParseAsync(semanticRouter, mapItem, cancellationToken).ConfigureAwait(false));
        await Task.WhenAll(tasks);

        _logger.WriteLog(AppLevel.R_Syntax, LogLevel.Dbg, "Phase 1: Parsing declarations done", LogLevelNode.End);

        return _collector.GetAll();
    }

#warning tobe checked
    public void RenewContextInfoList(IEnumerable<TContext> contextInfoList)
    {
    }
}
