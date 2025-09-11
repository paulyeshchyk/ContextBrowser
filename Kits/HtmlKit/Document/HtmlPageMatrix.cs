using System;
using System.Collections.Generic;
using System.IO;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Collector;
using HtmlKit.Builders.Core;
using HtmlKit.Document.Coverage;
using HtmlKit.Extensions;
using HtmlKit.Helpers;
using HtmlKit.Options;
using HtmlKit.Page;
using HtmlKit.Writer;

namespace HtmlKit.Document;

//context: htmlmatrix, model
public class HtmlPageMatrix : HtmlPage, IHtmlPageMatrix
{
    public IHtmlMatrixIndexer<ContextInfo> Indexer { get; }

    private HtmlTableOptions _options { get; }

    public IHtmlMatrixGenerator HtmlMatrixGenerator { get; }

    public IContextInfoDataset<ContextInfo> Dataset { get; }

    public readonly IUiMatrixSummaryBuilder UiMatrixSummaryBuilder;

    private readonly CoverManager _coverManager = new CoverManager();

    public ICoverageManager CoverageManager => _coverManager;

    private readonly Lazy<IHtmlMatrix> _lazyHtmlMatrix;

    public IHtmlMatrix HtmlMatrix => _lazyHtmlMatrix.Value;

    public HtmlPageMatrix(IHtmlMatrixGenerator htmlMatrixGenerator, IContextInfoDataset<ContextInfo> dataset, IHtmlMatrixIndexer<ContextInfo> indexer, IUiMatrixSummaryBuilder uiMatrixSummaryBuilder, HtmlTableOptions options) : base()
    {
        HtmlMatrixGenerator = htmlMatrixGenerator;
        Dataset = dataset;
        Indexer = indexer;
        UiMatrixSummaryBuilder = uiMatrixSummaryBuilder;
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

        HtmlBuilderFactory.Breadcrumb(new BreadcrumbNavigationItem("..\\index.html", "Контекстная матрица")).Cell(writer);

        HtmlBuilderFactory.P.Cell(writer);

        HtmlBuilderFactory.Table.With(writer, () =>
        {
            new HtmlMatrixWriter(htmlPageMatrix: this, hrefManager: hrefManager, fixedHtmlContentManager: fixedHtmlManager, uiMatrixSummaryBuilder: UiMatrixSummaryBuilder, options: _options)
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