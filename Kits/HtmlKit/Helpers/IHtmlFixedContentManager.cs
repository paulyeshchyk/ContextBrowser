using ContextBrowserKit.Options;

namespace HtmlKit.Helpers;

public interface IHtmlFixedContentManager
{
    string FirstSummaryRow(HtmlTableOptions _options);

    string LastSummaryRow(HtmlTableOptions _options);

    string SummaryRow(HtmlTableOptions _options);

    string TopLeftCell(HtmlTableOptions _options);
}
