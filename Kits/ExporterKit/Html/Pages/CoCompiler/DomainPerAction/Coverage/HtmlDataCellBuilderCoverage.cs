using System.Collections.Generic;
using System.IO;
using System.Threading;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Helpers;
using HtmlKit.Options;
using HtmlKit.Page;
using TensorKit.Model;

namespace HtmlKit.Document;

public class HtmlDataCellBuilderCoverage<TKey> : IHtmlDataCellBuilder<TKey>
    where TKey : notnull
{
    private readonly IHtmlCellDataProducer<string, TKey> _coverageDataProducer;
    private readonly IHtmlCellDataProducer<List<ContextInfo>, TKey> _contextInfoListDataProducer;
    private readonly IHrefManager<TKey> _hRefManager;
    private readonly IHtmlCellStyleBuilder<TKey> _cellStyleBuilder;
    private readonly IContextInfoDatasetProvider<TKey> _datasetProvider;

    private readonly Dictionary<string, ContextInfo>? _indexData;

    public HtmlDataCellBuilderCoverage(
        IHtmlCellDataProducer<string, TKey> coverageDataProducer, 
        IHtmlCellDataProducer<List<ContextInfo>, TKey> contextInfoListDataProducer, 
        IHrefManager<TKey> hRefManager, 
        IHtmlCellStyleBuilder<TKey> cellStyleBuilder, 
        IKeyIndexBuilder<ContextInfo> coverageIndexer, IContextInfoDatasetProvider<TKey> datasetProvider)
    {
        _coverageDataProducer = coverageDataProducer;
        _contextInfoListDataProducer = contextInfoListDataProducer;

        _hRefManager = hRefManager;
        _cellStyleBuilder = cellStyleBuilder;
        _datasetProvider = datasetProvider;

        _indexData = coverageIndexer.GetIndexData();
    }

    public void BuildDataCell(TextWriter textWriter, TKey cell, HtmlTableOptions options)
    {
        var cellData = _coverageDataProducer.ProduceDataAsync(cell, CancellationToken.None).GetAwaiter().GetResult();
        var contextList = _contextInfoListDataProducer.ProduceDataAsync(cell, CancellationToken.None).GetAwaiter().GetResult();

        var style = _cellStyleBuilder.BuildCellStyle(cell, contextList, _indexData);

        HtmlBuilderFactory.HtmlBuilderTableCell.Data.With(textWriter, () =>
        {
            var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefCell(cell, options) } };
            if (!string.IsNullOrWhiteSpace(style))
                attrs["style"] = style;
            HtmlBuilderFactory.A.Cell(textWriter, attrs, cellData, isEncodable: false);
        });
    }
}