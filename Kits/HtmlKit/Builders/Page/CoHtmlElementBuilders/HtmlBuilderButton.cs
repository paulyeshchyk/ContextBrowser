using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    public class HtmlBuilderButton : HtmlBuilder
    {
        public HtmlBuilderButton(string tag) : base(tag, string.Empty)
        {
        }

        protected override Task WriteContentTagAsync(TextWriter sb, IHtmlTagAttributes? attributes, string? content = "", bool isEncodable = true, CancellationToken cancellationToken = default)
        {
            var innerAttrs = new HtmlTagAttributes() { { "onClick", _onClickEvent } };
            innerAttrs.Concat(attributes);

            var attributesString = innerAttrs.ToString();
            var theContent = isEncodable
                ? WebUtility.HtmlEncode(content)
                : string.IsNullOrEmpty(content) ? string.Empty : content;
            sb.WriteLine($"<{Tag} {attributesString}>{theContent}</{Tag}>");
            return Task.CompletedTask;
        }
    }
}