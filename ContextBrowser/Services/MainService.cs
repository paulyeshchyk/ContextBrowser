using System.Threading;
using System.Threading.Tasks;
using CommandlineKit;
using CommandlineKit.Model;
using ContextBrowser.ContextCommentsParser;
using ContextBrowser.FileManager;
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
using HtmlKit.Page.Compiler;
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
    private readonly IParsingOrchestrator _parsingOrchestrant;
    private readonly IContextInfoDatasetBuilder _contextInfoDatasetBuilder;
    private readonly IDiagramCompilerOrchestrator _diagramCompilerOrchestrator;
    private readonly IHtmlCompilerOrchestrator _htmlCompilerOrchestrator;
    private readonly IServerStartSignal _serverStartSignal;


    public MainService(
        IAppLogger<AppLevel> appLogger,
        IAppOptionsStore optionsStore,
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoDatasetBuilder contextInfoDatasetBuilder,
        IDiagramCompilerOrchestrator diagramCompilerOrchestrator,
        IHtmlCompilerOrchestrator htmlCompilerOrchestrator,
        IServerStartSignal serverStartSignal)
    {
        _appLogger = appLogger;
        _optionsStore = optionsStore;
        _parsingOrchestrant = parsingOrchestrant;
        _contextInfoDatasetBuilder = contextInfoDatasetBuilder;
        _diagramCompilerOrchestrator = diagramCompilerOrchestrator;
        _htmlCompilerOrchestrator = htmlCompilerOrchestrator;
        _serverStartSignal = serverStartSignal;
    }

    // context: app, execute
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var appOptions = _optionsStore.Options();
        ExportPathDirectoryPreparer.Prepare(appOptions.Export.Paths, _appLogger.WriteLog);

        // парсинг кода
        var contextsList = await _parsingOrchestrant.GetParsedContextsAsync(appOptions, cancellationToken);

        //сборка контекстной матрицы
        var contextInfoDataset = _contextInfoDatasetBuilder.Build(contextsList, appOptions.Export.ExportMatrix, appOptions.Classifier);

        //компиляция диаграмм
        _diagramCompilerOrchestrator.CompileAll(contextInfoDataset, appOptions);

        // компиляция html
        _htmlCompilerOrchestrator.CompileAll(contextInfoDataset, appOptions.Export);

        // запуск кастомных html & puml серверов
        _serverStartSignal.Signal();
    }
}