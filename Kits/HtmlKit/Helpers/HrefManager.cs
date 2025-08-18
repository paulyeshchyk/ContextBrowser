using ContextBrowserKit.Options;
using ContextKit.Model;
using HtmlKit.Options;

namespace HtmlKit.Helpers;

internal static class HrefManager
{
    public static string GetHrefColSummary(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\sequence_domain_{key}.html"
            : $"pages\\action_{key}.html";

    public static string GetHrefRowSummary(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\action_{key}.html"
            : $"pages\\sequence_domain_{key}.html";

    public static string GetHRefRow(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\action_{key}.html"
            : $"pages\\sequence_domain_{key}.html";

    public static string GetHRefRowMeta(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\sequence_domain_{key}.html"
            : $"pages\\action_{key}.html";

    public static string GetHRefRowHeader(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\action_{key}.html"
            : $"pages\\sequence_domain_{key}.html";

    public static string GetHrefCell(ContextContainer cell, HtmlTableOptions _options) =>
        $"pages\\composite_{cell.Action}_{cell.Domain}.html";

    public static string GetHrefSummary(HtmlTableOptions _options) =>
        $"index.html";

    public static string GetHrefRowHeaderSummary(HtmlTableOptions _options)
    {
        return _options.SummaryPlacement switch
        {
            SummaryPlacementType.AfterFirst =>
            _options.Orientation == MatrixOrientationType.ActionRows
                ? "pages\\domain_summary.html"
                : "pages\\action_summary.html",
            SummaryPlacementType.AfterLast =>
            _options.Orientation == MatrixOrientationType.ActionRows
                ? "pages\\domain_summary.html"
                : "pages\\action_summary.html",
            _ => string.Empty
        };
    }

    public static string GetHrefRowHeaderSummaryAfterFirst(HtmlTableOptions _options)
    {
        return _options.SummaryPlacement switch
        {
            SummaryPlacementType.AfterFirst =>
            _options.Orientation == MatrixOrientationType.ActionRows
                ? "pages\\action_summary.html"
                : "pages\\domain_summary.html",
            SummaryPlacementType.AfterLast =>
            _options.Orientation == MatrixOrientationType.ActionRows
                ? "pages\\action_summary.html"
                : "pages\\domain_summary.html",
            _ => string.Empty
        };
    }

    public static string GetHrefColHeaderSummary(HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? "pages\\domain_summary.html"
            : "pages\\action_summary.html";
}