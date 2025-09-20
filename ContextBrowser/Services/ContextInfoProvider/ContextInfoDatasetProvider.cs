using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoDatasetProvider<TKey> : BaseContextInfoProvider, IContextInfoDatasetProvider<TKey>
    where TKey : notnull
{
    private readonly IContextInfoDatasetBuilder<TKey> _datasetBuilder;

    private IContextInfoDataset<ContextInfo, TKey>? _dataset;

    private readonly object _lock = new object();

    public ContextInfoDatasetProvider(
        IAppOptionsStore optionsStore,
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoDatasetBuilder<TKey> datasetBuilder)
        : base(parsingOrchestrant)
    {
        _datasetBuilder = datasetBuilder;
    }

    public async Task<IContextInfoDataset<ContextInfo, TKey>> GetDatasetAsync(CancellationToken cancellationToken)
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
        var newDataset = _datasetBuilder.Build(contextsList);

        lock (_lock)
        {
            _dataset = newDataset;
        }
    }
}
