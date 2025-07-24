using ContextBrowser.HtmlKit.Builders.Core;

namespace ContextBrowser.HtmlKit.Builders.Tag;

// pattern: Builder
public class HtmlBuilderTableCell : HtmlBuilder
{
    public HtmlBuilderTableCell(string className) : base("td", className)
    {
    }
}
