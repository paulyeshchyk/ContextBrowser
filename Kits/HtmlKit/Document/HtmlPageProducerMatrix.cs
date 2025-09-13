using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ExporterKit.Html;
using HtmlKit.Builders.Core;
using HtmlKit.Document.Coverage;
using HtmlKit.Extensions;
using HtmlKit.Helpers;
using HtmlKit.Options;
using HtmlKit.Page;
using HtmlKit.Writer;

namespace HtmlKit.Document;

//context: htmlmatrix, model
public class HtmlPageProducerMatrix : HtmlPageProducer, IHtmlPageMatrix
{
    private readonly HtmlTableOptions _options;
    private readonly Lazy<IHtmlMatrix> _lazyHtmlMatrix;
    private readonly IHtmlMatrixSummaryBuilder _matrixSummaryBuilder;
    private readonly IContextKeyIndexer<ContextInfo> _indexer;
    private readonly IContextInfoDataset<ContextInfo> _dataset;
    private readonly IFixedHtmlContentManager _fixedHtmlManager;
    private readonly IHrefManager _hrefManager;
    private readonly IHtmlPageDataProducer _htmlPageDataProducer;
    private readonly IHtmlDataCellBuilder<ContextKey> _datacellBuilder;
    private readonly IHtmlMatrixWriter _matrixWriter;

    private readonly IHtmlMatrixWriterFactory _htmlMatrixWriterFactory;

    public IHtmlMatrix HtmlMatrix => _lazyHtmlMatrix.Value;

    public HtmlPageProducerMatrix(
        IHtmlMatrixGenerator htmlMatrixGenerator,
        IContextInfoDataset<ContextInfo> dataset,
        IContextKeyIndexer<ContextInfo> indexer,
        IHtmlMatrixSummaryBuilder matrixSummaryBuilder,
        HtmlTableOptions options,
        IFixedHtmlContentManager fixedHtmlManager,
        IHrefManager hrefManager,
        IHtmlPageDataProducer htmlPageDataProducer,
        IHtmlDataCellBuilder<ContextKey> datacellBuilder,
        IHtmlMatrixWriterFactory htmlMatrixWriterFactory) : base()
    {
        _dataset = dataset;
        _matrixSummaryBuilder = matrixSummaryBuilder;
        _options = options;
        _lazyHtmlMatrix = new Lazy<IHtmlMatrix>(() => htmlMatrixGenerator.Generate());
        _indexer = indexer;
        _fixedHtmlManager = fixedHtmlManager;
        _hrefManager = hrefManager;
        _htmlPageDataProducer = htmlPageDataProducer;
        _datacellBuilder = datacellBuilder;
        _htmlMatrixWriterFactory = htmlMatrixWriterFactory;
        _matrixWriter = _htmlMatrixWriterFactory.Create(HtmlMatrix, _options);
    }

    protected override IEnumerable<string> GetScripts()
    {
        yield return Resources.HtmlProducerContentStyleScript;
        yield return Resources.ScriptAutoShrinkEmbeddedTable;
        yield return Resources.ScriptAutoFontShrink;
    }

    protected override void WriteContent(TextWriter writer)
    {
        HtmlBuilderFactory.Breadcrumb(new BreadcrumbNavigationItem("..\\index.html", "Контекстная матрица")).Cell(writer);

        HtmlBuilderFactory.P.Cell(writer);

        HtmlBuilderFactory.Table.With(writer, () =>
        {
            _matrixWriter
                .PrepareSummary()
                .WriteHeaderRow(writer)
                .WriteSummaryRowIf(writer, SummaryPlacementType.AfterFirst)
                .WriteAllDataRows(writer)
                .WriteSummaryRowIf(writer, SummaryPlacementType.AfterLast);
        });
    }

    public string ProduceData(IContextKey container)
    {
        _dataset.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;
        var builderResult = CellWithCoverageBuilder.Build(container, cnt);
        return builderResult;
    }
}
