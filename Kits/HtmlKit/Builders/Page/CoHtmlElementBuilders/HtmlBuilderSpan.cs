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
    // Специализированный билдер для <h1>
    private class HtmlBuilderSpan : HtmlBuilder
    {
        public HtmlBuilderSpan() : base("span", string.Empty)
        {
        }
    }
}