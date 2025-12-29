using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    public class HtmlBuilderTableCellBase : HtmlBuilder
    {
        public HtmlBuilderTableCellBase(string className) : base("td", className)
        {
        }
    }
}