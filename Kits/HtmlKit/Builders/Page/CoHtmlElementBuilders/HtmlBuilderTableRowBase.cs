using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    public class HtmlBuilderTableRowBase : HtmlBuilder
    {
        public HtmlBuilderTableRowBase(string className) : base("tr", className)
        {
        }
    }
}