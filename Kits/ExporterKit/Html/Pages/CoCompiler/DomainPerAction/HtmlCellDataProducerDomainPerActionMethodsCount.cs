using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using HtmlKit.Document;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

public class HtmlCellDataProducerListOfItems<TTensor> : IHtmlCellDataProducer<List<ContextInfo>, TTensor>
    where TTensor : notnull
{
    private readonly IContextInfoDatasetProvider<TTensor> _datasetProvider;
    private readonly IHtmlContentInjector<TTensor> _contentInjector;

    public HtmlCellDataProducerListOfItems(IContextInfoDatasetProvider<TTensor> datasetProvider, IHtmlContentInjector<TTensor> contentInjector)
    {
        _datasetProvider = datasetProvider;
        _contentInjector = contentInjector;
    }

    public async Task<List<ContextInfo>> ProduceDataAsync(TTensor container, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        dataset.TryGetValue(container, out var result);
        return result;
    }
}

public class HtmlCellDataProducerDomainPerActionMethodsCount<TTensor> : IHtmlCellDataProducer<string, TTensor>
    where TTensor : notnull
{
    private readonly IContextInfoDatasetProvider<TTensor> _datasetProvider;
    private readonly IHtmlContentInjector<TTensor> _contentInjector;

    public HtmlCellDataProducerDomainPerActionMethodsCount(IContextInfoDatasetProvider<TTensor> datasetProvider, IHtmlContentInjector<TTensor> contentInjector)
    {
        _datasetProvider = datasetProvider;
        _contentInjector = contentInjector;
    }

    public async Task<string> ProduceDataAsync(TTensor container, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        dataset.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;

        // вставка вложенного контента
        var result = _contentInjector.Inject(container, cnt);
        return result;
    }
}

public class HtmlCellDataProducerMethodList<TTensor> : IHtmlCellDataProducer<string, TTensor>
    where TTensor : notnull
{
    private readonly IContextInfoDatasetProvider<TTensor> _datasetProvider;
    private readonly IHtmlContentInjector<TTensor> _contentInjector;

    public HtmlCellDataProducerMethodList(IContextInfoDatasetProvider<TTensor> datasetProvider, IHtmlContentInjector<TTensor> contentInjector)
    {
        _datasetProvider = datasetProvider;
        _contentInjector = contentInjector;
    }

    public async Task<string> ProduceDataAsync(TTensor container, CancellationToken cancellationToken)
    {
        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);

        dataset.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;

        // вставка вложенного контента
        var result = _contentInjector.Inject(container, cnt);
        return result;
    }
}