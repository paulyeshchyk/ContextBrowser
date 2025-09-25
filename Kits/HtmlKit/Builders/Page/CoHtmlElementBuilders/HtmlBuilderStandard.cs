using System;
using System.IO;
using HtmlKit.Builders.Core;

namespace HtmlKit.Builders.Page.CoHtmlElementBuilders;

public static partial class HtmlBuilderFactory
{
    // context: html, build
    internal class HtmlBuilderStandard : HtmlBuilder
    {
        public HtmlBuilderStandard(string tag, string className) : base(tag, className)
        {
        }

        // Cell метод не всегда применим для произвольных тегов (html, head, body, table)
        public override void Cell(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true)
        {
            throw new NotSupportedException($"Cell method is not supported for <{Tag}> tag in StandardTagBuilder. Use Start/End instead.");
        }
    }
}