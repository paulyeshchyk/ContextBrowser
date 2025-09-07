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
    // Специализированный билдер для <render-plantuml>
    public class HtmlBuilderPuml : HtmlBuilder
    {
        private const string SRenderPlantumlJs = "<script type=\"module\">import enableElement from \"./../render-plantuml.js\";enableElement({{serverURL: \"{0}\"}});</script>";

        public string RendererMode { get; set; }
        public string Server { get; set; }
        public string Content { get; set; }

        public HtmlBuilderPuml(string content, string server, string? rendererMode = null) : base("render-plantuml", string.Empty)
        {
            Content = content;
            Server = server;
            RendererMode = string.IsNullOrEmpty(rendererMode) ? "svg" : rendererMode;
        }

        public override void Start(TextWriter sb, IHtmlTagAttributes? attrs = null)
        {
            sb.Write(string.Format(SRenderPlantumlJs, Server));
        }

        public override void Cell(TextWriter sb, IHtmlTagAttributes? attributes = null, string? innerHtml = "", bool isEncodable = true)
        {
            var rendererAttributes = new HtmlTagAttributes() { { "rendererMode", RendererMode }, { "server", Server } };

            WriteContentTag(sb, rendererAttributes, Content, isEncodable: false);
        }

        public override void End(TextWriter sb)
        {
            // nothing to do and noting to override
        }
    }
}