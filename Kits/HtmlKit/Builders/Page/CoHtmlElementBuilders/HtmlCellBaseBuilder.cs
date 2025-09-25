using System.IO;
using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    // context: html, build
    public record HtmlCellBaseBuilder : IHtmlCellBuilder
    {
        // context: html, build
        public void Cell(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true)
        {
            if (string.IsNullOrWhiteSpace(innerHtml))
                return;

            sb.WriteLine(innerHtml);
        }
    }
}