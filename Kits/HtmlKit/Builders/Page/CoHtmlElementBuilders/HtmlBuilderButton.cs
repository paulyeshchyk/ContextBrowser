using System.IO;
using System.Net;
using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    public class HtmlBuilderButton : HtmlBuilder
    {
        public HtmlBuilderButton(string tag) : base(tag, string.Empty)
        {
        }

        protected override void WriteContentTag(TextWriter sb, IHtmlTagAttributes? attributes, string? content = "", bool isEncodable = true)
        {
            var innerAttrs = new HtmlTagAttributes() { { "onClick", _onClickEvent } };
            innerAttrs.Concat(attributes);

            var attributesString = innerAttrs.ToString();
            var theContent = isEncodable
                ? WebUtility.HtmlEncode(content)
                : string.IsNullOrEmpty(content) ? string.Empty : content;
            sb.WriteLine($"<{Tag} {attributesString}>{theContent}</{Tag}>");
        }
    }
}