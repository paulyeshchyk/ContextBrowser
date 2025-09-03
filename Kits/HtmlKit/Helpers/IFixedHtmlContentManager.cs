using ContextBrowserKit.Options;
using HtmlKit.Options;

namespace HtmlKit.Helpers;

public interface IFixedHtmlContentManager
{
    string FirstSummaryRow(HtmlTableOptions _options);

    string LastSummaryRow(HtmlTableOptions _options);

    string SummaryRow(HtmlTableOptions _options);

    string TopLeftCell(HtmlTableOptions _options);
}
