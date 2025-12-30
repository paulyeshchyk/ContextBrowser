using ContextBrowserKit.Options;
using ContextKit.Model;

namespace HtmlKit.Helpers;

public interface IHtmlHrefManager<TTensor>
    where TTensor : notnull
{
    string GetHrefCell(TTensor cell, HtmlTableOptions _options);

    string GetHrefColSummary(ILabeledValue key, HtmlTableOptions _options);

    string GetHRefRow(ILabeledValue key, HtmlTableOptions _options);

    string GetHRefRowHeader(ILabeledValue key, HtmlTableOptions _options);

    string GetHrefRowHeaderSummary(HtmlTableOptions _options);

    string GetHRefRowMeta(ILabeledValue key, HtmlTableOptions _options);

    string GetHrefRowSummary(ILabeledValue key, HtmlTableOptions _options);

    string GetHrefSummary(HtmlTableOptions _options);

    string GetHrefColHeaderSummary(HtmlTableOptions _options);

    string GetHrefRowHeaderSummaryAfterFirst(HtmlTableOptions _options);
}
