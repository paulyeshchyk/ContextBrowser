using HtmlKit.Builders.Classes;
using HtmlKit.Builders.Core;
using HtmlKit.Builders.Page.CoHtmlElementBuilders;

namespace HtmlKit.Builders.Page;

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
    public static readonly IHtmlBuilder Html = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderStandard("html", HtmlTagClasses.Page.Html);
    public static readonly IHtmlBuilder Head = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderStandard("head", HtmlTagClasses.Page.Head);
    public static readonly IHtmlBuilder Body = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderStandard("body", HtmlTagClasses.Page.Body);

    // Теги с уникальным поведением
    public static readonly IHtmlCellBuilder Raw = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlCellBaseBuilder();

    public static readonly IHtmlBuilder Title = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderTitle("title", HtmlTagClasses.Page.Title);
    public static readonly IHtmlBuilder Meta = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderMeta("meta");
    public static readonly IHtmlBuilder Script = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderScript("script");
    public static readonly IHtmlBuilder Style = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderStyle("style");

    // Теги общего назначения
    public static readonly IHtmlBuilder Table = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderStandard("table", HtmlTagClasses.Page.Table);
    public static readonly IHtmlBuilder H1 = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderH1();
    public static readonly IHtmlBuilder Button = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderButton("button");
    public static readonly IHtmlBuilder Div = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderDiv();
    public static readonly IHtmlBuilder P = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderP();
    public static readonly IHtmlBuilder Span = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderSpan();
    public static readonly IHtmlBuilder Ul = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderUl();
    public static readonly IHtmlBuilder Li = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderLi();
    public static readonly IHtmlBuilder A = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderAnchor(string.Empty);

    public static HtmlBuilder Breadcrumb(BreadcrumbNavigationItem navitem) => new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderBreadcrumb(navitem);

    public static HtmlBuilder Puml(string content, string server, string? rendererMode = null) => new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderPumlContent(content, server, rendererMode);

    public static HtmlBuilder PumlReference(string src, string server, string? rendererMode = null) => new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderPumlReference(src, server, rendererMode);

    // pattern: Abstract Factory
    // pattern note: weak
    // context: html, build
    public static class HtmlBuilderTableCell
    {
        public static readonly IHtmlBuilder SummaryCaption = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderTableCellBase(HtmlTagClasses.Cell.SummaryCaption);
        public static readonly IHtmlBuilder TotalSummary = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderTableCellBase(HtmlTagClasses.Cell.TotalSummary);
        public static readonly IHtmlBuilder ColSummary = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderTableCellBase(HtmlTagClasses.Cell.ColSummary);
        public static readonly IHtmlBuilder ColMeta = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderTableCellBase(HtmlTagClasses.Cell.ColMeta);
        public static readonly IHtmlBuilder RowMeta = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderTableCellBase(HtmlTagClasses.Cell.RowMeta);
        public static readonly IHtmlBuilder RowSummary = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderTableCellBase(HtmlTagClasses.Cell.RowSummary);
        public static readonly IHtmlBuilder Data = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderTableCellBase(HtmlTagClasses.Cell.Data);
        public static readonly IHtmlBuilder ActionDomain = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderTableCellBase(HtmlTagClasses.Cell.ActionDomain);
    }

    // pattern: Abstract Factory
    // pattern note: weak
    // context: html, build
    public static class HtmlBuilderTableRow
    {
        public static readonly IHtmlBuilder Summary = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderTableRowBase(HtmlTagClasses.Row.Summary);
        public static readonly IHtmlBuilder Data = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderTableRowBase(HtmlTagClasses.Row.Data);
        public static readonly IHtmlBuilder Meta = new CoHtmlElementBuilders.HtmlBuilderFactory.HtmlBuilderTableRowBase(HtmlTagClasses.Row.Meta);
    }
}