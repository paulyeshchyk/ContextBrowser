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
public static partial class HtmlBuilderFactory
{
    public static class JsScripts
    {
        public static readonly string JsShowTabsheetTabScript = Resources.JsShowTabseetTabScript;
    }

    public static class CssStyles
    {
        public static readonly string CssBase = Resources.HtmlProducerContentStyle;
        public static readonly string CssTabsheet = Resources.CssTabsheetTabs;
    }

    // Общие теги, поведение которых не сильно отличается от базового HtmlBuilderBase
    public static readonly IHtmlBuilder Html = new HtmlBuilderStandard("html", HtmlTagClasses.Page.Html);
    public static readonly IHtmlBuilder Head = new HtmlBuilderStandard("head", HtmlTagClasses.Page.Head);
    public static readonly IHtmlBuilder Body = new HtmlBuilderStandard("body", HtmlTagClasses.Page.Body);

    // Теги с уникальным поведением
    public static readonly IHtmlCellBuilder Raw = new HtmlCellBaseBuilder();

    public static readonly IHtmlBuilder Title = new HtmlBuilderTitle("title", HtmlTagClasses.Page.Title);
    public static readonly IHtmlBuilder Meta = new HtmlBuilderMeta("meta");
    public static readonly IHtmlBuilder Script = new HtmlBuilderScript("script");
    public static readonly IHtmlBuilder Style = new HtmlBuilderStyle("style");

    // Теги общего назначения
    public static readonly IHtmlBuilder Table = new HtmlBuilderStandard("table", HtmlTagClasses.Page.Table);
    public static readonly IHtmlBuilder H1 = new HtmlBuilderH1();
    public static readonly IHtmlBuilder Button = new HtmlBuilderButton("button");
    public static readonly IHtmlBuilder Div = new HtmlBuilderDiv();
    public static readonly IHtmlBuilder P = new HtmlBuilderP();
    public static readonly IHtmlBuilder Span = new HtmlBuilderSpan();
    public static readonly IHtmlBuilder Ul = new HtmlBuilderUl();
    public static readonly IHtmlBuilder Li = new HtmlBuilderLi();
    public static readonly IHtmlBuilder A = new HtmlBuilderAnchor(string.Empty);

    public static HtmlBuilder Breadcrumb(BreadcrumbNavigationItem navitem) => new HtmlBuilderBreadcrumb(navitem);

    public static HtmlBuilder Puml(string content, string server, string? rendererMode = null) => new HtmlBuilderPumlContent(content, server, rendererMode);

    public static HtmlBuilder PumlReference(string src, string server, string? rendererMode = null) => new HtmlBuilderPumlReference(src, server, rendererMode);

    // pattern: Abstract Factory
    // pattern note: weak
    // context: html, build
    public static class HtmlBuilderTableCell
    {
        public static readonly IHtmlBuilder SummaryCaption = new HtmlBuilderTableCellBase(HtmlTagClasses.Cell.SummaryCaption);
        public static readonly IHtmlBuilder TotalSummary = new HtmlBuilderTableCellBase(HtmlTagClasses.Cell.TotalSummary);
        public static readonly IHtmlBuilder ColSummary = new HtmlBuilderTableCellBase(HtmlTagClasses.Cell.ColSummary);
        public static readonly IHtmlBuilder ColMeta = new HtmlBuilderTableCellBase(HtmlTagClasses.Cell.ColMeta);
        public static readonly IHtmlBuilder RowMeta = new HtmlBuilderTableCellBase(HtmlTagClasses.Cell.RowMeta);
        public static readonly IHtmlBuilder RowSummary = new HtmlBuilderTableCellBase(HtmlTagClasses.Cell.RowSummary);
        public static readonly IHtmlBuilder Data = new HtmlBuilderTableCellBase(HtmlTagClasses.Cell.Data);
        public static readonly IHtmlBuilder ActionDomain = new HtmlBuilderTableCellBase(HtmlTagClasses.Cell.ActionDomain);
    }

    // pattern: Abstract Factory
    // pattern note: weak
    // context: html, build
    public static class HtmlBuilderTableRow
    {
        public static readonly IHtmlBuilder Summary = new HtmlBuilderTableRowBase(HtmlTagClasses.Row.Summary);
        public static readonly IHtmlBuilder Data = new HtmlBuilderTableRowBase(HtmlTagClasses.Row.Data);
        public static readonly IHtmlBuilder Meta = new HtmlBuilderTableRowBase(HtmlTagClasses.Row.Meta);
    }
}