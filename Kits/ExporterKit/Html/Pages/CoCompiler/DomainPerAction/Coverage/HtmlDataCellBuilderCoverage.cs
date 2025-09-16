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

public class HtmlDataCellBuilderCoverage : IHtmlDataCellBuilder<DomainPerActionTensor>
{
    private readonly IHtmlCellDataProducer<string> _coverageDataProducer;
    private readonly IHtmlCellDataProducer<List<ContextInfo>> _contextInfoListDataProducer;
    private readonly IHrefManager _hRefManager;
    private readonly IHtmlCellStyleBuilder _cellStyleBuilder;
    private readonly IContextInfoDatasetProvider _datasetProvider;

    private readonly Dictionary<string, ContextInfo>? _indexData;

    public HtmlDataCellBuilderCoverage(IHtmlCellDataProducer<string> coverageDataProducer, IHtmlCellDataProducer<List<ContextInfo>> contextInfoListDataProducer, IHrefManager hRefManager, IHtmlCellStyleBuilder cellStyleBuilder, DomainPerActionKeyIndexer<ContextInfo> coverageIndexer, IContextInfoDatasetProvider datasetProvider)
    {
        _coverageDataProducer = coverageDataProducer;
        _contextInfoListDataProducer = contextInfoListDataProducer;

        _hRefManager = hRefManager;
        _cellStyleBuilder = cellStyleBuilder;
        _datasetProvider = datasetProvider;

        _indexData = coverageIndexer.GetIndexData();
    }

    public void BuildDataCell(TextWriter textWriter, DomainPerActionTensor cell, HtmlTableOptions options)
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