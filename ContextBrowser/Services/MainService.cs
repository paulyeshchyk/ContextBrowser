using System.Threading;
using System.Threading.Tasks;
using CommandlineKit;
using CommandlineKit.Model;
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
using ExporterKit.Infrastucture;
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
using UmlKit.Infrastructure.Options;

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
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IUmlDiagramCompilerOrchestrator _diagramCompilerOrchestrator;
    private readonly IHtmlCompilerOrchestrator _htmlCompilerOrchestrator;
    private readonly IServerStartSignal _serverStartSignal;

    public MainService(
        IAppLogger<AppLevel> appLogger,
        IAppOptionsStore optionsStore,
        IContextInfoDatasetProvider datasetProvider,
        IUmlDiagramCompilerOrchestrator diagramCompilerOrchestrator,
        IHtmlCompilerOrchestrator htmlCompilerOrchestrator,
        IServerStartSignal serverStartSignal)
    {
        _appLogger = appLogger;
        _optionsStore = optionsStore;
        _datasetProvider = datasetProvider;
        _diagramCompilerOrchestrator = diagramCompilerOrchestrator;
        _htmlCompilerOrchestrator = htmlCompilerOrchestrator;
        _serverStartSignal = serverStartSignal;
    }

    // context: app, execute
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var classifier = _optionsStore.GetOptions<IDomainPerActionContextClassifier>();
        var diagramBuilderOptions = _optionsStore.GetOptions<DiagramBuilderOptions>();

        ExportPathDirectoryPreparer.Prepare(exportOptions.FilePaths);

        await _datasetProvider.GetDatasetAsync(cancellationToken);

        //компиляция диаграмм
        await _diagramCompilerOrchestrator.CompileAllAsync(classifier, exportOptions, diagramBuilderOptions, cancellationToken);

        // компиляция html
        await _htmlCompilerOrchestrator.CompileAllAsync(classifier, exportOptions, cancellationToken);

        // запуск кастомных html & puml серверов
        _serverStartSignal.Signal();
    }
}