using ContextBrowserKit.Options;
using ContextKit.Model;
using HtmlKit.Model;

namespace HtmlKit.Helpers;

internal static class HrefManager
{
    public static string GetHrefColSummary(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientation.ActionRows
            ? $"sequence_domain_{key}.html"
            : $"action_{key}.html";

    public static string GetHrefRowSummary(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{key}.html"
            : $"sequence_domain_{key}.html";

    public static string GetHRefRow(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{key}.html"
            : $"sequence_domain_{key}.html";

    public static string GetHRefRowMeta(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientation.ActionRows
            ? $"sequence_domain_{key}.html"
            : $"action_{key}.html";

    public static string GetHRefRowHeader(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientation.ActionRows
            ? $"action_{key}.html"
            : $"sequence_domain_{key}.html";

    public static string GetHrefCell(ContextContainer cell, HtmlTableOptions _options) =>
        $"composite_{cell.Action}_{cell.Domain}.html";

    public static string GetHrefSummary(HtmlTableOptions _options) =>
        $"index.html";

    public static string GetHrefRowHeaderSummary(HtmlTableOptions _options)
    {
        return _options.SummaryPlacement switch
        {
            SummaryPlacement.AfterFirst =>
            _options.Orientation == MatrixOrientation.ActionRows
                ? "domain_summary.html"
                : "action_summary.html",
            SummaryPlacement.AfterLast =>
            _options.Orientation == MatrixOrientation.ActionRows
                ? "domain_summary.html"
                : "action_summary.html",
            _ => string.Empty
        };
    }

    public static string GetHrefRowHeaderSummaryAfterFirst(HtmlTableOptions _options)
    {
        return _options.SummaryPlacement switch
        {
            SummaryPlacement.AfterFirst =>
            _options.Orientation == MatrixOrientation.ActionRows
                ? "action_summary.html"
                : "domain_summary.html",
            SummaryPlacement.AfterLast =>
            _options.Orientation == MatrixOrientation.ActionRows
                ? "action_summary.html"
                : "domain_summary.html",
            _ => string.Empty
        };
    }

    public static string GetHrefColHeaderSummary(HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientation.ActionRows
            ? "domain_summary.html"
            : "action_summary.html";
}