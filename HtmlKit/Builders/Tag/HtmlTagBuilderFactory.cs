using ContextBrowser.HtmlKit.Builders.Core;
using ContextBrowser.HtmlKit.Classes;

namespace ContextBrowser.HtmlKit.Builders.Tag;

// pattern: Abstract Factory
// pattern note: weak
public static partial class HtmlTagBuilderFactory
{
    // Общие теги, поведение которых не требует Cell
    public static readonly IHtmlTagBuilder Html = new HtmlBuilderStandard("html", HtmlTagClasses.Page.Html);
    public static readonly IHtmlTagBuilder Head = new HtmlBuilderStandard("head", HtmlTagClasses.Page.Head);
    public static readonly IHtmlTagBuilder Body = new HtmlBuilderStandard("body", HtmlTagClasses.Page.Body);
    public static readonly IHtmlTagBuilder Table = new HtmlBuilderStandard("table", HtmlTagClasses.Page.Table);

    // Теги с контентом — но без ссылки
    public static readonly IHtmlCellBuilder Title = new HtmlBuilderTitle("title", HtmlTagClasses.Page.Title);
    public static readonly IHtmlCellBuilder Script = new HtmlBuilderContent("script");
    public static readonly IHtmlCellBuilder Style = new HtmlBuilderContent("style");
    public static readonly IHtmlCellBuilder H1 = new HtmlBuilderContent("h1", HtmlTagClasses.Page.H1);


    public static readonly IHtmlBuilder SummaryCaption = new HtmlBuilderTableCell(HtmlTagClasses.Cell.SummaryCaption);
    public static readonly IHtmlBuilder TotalSummary = new HtmlBuilderTableCell(HtmlTagClasses.Cell.TotalSummary);
    public static readonly IHtmlBuilder ColSummary = new HtmlBuilderTableCell(HtmlTagClasses.Cell.ColSummary);
    public static readonly IHtmlBuilder ColMeta = new HtmlBuilderTableCell(HtmlTagClasses.Cell.ColMeta);
    public static readonly IHtmlBuilder RowMeta = new HtmlBuilderTableCell(HtmlTagClasses.Cell.RowMeta);
    public static readonly IHtmlBuilder RowSummary = new HtmlBuilderTableCell(HtmlTagClasses.Cell.RowSummary);
    public static readonly IHtmlBuilder Data = new HtmlBuilderTableCell(HtmlTagClasses.Cell.Data);
    public static readonly IHtmlBuilder ActionDomain = new HtmlBuilderTableCell(HtmlTagClasses.Cell.ActionDomain);
}
