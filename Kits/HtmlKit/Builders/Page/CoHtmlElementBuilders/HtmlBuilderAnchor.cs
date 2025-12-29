using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    public class HtmlBuilderAnchor : HtmlBuilder
    {
        public HtmlBuilderAnchor(string className) : base("a", className)
        {
        }
    }
}