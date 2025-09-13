using System.Threading;
using ContextKit.Model;
using HtmlKit.Writer;

namespace HtmlKit.Document;

public class HtmlPageDataProducerDomainAction : IHtmlPageDataProducer
{
    private readonly IContextInfoDatasetProvider _datasetProvider;

    public HtmlPageDataProducerDomainAction(IContextInfoDatasetProvider datasetProvider)
    {
        _datasetProvider = datasetProvider;
    }

    public string ProduceData(IContextKey container)
    {
        var dataset = _datasetProvider.GetDatasetAsync(CancellationToken.None).GetAwaiter().GetResult();
        dataset.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;
        var builderResult = CellWithCoverageBuilder.Build(container, cnt);
        return builderResult;
    }
}