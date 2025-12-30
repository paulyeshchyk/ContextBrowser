using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowser.Services.Parsing;
using ContextKit.Model;
using ExporterKit.Html.Containers;
using TensorKit.Model;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoDatasetProviderMethodList<TDataTensor> : BaseContextInfoProvider, IContextInfoDatasetProvider<MethodListTensor<TDataTensor>>
    where TDataTensor : IDomainPerActionTensor
{
    private readonly IContextInfoDatasetBuilder<MethodListTensor<TDataTensor>> _datasetBuilder;

    private IContextInfoDataset<ContextInfo, MethodListTensor<TDataTensor>>? _dataset;

    private readonly object _lock = new object();

    public ContextInfoDatasetProviderMethodList(
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoDatasetBuilder<MethodListTensor<TDataTensor>> datasetBuilder)
        : base(parsingOrchestrant)
    {
        _datasetBuilder = datasetBuilder;
    }

    public async Task<IContextInfoDataset<ContextInfo, MethodListTensor<TDataTensor>>> GetDatasetAsync(CancellationToken cancellationToken)
    {
        if (_dataset == null)
        {
            await BuildDatasetAsync(cancellationToken).ConfigureAwait(false);
        }
        return _dataset!;
    }

    internal async Task BuildDatasetAsync(CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_dataset != null)
            {
                return;
            }
        }

        var contextsList = await GetParsedContextsAsync(cancellationToken).ConfigureAwait(false);

        // Здесь мы фильтруем и строим датасет специально для списка методов
        var filteredContexts = contextsList
            .Where(c => c.ElementType == ContextInfoElementType.@method)
            .ToList();

        var newDataset = _datasetBuilder.Build(filteredContexts, cancellationToken);

        lock (_lock)
        {
            _dataset = newDataset;
        }
    }
}