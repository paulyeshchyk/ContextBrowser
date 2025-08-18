using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Tag;

// pattern: Builder
public class HtmlBuilderContent : HtmlBuilder, IHtmlCellBuilder
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