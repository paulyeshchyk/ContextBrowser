using ContextBrowser.HtmlKit.Builders.Tag;

namespace ContextBrowser.HtmlKit.Builders.Core;

//pattern: Builder, template method
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

    public void Start(TextWriter sb, string? className)
    {
        string classAttr = HtmlBuilderTagAttribute.BuildClassAttribute(className);
        sb.WriteLine($"<{Tag}{classAttr}>");
    }

    public virtual void End(TextWriter sb) => sb.WriteLine($"</{Tag}>");

    public virtual void Cell(TextWriter sb, string? innerHtml = "", string? href = null, string? style = null)
    {
        var content = string.IsNullOrWhiteSpace(href)
            ? innerHtml
            : new HtmlBuilderEncodedAnchorSpecial(href, innerHtml).ToString();

        WriteContentTag(sb, content, style);
    }

    protected void WriteContentTag(TextWriter sb, string? content = "", string? style = null)
    {
        string classAttr = HtmlBuilderTagAttribute.BuildClassAttribute(ClassName);
        string styleAttr = HtmlBuilderTagAttribute.BuildStyleAttribute(style);
        sb.WriteLine($"<{Tag}{classAttr}{styleAttr}>{content}</{Tag}>");
    }
}
