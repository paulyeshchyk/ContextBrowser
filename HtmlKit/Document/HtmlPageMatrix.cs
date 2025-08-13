using ContextBrowserKit.Matrix;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Document.Coverage;
using HtmlKit.Options;
using HtmlKit.Page;
using HtmlKit.Writer;

namespace HtmlKit.Document;

public class HtmlPageMatrix : HtmlPage, IHtmlPageMatrix
{
    public Dictionary<string, ContextInfo> ContextsLookup { get; }

    private HtmlTableOptions _options { get; }

    public UiMatrix UiMatrix { get; }

    public Dictionary<ContextContainer, List<string>> ContextsMatrix { get; }

    private CoverManager _coverManager = new CoverManager();

    public ICoverageManager CoverageManager => _coverManager;

    public HtmlPageMatrix(UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix, HtmlTableOptions options, Dictionary<string, ContextInfo> contextLookup) : base()
    {
        UiMatrix = uiMatrix;
        ContextsMatrix = matrix;
        _options = options;
        ContextsLookup = contextLookup;
    }

    protected override IEnumerable<string> GetScripts()
    {
        yield return Resources.HtmlProducerContentStyleScript; // базовый скрипт, если нужен
        yield return Resources.ScriptAutoShrinkEmbeddedTable;  // специфичный скрипт для подтаблицы
        yield return Resources.ScriptAutoFontShrink;
    }

    protected override void WriteContent(TextWriter tw)
    {
        HtmlBuilderFactory.Table.With(tw,() =>
        {
            new HtmlMatrixWriter(this, _options)
                .WriteHeaderRow(tw)
                .WriteSummaryRowIf(tw, SummaryPlacementType.AfterFirst)
                .WriteAllDataRows(tw)
                .WriteSummaryRowIf(tw, SummaryPlacementType.AfterLast);
        });
    }

    public string ProduceData(ContextContainer container)
    {
        ContextsMatrix.TryGetValue(container, out var methods);
        var cnt = methods?.Count ?? 0;
        var builderResult = CellWithCoverageBuilder.Build(container, cnt);
        return builderResult;
    }
}