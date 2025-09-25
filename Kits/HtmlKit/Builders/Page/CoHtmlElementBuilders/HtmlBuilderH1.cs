using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    // Специализированный билдер для <h1>
    public class HtmlBuilderH1 : HtmlBuilder
    {
        public HtmlBuilderH1() : base("h1", string.Empty)
        {
        }
    }
}

