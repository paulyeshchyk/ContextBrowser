using System.IO;
using System.Net;
using HtmlKit.Builders.Tag;

namespace HtmlKit.Builders.Core;

//pattern: Builder, template method
public abstract class HtmlBuilder : IHtmlBuilder
{
    protected readonly string Tag;
    protected readonly string ClassName;
    protected string _onClickEvent = string.Empty;

    protected virtual bool isRaw => false;

    protected virtual bool IsClosable => true;

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

    public void Start(TextWriter sb, string? className, string? id)
    {
        string classAttr = HtmlBuilderTagAttribute.BuildClassAttribute(className);
        string idAttr = HtmlBuilderTagAttribute.BuildIdAttribute(id);
        if (IsClosable)
        {
            sb.WriteLine($"<{Tag}{classAttr}{idAttr}>");
        }
        else
        {
            sb.WriteLine($"<{Tag}{classAttr}{idAttr}/>");
        }
    }

    public virtual void End(TextWriter sb)
    {
        if (IsClosable)
        {
            sb.WriteLine($"</{Tag}>");
        }
    }

    public virtual void Cell(TextWriter sb, string? innerHtml = "", string? href = null, string? style = null, string className = "")
    {
        var content = string.IsNullOrWhiteSpace(href)
            ? (isRaw ? innerHtml : WebUtility.HtmlEncode(innerHtml))
            : new HtmlBuilderEncodedAnchorSpecial(href, innerHtml).ToString();

        WriteContentTag(sb, content, style, className);
    }

    public virtual HtmlBuilder OnClick(string eventScript)
    {
        _onClickEvent = eventScript;
        return this;
    }

    protected virtual void WriteContentTag(TextWriter sb, string? content = "", string? style = null, string className = "")
    {
        string classAttr = HtmlBuilderTagAttribute.BuildClassAttribute(className);
        string styleAttr = HtmlBuilderTagAttribute.BuildStyleAttribute(style);

        sb.WriteLine($"<{Tag}{classAttr}{styleAttr}>{content}</{Tag}>");
    }
}