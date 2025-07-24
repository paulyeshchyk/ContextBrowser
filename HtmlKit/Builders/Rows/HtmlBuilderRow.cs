using ContextBrowser.HtmlKit.Builders.Core;
using ContextBrowser.HtmlKit.Classes;

namespace ContextBrowser.HtmlKit.Builders.Rows;

// pattern: Abstract Factory
// pattern note: weak
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
