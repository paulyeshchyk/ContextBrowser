using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using ExporterKit.Html.Containers;
using TensorKit.Model.DomainPerAction;

namespace ContextBrowser.Services.ContextInfoProvider;

public class ContextInfoDatasetProviderMethodList : BaseContextInfoProvider, IContextInfoDatasetProvider<MethodListTensor>
{
    private readonly IContextInfoDatasetBuilder<MethodListTensor> _datasetBuilder;

    private IContextInfoDataset<ContextInfo, MethodListTensor>? _dataset;

    private readonly object _lock = new object();

    public ContextInfoDatasetProviderMethodList(
        IParsingOrchestrator parsingOrchestrant,
        IContextInfoDatasetBuilder<MethodListTensor> datasetBuilder)
        : base(parsingOrchestrant)
    {
        _datasetBuilder = datasetBuilder;
    }

    public async Task<IContextInfoDataset<ContextInfo, MethodListTensor>> GetDatasetAsync(CancellationToken cancellationToken)
    {
        if (_dataset == null)
        {
            await BuildDatasetAsync(cancellationToken);
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

        var contextsList = await GetParsedContextsAsync(cancellationToken);

        // Здесь мы фильтруем и строим датасет специально для списка методов
        var filteredContexts = contextsList
            .Where(c => c.ElementType == ContextInfoElementType.@method)
            .ToList();

        var newDataset = _datasetBuilder.Build(filteredContexts);

        lock (_lock)
        {
            _dataset = newDataset;
        }
    }
}