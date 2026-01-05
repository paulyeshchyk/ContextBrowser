using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Html.Pages;
using LoggerKit;
using TensorKit.Model;
using UmlKit.Compiler;

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
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IUmlDiagramCompilerOrchestrator _diagramCompilerOrchestrator;
    private readonly IHtmlCompilerOrchestrator _htmlCompilerOrchestrator;
    private readonly IServerStartSignal _serverStartSignal;

    public MainService(
        IAppLogger<AppLevel> appLogger,
        IAppOptionsStore optionsStore,
        IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider,
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

    // context: app, execute, compilationFlow
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();

        exportOptions.FilePaths.Prepare();

        //необязательный шаг: принудительная загрузка контента
        await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        //компиляция диаграмм
        await _diagramCompilerOrchestrator.CompileAllAsync(cancellationToken).ConfigureAwait(false);

        // компиляция html
        await _htmlCompilerOrchestrator.CompileAllAsync(cancellationToken).ConfigureAwait(false);

        // запуск кастомных html & puml серверов
        _serverStartSignal.Signal();
    }
}