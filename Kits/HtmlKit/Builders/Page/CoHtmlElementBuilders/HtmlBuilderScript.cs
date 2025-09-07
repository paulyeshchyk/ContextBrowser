using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Tag;
using HtmlKit.Classes;

namespace HtmlKit.Page;

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