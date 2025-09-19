using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using HtmlKit.Options;
using HtmlKit.Writer;
using TensorKit.Model;

namespace HtmlKit.Document;

public class HtmlCellDataProducerrDomainPerActionMethodsList : IHtmlCellDataProducer<List<ContextInfo>, DomainPerActionTensor>
{
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IHtmlContentInjector _contentInjector;

    public HtmlCellDataProducerrDomainPerActionMethodsList(IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IHtmlContentInjector contentInjector)
    {
        _datasetProvider = datasetProvider;
        _contentInjector = contentInjector;
    }

    public async Task<List<ContextInfo>> ProduceDataAsync(DomainPerActionTensor container, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        dataset.TryGetValue(container, out var result);
        return result;
    }
}

public class HtmlCellDataProducerDomainPerActionMethodsCount : IHtmlCellDataProducer<string, DomainPerActionTensor>
{
    private readonly IContextInfoDatasetProvider<DomainPerActionTensor> _datasetProvider;
    private readonly IHtmlContentInjector _contentInjector;

    public HtmlCellDataProducerDomainPerActionMethodsCount(IContextInfoDatasetProvider<DomainPerActionTensor> datasetProvider, IHtmlContentInjector contentInjector)
    {
        _datasetProvider = datasetProvider;
        _contentInjector = contentInjector;
    }

    public async Task<string> ProduceDataAsync(DomainPerActionTensor container, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        dataset.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;

        // вставка вложенного контента
        var result = _contentInjector.Inject(container, cnt);
        return result;
    }
}