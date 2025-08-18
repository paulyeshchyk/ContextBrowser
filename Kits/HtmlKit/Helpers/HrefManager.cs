using ContextBrowserKit.Options;
using ContextKit.Model.Matrix;
using HtmlKit.Options;

namespace HtmlKit.Helpers;

internal static class HrefManager
{
    private static long TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public static string GetHrefColSummary(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\sequence_domain_{key}.html?v={TimeStamp}"
            : $"pages\\action_{key}.html?v={TimeStamp}";

    public static string GetHrefRowSummary(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\action_{key}.html?v={TimeStamp}"
            : $"pages\\sequence_domain_{key}.html?v={TimeStamp}";

    public static string GetHRefRow(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\action_{key}.html?v={TimeStamp}"
            : $"pages\\sequence_domain_{key}.html?v={TimeStamp}";

    public static string GetHRefRowMeta(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\sequence_domain_{key}.html?v={TimeStamp}"
            : $"pages\\action_{key}.html?v={TimeStamp}";

    public static string GetHRefRowHeader(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\action_{key}.html?v={TimeStamp}"
            : $"pages\\sequence_domain_{key}.html?v={TimeStamp}";

    public static string GetHrefCell(ContextInfoMatrixCell cell, HtmlTableOptions _options) =>
        $"pages\\composite_{cell.Action}_{cell.Domain}.html?v={TimeStamp}";

    public static string GetHrefSummary(HtmlTableOptions _options) =>
        $"index.html?v={TimeStamp}";

    public static string GetHrefRowHeaderSummary(HtmlTableOptions _options)
    {
        return _options.SummaryPlacement switch
        {
            SummaryPlacementType.AfterFirst =>
            _options.Orientation == MatrixOrientationType.ActionRows
                ? $"pages\\domain_summary.html?v={TimeStamp}"
                : $"pages\\action_summary.html?v={TimeStamp}",
            SummaryPlacementType.AfterLast =>
            _options.Orientation == MatrixOrientationType.ActionRows
                ? $"pages\\domain_summary.html?v={TimeStamp}"
                : $"pages\\action_summary.html?v={TimeStamp}",
            _ => string.Empty
        };
    }

    public static string GetHrefRowHeaderSummaryAfterFirst(HtmlTableOptions _options)
    {
        return _options.SummaryPlacement switch
        {
            SummaryPlacementType.AfterFirst =>
            _options.Orientation == MatrixOrientationType.ActionRows
                ? $"pages\\action_summary.html?v={TimeStamp}"
                : $"pages\\domain_summary.html?v={TimeStamp}",
            SummaryPlacementType.AfterLast =>
            _options.Orientation == MatrixOrientationType.ActionRows
                ? $"pages\\action_summary.html?v={TimeStamp}"
                : $"pages\\domain_summary.html?v={TimeStamp}",
            _ => string.Empty
        };
    }

    public static string GetHrefColHeaderSummary(HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\domain_summary.html?v={TimeStamp}"
            : $"pages\\action_summary.html?v={TimeStamp}";
}