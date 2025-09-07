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
    // Специализированный билдер для <meta>
    // context: html, build
    internal class HtmlBuilderMeta : HtmlBuilder
    {
        protected override bool IsClosable => false;

        public HtmlBuilderMeta(string tag) : base(tag, string.Empty)
        {
        }
    }
}