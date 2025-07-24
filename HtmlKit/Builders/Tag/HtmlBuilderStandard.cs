using ContextBrowser.HtmlKit.Builders.Core;

namespace ContextBrowser.HtmlKit.Builders.Tag;

// pattern: Builder
public class HtmlBuilderStandard : IHtmlTagBuilder
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

    public void Start(TextWriter writer, string? className)
    {
        throw new NotImplementedException();
    }
}
