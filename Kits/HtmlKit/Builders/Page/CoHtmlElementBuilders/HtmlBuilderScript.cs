using System.IO;
using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    // Специализированный билдер для <script>
    // context: html, build
    internal class HtmlBuilderScript : HtmlBuilder
    {
        protected override bool isRaw => true;

        public HtmlBuilderScript(string tag) : base(tag, string.Empty)
        {
        }

        public override void Cell(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true)
        {
            var content = !string.IsNullOrWhiteSpace(innerHtml)
                ? innerHtml
                : string.Empty;

            WriteContentTag(sb, attributes, content, isEncodable);
        }
    }
}