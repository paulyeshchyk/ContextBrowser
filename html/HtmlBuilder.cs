using System.Net;

namespace ContextBrowser.html;

public interface IHtmlTagBuilder
{
    void Start(TextWriter sb);

    void End(TextWriter sb);
}

public interface IHtmlCellBuilder
{
    void Cell(TextWriter sb, string? innerHtml = "", string? href = null, string? style = null);
}

public interface IHtmlBuilder : IHtmlTagBuilder, IHtmlCellBuilder
{
}

public static class IHtmlBuilderExtensions
{
    public static void With(this IHtmlBuilder builder, TextWriter sb, Action<TextWriter> body)
    {
        builder.Start(sb);
        body(sb);
        builder.End(sb);
    }

    public static void With(this IHtmlTagBuilder builder, TextWriter sb, Action body)
    {
        builder.Start(sb);
        body();
        builder.End(sb);
    }
}

public abstract class HtmlBuilder : IHtmlBuilder
{
    protected readonly string Tag;
    protected readonly string ClassName;

    protected HtmlBuilder(string tag, string className)
    {
        Tag = tag;
        ClassName = className;
    }

    public virtual void Start(TextWriter sb)
    {
        string classAttr = HtmlBuilderTagAttribute.BuildClassAttribute(ClassName);
        sb.WriteLine($"<{Tag}{classAttr}>");
    }

    public virtual void End(TextWriter sb) => sb.WriteLine($"</{Tag}>");

    public virtual void Cell(TextWriter sb, string? innerHtml = "", string? href = null, string? style = null)
    {
        var content = string.IsNullOrWhiteSpace(href)
            ? innerHtml
            : new HtmlBuilderLink(href, innerHtml).ToString();

        WriteContentTag(sb, content, style);
    }

    protected void WriteContentTag(TextWriter sb, string? content = "", string? style = null)
    {
        string classAttr = HtmlBuilderTagAttribute.BuildClassAttribute(ClassName);
        string styleAttr = HtmlBuilderTagAttribute.BuildStyleAttribute(style);
        sb.WriteLine($"<{Tag}{classAttr}{styleAttr}>{content}</{Tag}>");
    }
}

public static class HtmlBuilderRow
{
    public static readonly IHtmlBuilder Summary = new RowBuilder(HtmlTagClasses.Row.Summary);
    public static readonly IHtmlBuilder Data = new RowBuilder(HtmlTagClasses.Row.Data);
    public static readonly IHtmlBuilder Meta = new RowBuilder(HtmlTagClasses.Row.Meta);

    private class RowBuilder : HtmlBuilder
    {
        public RowBuilder(string className) : base("tr", className)
        {
        }
    }
}

public static class HtmlBuilderCell
{
    // Общие теги, поведение которых не требует Cell
    public static readonly IHtmlTagBuilder Html = new HtmlBuilderStandard("html", HtmlTagClasses.Page.Html);
    public static readonly IHtmlTagBuilder Head = new HtmlBuilderStandard("head", HtmlTagClasses.Page.Head);
    public static readonly IHtmlTagBuilder Body = new HtmlBuilderStandard("body", HtmlTagClasses.Page.Body);
    public static readonly IHtmlTagBuilder Table = new HtmlBuilderStandard("table", HtmlTagClasses.Page.Table);

    // Теги с контентом — но без ссылки
    public static readonly IHtmlCellBuilder Title = new HtmlBuilderTitle("title", HtmlTagClasses.Page.Title);
    public static readonly IHtmlCellBuilder Script = new HtmlBuilderContent("script");
    public static readonly IHtmlCellBuilder Style = new HtmlBuilderContent("style");
    public static readonly IHtmlCellBuilder H1 = new HtmlBuilderContent("h1", HtmlTagClasses.Page.H1);

    private class HtmlBuilderStandard : IHtmlTagBuilder
    {
        protected readonly string Tag;
        protected readonly string ClassName;

        public HtmlBuilderStandard(string tag, string className)
        {
            Tag = tag;
            ClassName = className;
        }

        public virtual void Start(TextWriter sb)
        {
            string classAttr = HtmlBuilderTagAttribute.BuildClassAttribute(ClassName);
            sb.WriteLine($"<{Tag}{classAttr}>");
        }

        public virtual void End(TextWriter sb)
        {
            sb.WriteLine($"</{Tag}>");
        }
    }


    private class HtmlBuilderTitle : HtmlBuilder, IHtmlCellBuilder
    {
        public HtmlBuilderTitle(string tag, string className) : base(tag, className)
        {
        }

        public override void Start(TextWriter sb)
        {
            sb.Write($"<{Tag}{HtmlBuilderTagAttribute.BuildClassAttribute(ClassName)}>");
        }

        public override void End(TextWriter sb)
        {
            sb.WriteLine($"</{Tag}>");
        }

        public override void Cell(TextWriter sb, string? innerHtml = "", string? href = null, string? style = null)
        {
            WriteContentTag(sb, WebUtility.HtmlEncode(innerHtml), style);
        }
    }

    /// <summary>
    /// Универсальный контент-билдер для скриптов, стилей и заголовков
    /// </summary>
    private class HtmlBuilderContent : HtmlBuilder, IHtmlCellBuilder
    {
        public HtmlBuilderContent(string tag, string? className = null) : base(tag, className ?? string.Empty)
        {
        }

        public override void Start(TextWriter sb)
        {
            string classAttr = HtmlBuilderTagAttribute.BuildClassAttribute(ClassName);
            sb.Write($"<{Tag}{classAttr}>");
        }

        public override void End(TextWriter sb)
        {
            sb.WriteLine($"</{Tag}>");
        }

        public override void Cell(TextWriter sb, string? innerHtml = "", string? href = null, string? style = null)
        {
            WriteContentTag(sb, innerHtml, style);
        }
    }


    public static readonly IHtmlBuilder SummaryCaption = new CellBuilder(HtmlTagClasses.Cell.SummaryCaption);
    public static readonly IHtmlBuilder TotalSummary = new CellBuilder(HtmlTagClasses.Cell.TotalSummary);
    public static readonly IHtmlBuilder ColSummary = new CellBuilder(HtmlTagClasses.Cell.ColSummary);
    public static readonly IHtmlBuilder ColMeta = new CellBuilder(HtmlTagClasses.Cell.ColMeta);
    public static readonly IHtmlBuilder RowMeta = new CellBuilder(HtmlTagClasses.Cell.RowMeta);
    public static readonly IHtmlBuilder RowSummary = new CellBuilder(HtmlTagClasses.Cell.RowSummary);
    public static readonly IHtmlBuilder Data = new CellBuilder(HtmlTagClasses.Cell.Data);
    public static readonly IHtmlBuilder ActionDomain = new CellBuilder(HtmlTagClasses.Cell.ActionDomain);

    private class CellBuilder : HtmlBuilder
    {
        public CellBuilder(string className) : base("td", className)
        {
        }
    }
}

public static class HtmlBuilderTagAttribute
{
    public static string BuildAttributes(IDictionary<string, string?>? attributes)
    {
        if(attributes == null || attributes.Count == 0)
        {
            return string.Empty;
        }

        var sb = new System.Text.StringBuilder();
        foreach(var kvp in attributes)
        {
            if(!string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value))
            {
                sb.Append($" {kvp.Key}=\"{kvp.Value}\"");
            }
        }
        return sb.ToString();
    }

    // Overload for simpler common attributes like class/style
    public static string BuildClassAttribute(string? className)
    {
        return string.IsNullOrWhiteSpace(className) ? string.Empty : $" class=\"{className}\"";
    }

    public static string BuildStyleAttribute(string? style)
    {
        return string.IsNullOrWhiteSpace(style) ? string.Empty : $" {style}";
    }
}

public class HtmlBuilderLink
{
    private readonly string _href;
    private readonly string _text;

    public HtmlBuilderLink(string href, string? text = "")
    {
        _href = WebUtility.HtmlEncode(href);
        _text = WebUtility.HtmlEncode(text ?? string.Empty);
    }

    public override string ToString()
    {
        var Tag = "a";
        return $"<{Tag} href=\"{_href}\">{_text}</{Tag}>";
    }
}