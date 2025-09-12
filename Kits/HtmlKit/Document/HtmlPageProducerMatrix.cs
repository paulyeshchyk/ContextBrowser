using System;
using System.Collections.Generic;
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
    private HtmlTableOptions _options { get; }

    private readonly Lazy<IHtmlMatrix> _lazyHtmlMatrix;

    private IHtmlMatrixGenerator _htmlMatrixGenerator { get; }

    private readonly IHtmlMatrixSummaryBuilder _matrixSummaryBuilder;

    public IContextInfoIndexerProvider IndexerProvider { get; }

    private readonly CoverManager _coverManager = new CoverManager();

    public IContextInfoDataset<ContextInfo> Dataset { get; }

    public ICoverageManager CoverageManager => _coverManager;

    public IHtmlMatrix HtmlMatrix => _lazyHtmlMatrix.Value;

    public HtmlPageProducerMatrix(IHtmlMatrixGenerator htmlMatrixGenerator, IContextInfoDataset<ContextInfo> dataset, IContextInfoIndexerProvider flatMapperProvider, IHtmlMatrixSummaryBuilder matrixSummaryBuilder, HtmlTableOptions options) : base()
    {
        _htmlMatrixGenerator = htmlMatrixGenerator;
        IndexerProvider = flatMapperProvider;
        Dataset = dataset;
        _matrixSummaryBuilder = matrixSummaryBuilder;
        _options = options;
        _lazyHtmlMatrix = new Lazy<IHtmlMatrix>(() => htmlMatrixGenerator.Generate());
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
        var indexer = IndexerProvider.GetIndexerAsync(GlobalMapperKeys.NameClassName, CancellationToken.None).GetAwaiter().GetResult();
        HtmlBuilderFactory.Breadcrumb(new BreadcrumbNavigationItem("..\\index.html", "Контекстная матрица")).Cell(writer);

        HtmlBuilderFactory.P.Cell(writer);

        HtmlBuilderFactory.Table.With(writer, () =>
        {
            new HtmlMatrixWriter(coverageManager: CoverageManager, dataProducer: this, dataset: Dataset, matrix: HtmlMatrix, indexer: indexer, hrefManager: hrefManager, fixedHtmlContentManager: fixedHtmlManager, summaryBuilder: _matrixSummaryBuilder, options: _options)
                .WriteHeaderRow(writer)
                .WriteSummaryRowIf(writer, SummaryPlacementType.AfterFirst)
                .WriteAllDataRows(writer)
                .WriteSummaryRowIf(writer, SummaryPlacementType.AfterLast);
        });
    }

    //context: ContextInfoMatrix, build
    public string ProduceData(IContextKey container)
    {
        Dataset.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;
        var builderResult = CellWithCoverageBuilder.Build(container, cnt);
        return builderResult;
    }
}