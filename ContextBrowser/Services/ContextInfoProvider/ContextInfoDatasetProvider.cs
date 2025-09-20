using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoDatasetProvider<TTensor> : BaseContextInfoProvider, IContextInfoDatasetProvider<TTensor>
    where TTensor : notnull
{
    private readonly IContextInfoDatasetBuilder<TTensor> _datasetBuilder;

    private IContextInfoDataset<ContextInfo, TTensor>? _dataset;

    private readonly object _lock = new object();

    public ContextInfoDatasetProvider(
        IAppOptionsStore optionsStore,
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoDatasetBuilder<TTensor> datasetBuilder)
        : base(parsingOrchestrant)
    {
        _datasetBuilder = datasetBuilder;
    }

    public async Task<IContextInfoDataset<ContextInfo, TTensor>> GetDatasetAsync(CancellationToken cancellationToken)
    {
        if (_dataset == null)
        {
            await BuildDatasetAsync(cancellationToken);
        }
        return _dataset!;
    }

    // Приватный метод для однократной сборки датасета.
    internal async Task BuildDatasetAsync(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_dataset != null)
            {
                return;
            }
        }

        var contextsList = await GetParsedContextsAsync(cancellationToken);
        var newDataset = _datasetBuilder.Build(contextsList);

        lock (_lock)
        {
            _dataset = newDataset;
        }
    }
}
