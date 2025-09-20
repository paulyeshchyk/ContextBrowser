using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using HtmlKit.Options;
using HtmlKit.Writer;
using TensorKit.Model;

namespace HtmlKit.Document;

public class HtmlCellDataProducerListOfItems<TKey> : IHtmlCellDataProducer<List<ContextInfo>, TKey>
    where TKey : notnull
{
    private readonly IContextInfoDatasetProvider<TKey> _datasetProvider;
    private readonly IHtmlContentInjector<TKey> _contentInjector;

    public HtmlCellDataProducerListOfItems(IContextInfoDatasetProvider<TKey> datasetProvider, IHtmlContentInjector<TKey> contentInjector)
    {
        _datasetProvider = datasetProvider;
        _contentInjector = contentInjector;
    }

    public async Task<List<ContextInfo>> ProduceDataAsync(TKey container, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        dataset.TryGetValue(container, out var result);
        return result;
    }
}

public class HtmlCellDataProducerDomainPerActionMethodsCount<TKey> : IHtmlCellDataProducer<string, TKey>
    where TKey : notnull
{
    private readonly IContextInfoDatasetProvider<TKey> _datasetProvider;
    private readonly IHtmlContentInjector<TKey> _contentInjector;

    public HtmlCellDataProducerDomainPerActionMethodsCount(IContextInfoDatasetProvider<TKey> datasetProvider, IHtmlContentInjector<TKey> contentInjector)
    {
        _datasetProvider = datasetProvider;
        _contentInjector = contentInjector;
    }

    public async Task<string> ProduceDataAsync(TKey container, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        dataset.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;

        // вставка вложенного контента
        var result = _contentInjector.Inject(container, cnt);
        return result;
    }
}