using System.IO;
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

        public override void Cell(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true)
        {
            if (!string.IsNullOrWhiteSpace(innerHtml))
                sb.WriteLine($"<style>\n{innerHtml}\n</style>");
        }
    }
}