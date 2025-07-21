using System.Text;

namespace ContextBrowser.model;

public interface IHtmlBuilder
{
    void Start(StringBuilder sb);

    void End(StringBuilder sb);

    void Cell(StringBuilder sb, string innerHtml, string? href = null, string? style = null);
}

public abstract class HtmlBuilderBase : IHtmlBuilder
{
    protected readonly string Tag;
    protected readonly string ClassName;

    protected HtmlBuilderBase(string tag, string className)
    {
        Tag = tag;
        ClassName = className;
    }

    public virtual void Start(StringBuilder sb) => sb.Append($"<{Tag} class=\"{ClassName}\">");

    public virtual void End(StringBuilder sb) => sb.Append($"</{Tag}>");

    public virtual void Cell(StringBuilder sb, string innerHtml, string? href = null, string? style = null)
    {
        var content = string.IsNullOrWhiteSpace(href)
            ? innerHtml
            : $"<a href=\"{href}\">{innerHtml}</a>";

        sb.Append($"<{Tag} class=\"{ClassName}\" {style}>{content}</{Tag}>");
    }
}

public static class HtmlRowBuilder
{
    public static readonly IHtmlBuilder Summary = new RowBuilder(HtmlClasses.Row.Summary);
    public static readonly IHtmlBuilder Data = new RowBuilder(HtmlClasses.Row.Data);
    public static readonly IHtmlBuilder Meta = new RowBuilder(HtmlClasses.Row.Meta);

    private class RowBuilder : HtmlBuilderBase
    {
        public RowBuilder(string className) : base("tr", className)
        {
        }
    }
}

public static class HtmlCellBuilder
{
    public static readonly IHtmlBuilder SummaryCaption = new CellBuilder(HtmlClasses.Cell.SummaryCaption);
    public static readonly IHtmlBuilder TotalSummary = new CellBuilder(HtmlClasses.Cell.TotalSummary);
    public static readonly IHtmlBuilder ColSummary = new CellBuilder(HtmlClasses.Cell.ColSummary);
    public static readonly IHtmlBuilder ColMeta = new CellBuilder(HtmlClasses.Cell.ColMeta);
    public static readonly IHtmlBuilder RowMeta = new CellBuilder(HtmlClasses.Cell.RowMeta);
    public static readonly IHtmlBuilder RowSummary = new CellBuilder(HtmlClasses.Cell.RowSummary);
    public static readonly IHtmlBuilder Data = new CellBuilder(HtmlClasses.Cell.Data);
    public static readonly IHtmlBuilder ActionDomain = new CellBuilder(HtmlClasses.Cell.ActionDomain);

    private class CellBuilder : HtmlBuilderBase
    {
        public CellBuilder(string className) : base("td", className)
        {
        }
    }
}