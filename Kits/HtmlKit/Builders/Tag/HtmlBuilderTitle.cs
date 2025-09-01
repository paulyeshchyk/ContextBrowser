using System.IO;
using System.Net;
using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Tag;

// pattern: Builder
public class HtmlBuilderTitle : HtmlBuilder, IHtmlCellBuilder
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

    public override void Cell(TextWriter sb, string? innerHtml = "", string? href = null, string? style = null, string className = "")
    {
        WriteContentTag(sb, WebUtility.HtmlEncode(innerHtml), style, className);
    }
}