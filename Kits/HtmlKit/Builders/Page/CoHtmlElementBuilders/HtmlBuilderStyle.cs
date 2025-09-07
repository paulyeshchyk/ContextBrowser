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