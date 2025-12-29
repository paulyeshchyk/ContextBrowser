using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

        public override async Task CellAsync(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true, CancellationToken cancellationToken = default)
        {
            var content = !string.IsNullOrWhiteSpace(innerHtml)
                ? innerHtml
                : string.Empty;

            await WriteContentTagAsync(sb, attributes, content, isEncodable, cancellationToken).ConfigureAwait(false);
        }
    }
}