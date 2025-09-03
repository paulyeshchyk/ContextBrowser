using System;
using ContextBrowserKit.Options;
using ContextKit.Model.Matrix;
using HtmlKit.Options;

namespace HtmlKit.Helpers;

internal class HrefManager : IHrefManager
{
    private static readonly long TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public string GetHrefColSummary(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\composite_domain_{key}.html?v={TimeStamp}"
            : $"pages\\composite_action_{key}.html?v={TimeStamp}";

    public string GetHrefRowSummary(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\composite_action_{key}.html?v={TimeStamp}"
            : $"pages\\composite_domain_{key}.html?v={TimeStamp}";

    public string GetHRefRow(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\composite_action_{key}.html?v={TimeStamp}"
            : $"pages\\composite_domain_{key}.html?v={TimeStamp}";

    public string GetHRefRowMeta(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\composite_domain_{key}.html?v={TimeStamp}"
            : $"pages\\composite_action_{key}.html?v={TimeStamp}";

    public string GetHRefRowHeader(string key, HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\composite_action_{key}.html?v={TimeStamp}"
            : $"pages\\composite_domain_{key}.html?v={TimeStamp}";

    public string GetHrefCell(ContextInfoDataCell cell, HtmlTableOptions _options) =>
        $"pages\\composite_{cell.Action}_{cell.Domain}.html?v={TimeStamp}";

    public string GetHrefSummary(HtmlTableOptions _options) =>
        $"pages\\summary.html?v={TimeStamp}";

    public string GetHrefRowHeaderSummary(HtmlTableOptions _options)
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
            SummaryPlacementType.None =>
                string.Empty,
            _ => string.Empty
        };
    }

    public string GetHrefRowHeaderSummaryAfterFirst(HtmlTableOptions _options)
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
            SummaryPlacementType.None =>
                string.Empty,
            _ => string.Empty
        };
    }

    public string GetHrefColHeaderSummary(HtmlTableOptions _options) =>
        _options.Orientation == MatrixOrientationType.ActionRows
            ? $"pages\\domain_summary.html?v={TimeStamp}"
            : $"pages\\action_summary.html?v={TimeStamp}";
}