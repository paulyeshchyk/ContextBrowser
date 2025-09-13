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

    public IHtmlMatrix HtmlMatrix => _lazyHtmlMatrix.Value;

    public HtmlPageProducerMatrix(IHtmlMatrixGenerator htmlMatrixGenerator, IContextInfoDataset<ContextInfo> dataset, IContextKeyIndexer<ContextInfo> indexer, IHtmlMatrixSummaryBuilder matrixSummaryBuilder, HtmlTableOptions options) : base()
    {
        _dataset = dataset;
        _matrixSummaryBuilder = matrixSummaryBuilder;
        _options = options;
        _lazyHtmlMatrix = new Lazy<IHtmlMatrix>(() => htmlMatrixGenerator.Generate());
        _indexer = indexer;
    }

    protected override IEnumerable<string> GetScripts()
    {
        yield return Resources.HtmlProducerContentStyleScript; // базовый скрипт, если нужен
        yield return Resources.ScriptAutoShrinkEmbeddedTable;  // специфичный скрипт для подтаблицы
        yield return Resources.ScriptAutoFontShrink;
    }

    //context: htmlmatrix, build
    protected override void WriteContent(TextWriter writer)
    {
        var hrefManager = new HrefManager();
        var fixedHtmlManager = new FixedHtmlContentManager();

        var _cellStyleBuilder = new CoverManager();
        var dataCellBuilder = new HtmlDataCellBuilder<ContextKey>(this, hrefManager, _cellStyleBuilder, _dataset, _options, _indexer);
        var matrixWriter = new HtmlMatrixWriter(dataCellBuilder: dataCellBuilder, dataset: _dataset, matrix: HtmlMatrix, hrefManager: hrefManager, fixedHtmlContentManager: fixedHtmlManager, summaryBuilder: _matrixSummaryBuilder, options: _options);

        HtmlBuilderFactory.Breadcrumb(new BreadcrumbNavigationItem("..\\index.html", "Контекстная матрица")).Cell(writer);

        HtmlBuilderFactory.P.Cell(writer);

        HtmlBuilderFactory.Table.With(writer, () =>
        {
            matrixWriter
                .WriteHeaderRow(writer)
                .WriteSummaryRowIf(writer, SummaryPlacementType.AfterFirst)
                .WriteAllDataRows(writer)
                .WriteSummaryRowIf(writer, SummaryPlacementType.AfterLast);
        });
    }

    //context: ContextInfoMatrix, build
    public string ProduceData(IContextKey container)
    {
        _dataset.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;
        var builderResult = CellWithCoverageBuilder.Build(container, cnt);
        return builderResult;
    }
}