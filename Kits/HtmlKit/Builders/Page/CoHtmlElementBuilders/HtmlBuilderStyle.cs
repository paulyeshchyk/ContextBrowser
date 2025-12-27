using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    // Специализированный билдер для <style>
    // context: html, build
    internal class HtmlBuilderStyle : HtmlBuilder
    {
        public HtmlBuilderStyle(string tag) : base(tag, string.Empty)
        {
        }

        public override Task CellAsync(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(innerHtml))
                sb.WriteLine($"<style>\n{innerHtml}\n</style>");
            return Task.CompletedTask;
        }
    }
}