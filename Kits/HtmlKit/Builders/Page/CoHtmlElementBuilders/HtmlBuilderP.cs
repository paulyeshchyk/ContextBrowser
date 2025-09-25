using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    public class HtmlBuilderP : HtmlBuilder
    {
        public HtmlBuilderP() : base("p", string.Empty)
        {
        }
    }
}