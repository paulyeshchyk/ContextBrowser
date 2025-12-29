using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    public class HtmlBuilderUl : HtmlBuilder
    {
        public HtmlBuilderUl() : base("ul", string.Empty)
        {
        }
    }
}