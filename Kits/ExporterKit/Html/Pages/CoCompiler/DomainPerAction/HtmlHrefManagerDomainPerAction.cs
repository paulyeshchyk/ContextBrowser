using System;
using ContextBrowserKit.Options;
using ContextKit.ContextData.Naming;
using ContextKit.Model;
using HtmlKit.Helpers;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

public class HtmlHrefManagerDomainPerAction : IHtmlHrefManager<DomainPerActionTensor>
{
    private static readonly long TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public string GetHrefColSummary(ILabeledValue key, HtmlTableOptions _options, INamingProcessor namingProcessor)
    {
        var page = _options.Orientation == TensorPermutationType.Standard
            ? namingProcessor.CompositeDomainPageLink(key.LabeledData.ToString())
            : namingProcessor.CompositeActionPageLink(key.LabeledData.ToString());
        return $"{page}?v={TimeStamp}";
    }

    public string GetHrefRowSummary(ILabeledValue key, HtmlTableOptions _options, INamingProcessor namingProcessor)
    {
        var page = _options.Orientation == TensorPermutationType.Standard
            ? namingProcessor.CompositeActionPageLink(key.LabeledData.ToString())
            : namingProcessor.CompositeDomainPageLink(key.LabeledData.ToString());
        return $"{page}?v={TimeStamp}";
    }

    public string GetHRefRow(ILabeledValue key, HtmlTableOptions _options, INamingProcessor namingProcessor)
    {
        var page = _options.Orientation == TensorPermutationType.Standard
            ? namingProcessor.CompositeActionPageLink(key.LabeledData.ToString())
            : namingProcessor.CompositeDomainPageLink(key.LabeledData.ToString());
        return $"{page}?v={TimeStamp}";
    }

    public string GetHRefRowMeta(ILabeledValue key, HtmlTableOptions _options, INamingProcessor namingProcessor)
    {
        var page = _options.Orientation == TensorPermutationType.Standard
            ? namingProcessor.CompositeDomainPageLink(key.LabeledData.ToString())
            : namingProcessor.CompositeActionPageLink(key.LabeledData.ToString());
        return $"{page}?v={TimeStamp}";
    }

    public string GetHRefRowHeader(ILabeledValue key, HtmlTableOptions _options, INamingProcessor namingProcessor)
    {
        var page = _options.Orientation == TensorPermutationType.Standard
            ? namingProcessor.CompositeActionPageLink(key.LabeledData.ToString())
            : namingProcessor.CompositeDomainPageLink(key.LabeledData.ToString());
        return $"{page}?v={TimeStamp}";
    }

    public string GetHrefCell(DomainPerActionTensor cell, HtmlTableOptions _options, INamingProcessor namingProcessor)
    {
        var page = namingProcessor.CompositeActionDomainPageLink(cell.Action, cell.Domain);
        return $"{page}?v={TimeStamp}";
    }

    public string GetHrefSummary(HtmlTableOptions _options, INamingProcessor namingProcessor) =>
        $"pages\\summary.html?v={TimeStamp}";

    public string GetHrefRowHeaderSummary(HtmlTableOptions _options, INamingProcessor namingProcessor)
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

    public string GetHrefRowHeaderSummaryAfterFirst(HtmlTableOptions _options, INamingProcessor namingProcessor)
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

    public string GetHrefColHeaderSummary(HtmlTableOptions _options, INamingProcessor namingProcessor) =>
        _options.Orientation == TensorPermutationType.Standard
            ? $"pages\\domain_summary.html?v={TimeStamp}"
            : $"pages\\action_summary.html?v={TimeStamp}";
}