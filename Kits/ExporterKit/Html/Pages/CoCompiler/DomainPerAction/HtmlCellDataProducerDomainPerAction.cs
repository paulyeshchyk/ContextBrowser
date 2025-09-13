using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using HtmlKit.Options;
using HtmlKit.Writer;

namespace HtmlKit.Document;

public class HtmlCellDataProducer : IHtmlCellDataProducer<List<ContextInfo>>
{
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IHtmlContentInjector _contentInjector;

    public HtmlCellDataProducer(IContextInfoDatasetProvider datasetProvider, IHtmlContentInjector contentInjector)
    {
        _datasetProvider = datasetProvider;
        _contentInjector = contentInjector;
    }

    public async Task<List<ContextInfo>> ProduceDataAsync(IContextKey container, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        dataset.TryGetValue(container, out var result);
        return result;
    }
}

public class HtmlCellDataProducerDomainPerAction : IHtmlCellDataProducer<string>
{
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IHtmlContentInjector _contentInjector;

    public HtmlCellDataProducerDomainPerAction(IContextInfoDatasetProvider datasetProvider, IHtmlContentInjector contentInjector)
    {
        _datasetProvider = datasetProvider;
        _contentInjector = contentInjector;
    }

    public async Task<string> ProduceDataAsync(IContextKey container, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        dataset.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;

        // вставка вложенного контента
        var result = _contentInjector.Inject(container, cnt);
        return result;
    }
}