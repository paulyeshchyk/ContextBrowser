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
    private class HtmlBuilderButton : HtmlBuilder
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