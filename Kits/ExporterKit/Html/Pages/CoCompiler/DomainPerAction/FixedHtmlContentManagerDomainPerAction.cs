using ContextBrowserKit.Options;
using HtmlKit.Options;
using TensorKit.Model;

namespace HtmlKit.Helpers;

public class FixedHtmlContentManagerDomainPerAction : IHtmlFixedContentManager
{
    public string TopLeftCell(HtmlTableOptions _options) =>
        _options.Orientation == TensorPermutationType.Standard
            ? "Action \\ Domain"
            : "Domain \\ Action";

    public string SummaryRow(HtmlTableOptions _options) => "<b>Σ</b>";

    public string FirstSummaryRow(HtmlTableOptions _options) => "<b>Σ</b>";

    public string LastSummaryRow(HtmlTableOptions _options) => "<b>Σ</b>";
}