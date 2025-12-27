using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    // context: html, build
    public record HtmlCellBaseBuilder : IHtmlCellBuilder
    {
        // context: html, build
        public Task CellAsync(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(innerHtml))
                return Task.CompletedTask;

            sb.WriteLine(innerHtml);
            return Task.CompletedTask;
        }
    }
}