using System.Collections.Generic;
using System.IO;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Document.Coverage;
using HtmlKit.Helpers;
using HtmlKit.Options;
using HtmlKit.Page;
using HtmlKit.Writer;

namespace HtmlKit.Document;

//context: htmlmatrix, model
public class HtmlPageMatrix : HtmlPage, IHtmlPageMatrix
{
    public Dictionary<string, ContextInfo> IndexMap { get; }

    private HtmlTableOptions _options { get; }

    public HtmlMatrix UiMatrix { get; }

    public IContextInfoDataset ContextsMatrix { get; }

    private readonly CoverManager _coverManager = new CoverManager();

    public ICoverageManager CoverageManager => _coverManager;

    public HtmlPageMatrix(HtmlMatrix uiMatrix, IContextInfoDataset matrix, HtmlTableOptions options, Dictionary<string, ContextInfo> indexMap) : base()
    {
        UiMatrix = uiMatrix;
        ContextsMatrix = matrix;
        _options = options;
        IndexMap = indexMap;
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
            new HtmlMatrixWriter(htmlPageMatrix: this, hrefManager: hrefManager, fixedHtmlContentManager: fixedHtmlManager, options: _options)
                .WriteHeaderRow(writer)
                .WriteSummaryRowIf(writer, SummaryPlacementType.AfterFirst)
                .WriteAllDataRows(writer)
                .WriteSummaryRowIf(writer, SummaryPlacementType.AfterLast);
        });
    }

    //context: ContextInfoMatrix, build
    public string ProduceData(IContextKey container)
    {
        ContextsMatrix.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;
        var builderResult = CellWithCoverageBuilder.Build(container, cnt);
        return builderResult;
    }
}