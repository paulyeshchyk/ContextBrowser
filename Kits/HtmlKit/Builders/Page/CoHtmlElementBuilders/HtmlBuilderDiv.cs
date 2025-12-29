using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    public class HtmlBuilderDiv : HtmlBuilder
    {
        public HtmlBuilderDiv() : base("div", string.Empty)
        {
        }
    }
}