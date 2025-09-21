using System.Collections.Generic;
using System.IO;
using System.Threading;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Helpers;
using HtmlKit.Options;
using HtmlKit.Page;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;

namespace HtmlKit.Document;

public class HtmlDataCellBuilderCoverage<TTensor> : IHtmlDataCellBuilder<TTensor>
    where TTensor : notnull
{
    private readonly IHtmlCellDataProducer<string, TTensor> _coverageDataProducer;
    private readonly IHtmlCellDataProducer<List<ContextInfo>, TTensor> _contextInfoListDataProducer;
    private readonly IHrefManager<TTensor> _hRefManager;
    private readonly IHtmlCellStyleBuilder<TTensor> _cellStyleBuilder;
    private readonly IContextInfoDatasetProvider<TTensor> _datasetProvider;

    private readonly Dictionary<object, ContextInfo>? _indexData;

    public HtmlDataCellBuilderCoverage(
        IHtmlCellDataProducer<string, TTensor> coverageDataProducer, 
        IHtmlCellDataProducer<List<ContextInfo>, TTensor> contextInfoListDataProducer, 
        IHrefManager<TTensor> hRefManager, 
        IHtmlCellStyleBuilder<TTensor> cellStyleBuilder, 
        IKeyIndexBuilder<ContextInfo> coverageIndexer, IContextInfoDatasetProvider<TTensor> datasetProvider)
    {
        _coverageDataProducer = coverageDataProducer;
        _contextInfoListDataProducer = contextInfoListDataProducer;

        _hRefManager = hRefManager;
        _cellStyleBuilder = cellStyleBuilder;
        _datasetProvider = datasetProvider;

        _indexData = coverageIndexer.GetIndexData();
    }

    public void BuildDataCell(TextWriter textWriter, TTensor cell, HtmlTableOptions options)
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
