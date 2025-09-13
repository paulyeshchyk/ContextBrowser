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
    public abstract class HtmlBuilderPumlBase : HtmlBuilder
    {
        private const string SRenderPlantumlJs = "<script type=\"module\">import enableElement from \"./../render-plantuml.js\";enableElement();</script>";

        public string RendererMode { get; set; }

        public string Server { get; set; }

        protected HtmlBuilderPumlBase(string server, string? rendererMode = null)
            : base("render-plantuml", string.Empty)
        {
            Server = server;
            RendererMode = string.IsNullOrEmpty(rendererMode) ? "svg" : rendererMode;
        }

        public override void Start(TextWriter sb, IHtmlTagAttributes? attrs = null)
        {
            sb.Write(SRenderPlantumlJs);
        }

        public override void End(TextWriter sb)
        {
            // nothing to do
        }
    }

    public class HtmlBuilderPumlContent : HtmlBuilderPumlBase
    {
        public string Content { get; set; }

        public HtmlBuilderPumlContent(string content, string server, string? rendererMode = null)
            : base(server, rendererMode)
        {
            Content = content;
        }

        public override void Cell(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true)
        {
            var rendererAttributes = new HtmlTagAttributes
        {
            { "rendererMode", RendererMode },
            { "server", Server }
        };

            WriteContentTag(sb, rendererAttributes, Content, isEncodable: false);
        }
    }

    public class HtmlBuilderPumlReference : HtmlBuilderPumlBase
    {
        public string Src { get; set; }

        public HtmlBuilderPumlReference(string src, string server, string? rendererMode = null)
            : base(server, rendererMode)
        {
            Src = src;
        }

        public override void Cell(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true)
        {
            var rendererAttributes = new HtmlTagAttributes
            {
                { "rendererMode", RendererMode },
                { "server", Server },
                { "src", Src }
            };

            WriteContentTag(sb, rendererAttributes, string.Empty);
        }
    }
}