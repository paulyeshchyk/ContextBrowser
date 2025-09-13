using System;
using System.Data;
using System.IO;
using System.Threading;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Helpers;
using HtmlKit.Model;
using HtmlKit.Options;
using HtmlKit.Page;

namespace HtmlKit.Document;

public interface IHtmlDataCellBuilder<TKey>
    where TKey : ContextKey
{
    void BuildDataCell(TextWriter textWriter, TKey cell, HtmlTableOptions options);
}

public class HtmlDataCellBuilder<TKey> : IHtmlDataCellBuilder<TKey>
    where TKey : ContextKey
{
    private readonly IHtmlCellDataProducer _dataProducer;
    private readonly IHrefManager _hRefManager;
    private readonly IHtmlCellStyleBuilder _cellStyleBuilder;
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IContextInfoDataset<ContextInfo> _dataset;
    private readonly IContextKeyIndexer<ContextInfo> _coverageIndexer;

    public HtmlDataCellBuilder(IHtmlCellDataProducer dataProducer, IHrefManager hRefManager, IHtmlCellStyleBuilder cellStyleBuilder, IContextKeyIndexer<ContextInfo> coverageIndexer, IContextInfoDatasetProvider datasetProvider)
    {
        _dataProducer = dataProducer;
        _hRefManager = hRefManager;
        _cellStyleBuilder = cellStyleBuilder;
        _coverageIndexer = coverageIndexer;
        _datasetProvider = datasetProvider;

        _dataset = _datasetProvider.GetDatasetAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    public void BuildDataCell(TextWriter textWriter, TKey cell, HtmlTableOptions options)
    {
        var indexData = _coverageIndexer.GetIndexData();

        var data = _dataProducer.ProduceData(cell);

        HtmlBuilderFactory.HtmlBuilderTableCell.Data.With(textWriter, () =>
        {
            var style = _cellStyleBuilder.BuildCellStyle(cell, _dataset, indexData);
            var attrs = new HtmlTagAttributes() { { "href", _hRefManager.GetHrefCell(cell, options) } };
            if (!string.IsNullOrWhiteSpace(style))
                attrs["style"] = style;
            HtmlBuilderFactory.A.Cell(textWriter, attrs, data, isEncodable: false);
        });
    }
}