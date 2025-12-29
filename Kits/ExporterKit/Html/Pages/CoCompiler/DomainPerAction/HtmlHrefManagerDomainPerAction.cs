using System;
using ContextBrowserKit.Options;
using ExporterKit.Html.Containers;
using HtmlKit.Helpers;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

public class HtmlHrefManagerDomainPerAction : IHtmlHrefManager<DomainPerActionTensor>
{
    private static readonly long TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public string GetHrefColSummary(object key, HtmlTableOptions _options) =>
        _options.Orientation == TensorPermutationType.Standard
            ? $"pages\\composite_domain_{key}.html?v={TimeStamp}"
            : $"pages\\composite_action_{key}.html?v={TimeStamp}";

    public string GetHrefRowSummary(object key, HtmlTableOptions _options) =>
        _options.Orientation == TensorPermutationType.Standard
            ? $"pages\\composite_action_{key}.html?v={TimeStamp}"
            : $"pages\\composite_domain_{key}.html?v={TimeStamp}";

    public string GetHRefRow(string key, HtmlTableOptions _options) =>
        _options.Orientation == TensorPermutationType.Standard
            ? $"pages\\composite_action_{key}.html?v={TimeStamp}"
            : $"pages\\composite_domain_{key}.html?v={TimeStamp}";

    public string GetHRefRowMeta(object key, HtmlTableOptions _options) =>
        _options.Orientation == TensorPermutationType.Standard
            ? $"pages\\composite_domain_{key}.html?v={TimeStamp}"
            : $"pages\\composite_action_{key}.html?v={TimeStamp}";

    public string GetHRefRowHeader(object key, HtmlTableOptions _options) =>
        _options.Orientation == TensorPermutationType.Standard
            ? $"pages\\composite_action_{key}.html?v={TimeStamp}"
            : $"pages\\composite_domain_{key}.html?v={TimeStamp}";

    public string GetHrefCell(DomainPerActionTensor cell, HtmlTableOptions _options) =>
        $"pages\\composite_{cell.Action}_{cell.Domain}.html?v={TimeStamp}";

    public string GetHrefSummary(HtmlTableOptions _options) =>
        $"pages\\summary.html?v={TimeStamp}";

    public string GetHrefRowHeaderSummary(HtmlTableOptions _options)
    {
        return _options.SummaryPlacement switch
        {
            SummaryPlacementType.AfterFirst =>
                _options.Orientation == TensorPermutationType.Standard
                ? $"pages\\domain_summary.html?v={TimeStamp}"
                : $"pages\\action_summary.html?v={TimeStamp}",
            SummaryPlacementType.AfterLast =>
                _options.Orientation == TensorPermutationType.Standard
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
                _options.Orientation == TensorPermutationType.Standard
                ? $"pages\\action_summary.html?v={TimeStamp}"
                : $"pages\\domain_summary.html?v={TimeStamp}",
            SummaryPlacementType.AfterLast =>
                _options.Orientation == TensorPermutationType.Standard
                ? $"pages\\action_summary.html?v={TimeStamp}"
                : $"pages\\domain_summary.html?v={TimeStamp}",
            SummaryPlacementType.None =>
                string.Empty,
            _ => string.Empty
        };
    }

    public string GetHrefColHeaderSummary(HtmlTableOptions _options) =>
        _options.Orientation == TensorPermutationType.Standard
            ? $"pages\\domain_summary.html?v={TimeStamp}"
            : $"pages\\action_summary.html?v={TimeStamp}";
}