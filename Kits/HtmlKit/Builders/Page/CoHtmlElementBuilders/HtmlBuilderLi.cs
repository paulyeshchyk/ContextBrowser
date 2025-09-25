using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    public class HtmlBuilderLi : HtmlBuilder
    {
        public HtmlBuilderLi() : base("li", string.Empty)
        {
        }
    }
}