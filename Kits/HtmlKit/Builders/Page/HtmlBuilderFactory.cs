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

// pattern: Builder
// pattern note: weak
// context: html, build
public static class HtmlBuilderFactory
{
    public static class JsScripts
    {
        public static readonly string JsShowTabsheetTabScript = Resources.JsShowTabseetTabScript;
    }

    public static class CssStyles
    {
        public static readonly string CssTabsheet = Resources.CssTabsheetTabs;
    }

    // Общие теги, поведение которых не сильно отличается от базового HtmlBuilderBase
    public static readonly IHtmlBuilder Html = new HtmlBuilderStandard("html", HtmlTagClasses.Page.Html);

    public static readonly IHtmlBuilder Head = new HtmlBuilderStandard("head", HtmlTagClasses.Page.Head);
    public static readonly IHtmlBuilder Body = new HtmlBuilderStandard("body", HtmlTagClasses.Page.Body);
    public static readonly IHtmlBuilder Table = new HtmlBuilderStandard("table", HtmlTagClasses.Page.Table);

    // Теги с уникальным поведением
    public static readonly IHtmlBuilder Title = new HtmlBuilderTitle("title", HtmlTagClasses.Page.Title);

    public static readonly IHtmlBuilder Meta = new HtmlBuilderMeta("meta");
    public static readonly IHtmlBuilder Script = new HtmlBuilderScript("script");
    public static readonly IHtmlBuilder Style = new HtmlBuilderStyle("style");
    public static readonly IHtmlBuilder H1 = new HtmlBuilderH1("h1");
    public static readonly IHtmlBuilder Button = new HtmlBuilderButton("button");
    public static readonly IHtmlBuilder Div = new HtmlBuilderP("div");
    public static readonly IHtmlBuilder P = new HtmlBuilderP("p");
    public static readonly IHtmlBuilder Ul = new HtmlBuilderUl("ul");
    public static readonly IHtmlBuilder Li = new HtmlBuilderLi("li");
    public static readonly IHtmlBuilder A = new HtmlBuilderAnchor(string.Empty);
    public static readonly IHtmlCellBuilder Raw = new RawBuilder();
    public static HtmlBuilder Puml(string content, string server, string? rendererMode = null) => new PumlHtmlBuilder(content, server, rendererMode);

    // pattern: Abstract Factory
    // pattern note: weak
    // context: html, build
    public static class HtmlBuilderTableCell
    {
        public static readonly IHtmlBuilder SummaryCaption = new CellBuilder(HtmlTagClasses.Cell.SummaryCaption);
        public static readonly IHtmlBuilder TotalSummary = new CellBuilder(HtmlTagClasses.Cell.TotalSummary);
        public static readonly IHtmlBuilder ColSummary = new CellBuilder(HtmlTagClasses.Cell.ColSummary);
        public static readonly IHtmlBuilder ColMeta = new CellBuilder(HtmlTagClasses.Cell.ColMeta);
        public static readonly IHtmlBuilder RowMeta = new CellBuilder(HtmlTagClasses.Cell.RowMeta);
        public static readonly IHtmlBuilder RowSummary = new CellBuilder(HtmlTagClasses.Cell.RowSummary);
        public static readonly IHtmlBuilder Data = new CellBuilder(HtmlTagClasses.Cell.Data);
        public static readonly IHtmlBuilder ActionDomain = new CellBuilder(HtmlTagClasses.Cell.ActionDomain);
    }

    // pattern: Abstract Factory
    // pattern note: weak
    // context: html, build
    public static class HtmlBuilderTableRow
    {
        public static readonly IHtmlBuilder Summary = new RowBuilder(HtmlTagClasses.Row.Summary);
        public static readonly IHtmlBuilder Data = new RowBuilder(HtmlTagClasses.Row.Data);
        public static readonly IHtmlBuilder Meta = new RowBuilder(HtmlTagClasses.Row.Meta);
    }

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

    // Специализированный билдер для <title>
    // context: html, build
    internal class HtmlBuilderTitle : HtmlBuilder
    {
        public HtmlBuilderTitle(string tag, string className) : base(tag, className)
        {
        }

    }

    // Специализированный билдер для <meta>
    // context: html, build
    internal class HtmlBuilderMeta : HtmlBuilder
    {
        protected override bool IsClosable => false;

        public HtmlBuilderMeta(string tag) : base(tag, string.Empty)
        {
        }
    }

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

    // Специализированный билдер для <render-plantuml>
    public class PumlHtmlBuilder : HtmlBuilder
    {
        private const string SRenderPlantumlJs = "<script type=\"module\">import enableElement from \"./../render-plantuml.js\";enableElement({{serverURL: \"{0}\"}});</script>";

        public string RendererMode { get; set; }
        public string Server { get; set; }
        public string Content { get; set; }

        public PumlHtmlBuilder(string content, string server, string? rendererMode = null) : base("render-plantuml", string.Empty)
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

    // Специализированный билдер для <h1>
    private class HtmlBuilderH1 : HtmlBuilder
    {
        public HtmlBuilderH1(string tag) : base(tag, string.Empty)
        {
        }
    }

    private class HtmlBuilderP : HtmlBuilder
    {
        public HtmlBuilderP(string tag) : base(tag, string.Empty)
        {
        }
    }

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

    private class HtmlBuilderDiv : HtmlBuilder
    {
        public HtmlBuilderDiv(string tag) : base(tag, string.Empty)
        {
        }
    }

    private class HtmlBuilderUl : HtmlBuilder
    {
        public HtmlBuilderUl(string tag) : base(tag, string.Empty)
        {
        }
    }

    private class HtmlBuilderAnchor : HtmlBuilder
    {
        public HtmlBuilderAnchor(string className) : base("a", className)
        {
        }
    }

    private class HtmlBuilderLi : HtmlBuilder
    {
        public HtmlBuilderLi(string tag) : base(tag, string.Empty)
        {
        }
    }

    private class CellBuilder : HtmlBuilder
    {
        public CellBuilder(string className) : base("td", className)
        {
        }
    }

    private class RowBuilder : HtmlBuilder
    {
        public RowBuilder(string className) : base("tr", className)
        {
        }
    }

    // context: html, build
    public readonly struct RawBuilder : IHtmlCellBuilder
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