using ContextBrowserKit.Matrix;
using ContextKit.Model;
using HtmlKit.Builders.Core;
using HtmlKit.Document.Coverage;
using HtmlKit.Model;
using HtmlKit.Page;
using HtmlKit.Writer;

namespace HtmlKit.Document;

public class HtmlPageMatrix : HtmlPage, IHtmlPageMatrix
{
    public Dictionary<string, ContextInfo> ContextsLookup { get; }

    public HtmlTableOptions Options { get; }

    public UiMatrix UiMatrix { get; }

    public Dictionary<ContextContainer, List<string>> ContextsMatrix { get; }

    private CoverManager _coverManager = new CoverManager();

    public ICoverageManager CoverageManager => _coverManager;

    public HtmlPageMatrix(UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix, HtmlTableOptions options, Dictionary<string, ContextInfo> contextLookup) : base()
    {
        UiMatrix = uiMatrix;
        ContextsMatrix = matrix;
        Options = options;
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
            new HtmlMatrixWriter(this)
                .WriteHeaderRow(tw)
                .WriteSummaryRowIf(tw, SummaryPlacement.AfterFirst)
                .WriteAllDataRows(tw)
                .WriteSummaryRowIf(tw, SummaryPlacement.AfterLast);
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