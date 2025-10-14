using ContextBrowserKit.Options;

namespace HtmlKit.Helpers;

public interface IHtmlHrefManager<TTensor>
    where TTensor : notnull
{
    string GetHrefCell(TTensor cell, HtmlTableOptions _options);

    string GetHrefColSummary(object key, HtmlTableOptions _options);

    string GetHRefRow(string key, HtmlTableOptions _options);

    string GetHRefRowHeader(object key, HtmlTableOptions _options);

    string GetHrefRowHeaderSummary(HtmlTableOptions _options);

    string GetHRefRowMeta(object key, HtmlTableOptions _options);

    string GetHrefRowSummary(object key, HtmlTableOptions _options);

    string GetHrefSummary(HtmlTableOptions _options);

    string GetHrefColHeaderSummary(HtmlTableOptions _options);

    string GetHrefRowHeaderSummaryAfterFirst(HtmlTableOptions _options);
}
