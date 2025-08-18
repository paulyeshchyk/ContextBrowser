using ContextBrowserKit.Options;
using HtmlKit.Options;

namespace HtmlKit.Helpers;

internal static class FixedDataManager
{
    public static string TopLeftCell(HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? "Action \\ Domain"
            : "Domain \\ Action";

    public static string SummaryRow(HtmlTableOptions _options) => "<b>Σ</b>";

    public static string FirstSummaryRow(HtmlTableOptions _options) => "<b>Σ</b>";

    public static string LastSummaryRow(HtmlTableOptions _options) => "<b>Σ</b>";
}