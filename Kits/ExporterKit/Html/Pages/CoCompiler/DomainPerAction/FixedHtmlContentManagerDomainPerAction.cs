using ContextBrowserKit.Options;
using HtmlKit.Options;

namespace HtmlKit.Helpers;

public class FixedHtmlContentManagerDomainPerAction : IFixedHtmlContentManager
{
    public string TopLeftCell(HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? "Action \\ Domain"
            : "Domain \\ Action";

    public string SummaryRow(HtmlTableOptions _options) => "<b>Σ</b>";

    public string FirstSummaryRow(HtmlTableOptions _options) => "<b>Σ</b>";

    public string LastSummaryRow(HtmlTableOptions _options) => "<b>Σ</b>";
}