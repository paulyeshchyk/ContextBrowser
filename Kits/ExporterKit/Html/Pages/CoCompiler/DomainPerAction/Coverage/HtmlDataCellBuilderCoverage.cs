using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page;
using HtmlKit.Document;
using HtmlKit.Helpers;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction.Coverage;

public class HtmlDataCellBuilderCoverage<TTensor> : IHtmlDataCellBuilder<TTensor>
    where TTensor : notnull
{
    private readonly IHtmlCellDataProducer<string, TTensor> _coverageDataProducer;
    private readonly IHtmlCellDataProducer<List<ContextInfo>, TTensor> _contextInfoListDataProducer;
    private readonly IHtmlHrefManager<TTensor> _hRefManager;
    private readonly IHtmlCellStyleBuilder<TTensor> _cellStyleBuilder;
    private readonly IContextInfoDatasetProvider<TTensor> _datasetProvider;
    private readonly IKeyIndexBuilder<ContextInfo> _coverageIndexer;
    private readonly INamingProcessor _namingProcessor;

    private Dictionary<object, ContextInfo>? _indexData;

    public HtmlDataCellBuilderCoverage(
        IHtmlCellDataProducer<string, TTensor> coverageDataProducer,
        IHtmlCellDataProducer<List<ContextInfo>, TTensor> contextInfoListDataProducer,
        IHtmlHrefManager<TTensor> hRefManager,
        IHtmlCellStyleBuilder<TTensor> cellStyleBuilder,
        IKeyIndexBuilder<ContextInfo> coverageIndexer,
        IContextInfoDatasetProvider<TTensor> datasetProvider,
        INamingProcessor namingProcessor)
    {
        _coverageDataProducer = coverageDataProducer;
        _contextInfoListDataProducer = contextInfoListDataProducer;

        _hRefManager = hRefManager;
        _cellStyleBuilder = cellStyleBuilder;
        _datasetProvider = datasetProvider;

        _coverageIndexer = coverageIndexer;
        _namingProcessor = namingProcessor;

    }

    public async Task BuildDataCell(TextWriter textWriter, TTensor cell, HtmlTableOptions options, CancellationToken cancellationToken)
    {
        if (_indexData == null)
        {
            var ds = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);
            var list = ds.GetAll();

            _indexData = _coverageIndexer.Build(list);
        }

        var cellData = await _coverageDataProducer.ProduceDataAsync(cell, cancellationToken).ConfigureAwait(false);
        var contextList = await _contextInfoListDataProducer.ProduceDataAsync(cell, cancellationToken).ConfigureAwait(false);

        var styleValue = _cellStyleBuilder.BuildCellStyleValue(cell, contextList, _indexData);

        await HtmlBuilderFactory.HtmlBuilderTableCell.Data.WithAsync(textWriter, (token) =>
        {
            var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefCell(cell, options, _namingProcessor) } };
            if (!string.IsNullOrWhiteSpace(styleValue))
                attrs["style"] = styleValue;
            HtmlBuilderFactory.A.CellAsync(textWriter, attrs, cellData, isEncodable: false);
            return Task.CompletedTask;
        }, cancellationToken).ConfigureAwait(false);
    }
}
