using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Uml;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoDatasetProvider : BaseContextInfoProvider, IContextInfoDatasetProvider
{
    private readonly IContextInfoDatasetBuilder _datasetBuilder;

    private IAppOptionsStore _optionsStore;

    // Приватное поле для кэширования результата.
    private IContextInfoDataset<ContextInfo>? _dataset;

    // Объект для синхронизации доступа.
    private readonly object _lock = new object();

    public ContextInfoDatasetProvider(
        IAppOptionsStore optionsStore,
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoDatasetBuilder datasetBuilder)
        : base(parsingOrchestrant)
    {
        _optionsStore = optionsStore;
        _datasetBuilder = datasetBuilder;
    }

    // Метод для асинхронного получения уже сформированного датасета.
    public async Task<IContextInfoDataset<ContextInfo>> GetDatasetAsync(CancellationToken cancellationToken)
    {
        if (_dataset == null)
        {
            await BuildDatasetAsync(cancellationToken);
        }
        return _dataset!;
    }

    // Приватный метод для однократной сборки датасета.
    private async Task BuildDatasetAsync(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_dataset != null)
            {
                return;
            }
        }

        var contextsList = await GetParsedContextsAsync(cancellationToken);
        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var classifier = _optionsStore.GetOptions<IDomainPerActionContextClassifier>();
        var newDataset = _datasetBuilder.Build(contextsList, exportOptions.ExportMatrix, classifier);

        lock (_lock)
        {
            _dataset = newDataset;
        }
    }
}
