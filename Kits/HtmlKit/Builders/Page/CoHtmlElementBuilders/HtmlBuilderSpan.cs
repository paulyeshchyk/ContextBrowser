using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    // Специализированный билдер для <h1>
    public class HtmlBuilderSpan : HtmlBuilder
    {
        public HtmlBuilderSpan() : base("span", string.Empty)
        {
        }
    }
}