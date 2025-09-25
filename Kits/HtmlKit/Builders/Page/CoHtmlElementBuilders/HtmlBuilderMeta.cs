using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    // Специализированный билдер для <meta>
    // context: html, build
    internal class HtmlBuilderMeta : HtmlBuilder
    {
        protected override bool IsClosable => false;

        public HtmlBuilderMeta(string tag) : base(tag, string.Empty)
        {
        }
    }
}