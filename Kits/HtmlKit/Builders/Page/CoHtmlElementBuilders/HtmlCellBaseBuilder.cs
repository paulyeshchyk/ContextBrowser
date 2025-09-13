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

        // context: html, build
        public void With(TextWriter writer, Action block)
        {
            block?.Invoke();
        }
    }
}