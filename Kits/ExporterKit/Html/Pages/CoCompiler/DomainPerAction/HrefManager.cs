using System;
using ContextBrowserKit.Options;
using ContextKit.Model;
using HtmlKit.Options;
using HtmlKit.Matrix;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;
using ExporterKit.Html.Containers;

namespace HtmlKit.Helpers;

public class HrefManagerMethodList<TDataTensor> : IHrefManager<MethodListTensor<TDataTensor>>
    where TDataTensor : notnull
{
    public string GetHrefCell(MethodListTensor<TDataTensor> cell, HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHrefColHeaderSummary(HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHrefColSummary(object key, HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHRefRow(string key, HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHRefRowHeader(object key, HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHrefRowHeaderSummary(HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHrefRowHeaderSummaryAfterFirst(HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHRefRowMeta(object key, HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHrefRowSummary(object key, HtmlTableOptions _options)
    {
        return string.Empty;
    }

    public string GetHrefSummary(HtmlTableOptions _options)
    {
        return string.Empty;
    }
}

public class HrefManager<TTensor> : IHrefManager<TTensor>
    where TTensor : IDomainPerActionTensor
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

    public string GetHrefCell(TTensor cell, HtmlTableOptions _options) =>
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