using System.Threading;
using ContextKit.Model;
using HtmlKit.Options;
using HtmlKit.Writer;

namespace HtmlKit.Document;

public class HtmlCellDataProducerDomainPerAction : IHtmlCellDataProducer
{
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IHtmlContentInjector _contentInjector;

    public HtmlCellDataProducerDomainPerAction(IContextInfoDatasetProvider datasetProvider, IHtmlContentInjector contentInjector)
    {
        _datasetProvider = datasetProvider;
        _contentInjector = contentInjector;
    }

    public string ProduceData(IContextKey container)
    {
        var dataset = _datasetProvider.GetDatasetAsync(CancellationToken.None).GetAwaiter().GetResult();
        dataset.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;

        // вставка вложенного контента
        var result = _contentInjector.Inject(container, cnt);
        return result;
    }
}