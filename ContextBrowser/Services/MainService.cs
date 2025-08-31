using CommandlineKit;
using CommandlineKit.Model;
using ContextBrowser.ContextCommentsParser;
using ContextBrowser.FileManager;
using ContextBrowser.Html.Composite;
using ContextBrowser.Html.Pages.Index;
using ContextBrowser.Infrastructure;
using ContextBrowser.Model;
using ContextBrowser.Roslyn;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ContextKit.Model.Factory;
using ContextKit.Stategies;
using ExporterKit.HtmlPageSamples;
using ExporterKit.Puml;
using ExporterKit.Uml;
using HtmlKit.Model;
using LoggerKit;
using LoggerKit.Model;
using Microsoft.Extensions.Hosting;
using RoslynKit.Assembly;
using RoslynKit.Phases;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Phases.Invocations;
using RoslynKit.Phases.Syntax.Parsers;
using RoslynKit.Tree;
using RoslynKit.Wrappers.Extractor;
using SemanticKit.Model;

namespace ContextBrowser.Services;

//context: app, execute
public interface IMainService
{
    //context: app, execute
    Task RunAsync(CancellationToken cancellationToken);
}


//context app, model
public class MainService : IMainService
{
    private readonly IAppLogger<AppLevel> _appLogger;
    private readonly IAppOptionsStore _optionsStore;
    private readonly ICodeParseService _codeParseService;
    private readonly IContextInfoCacheService _contextInfoCacheService;
    private readonly IParsingOrchestrator _parsingOrchestrant;

    public MainService(
        IAppLogger<AppLevel> appLogger,
        ICodeParseService codeParseService,
        IContextInfoCacheService contextInfoCacheService,
        IAppOptionsStore optionsStore,
        IParsingOrchestrator parsingOrchestrant)
    {
        _appLogger = appLogger;
        _optionsStore = optionsStore;
        _codeParseService = codeParseService;
        _contextInfoCacheService = contextInfoCacheService;
        _parsingOrchestrant = parsingOrchestrant;
    }

    // context: app, execute
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var appOptions = _optionsStore.Options;
        ExportPathDirectoryPreparer.Prepare(appOptions.Export.Paths);

        var contextsList = await _parsingOrchestrant.GetParsedContextsAsync(appOptions, cancellationToken);

        var contextInfoDataset = ContextInfoDatasetBuilder.Build(contextsList, appOptions.Export.ExportMatrix, appOptions.Classifier, _appLogger);

        PumlExtraDiagramsBuilder.Build(contextInfoDataset, appOptions, appOptions.Classifier, _appLogger);

        PumlComponentDiagramBuilder.Build(contextInfoDataset, appOptions, _appLogger);

        HtmlIndexBuilder.Build(contextInfoDataset, appOptions, appOptions.Classifier, _appLogger);

        PumlActionPerDomainDiagramBuilder.Build(contextInfoDataset, appOptions, _appLogger);

        //
        var actionStateDiagramCompiler = new UmlStateActionDiagramCompiler(contextInfoDataset.ContextInfoData, appOptions.Classifier, appOptions.Export, appOptions.DiagramBuilder, _appLogger);
        _ = actionStateDiagramCompiler.Compile(contextInfoDataset.ContextsList);

        var actionSequenceDiagramCompiler = new UmlSequenceActionDiagramCompiler(contextInfoDataset.ContextInfoData, appOptions.Classifier, appOptions.Export, appOptions.DiagramBuilder, _appLogger);
        _ = actionSequenceDiagramCompiler.Compile(contextInfoDataset.ContextsList);

        var domainSequenceDiagramCompiler = new UmlSequenceDomainDiagramCompiler(contextInfoDataset.ContextInfoData, appOptions.Classifier, appOptions.Export, appOptions.DiagramBuilder, _appLogger);
        _ = domainSequenceDiagramCompiler.Compile(contextInfoDataset.ContextsList);

        // action per domain
        ActionPerDomainHtmlPageBuilder.Build(contextInfoDataset, appOptions, _appLogger);

        // action only
        ActionOnlyHtmlPageBuilder.Build(contextInfoDataset, appOptions, _appLogger);

        // domain only
        DomainOnlyHtmlPageBuilder.Build(contextInfoDataset, appOptions, _appLogger);


        PumlExtraDiagramsBuilder.Build(contextInfoDataset, appOptions, appOptions.Classifier, _appLogger);

        CustomEnvironment.CopyResources(appOptions.Export.Paths.OutputDirectory);
        CustomEnvironment.RunServers(appOptions.Export.Paths.OutputDirectory);
    }

}

public class CommentParsingStrategyFactory<TContext> : ICommentParsingStrategyFactory<TContext>
    where TContext : ContextInfo
{
    private readonly IAppLogger<AppLevel> _appLogger;

    public CommentParsingStrategyFactory(IAppLogger<AppLevel> appLogger)
    {
        _appLogger = appLogger;
    }

    public IEnumerable<ICommentParsingStrategy<TContext>> CreateStrategies(IContextClassifier classifier)
    {
        return new List<ICommentParsingStrategy<TContext>>()
        {
            new CoverageStrategy<TContext>(),
            new ContextValidationDecorator<TContext>(
                classifier,
                new ContextStrategy<TContext>(classifier),
                _appLogger.WriteLog),
        };
    }
}
