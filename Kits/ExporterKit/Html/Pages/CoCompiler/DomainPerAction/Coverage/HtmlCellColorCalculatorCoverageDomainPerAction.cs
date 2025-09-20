using System.Collections.Generic;
using System.Data;
using System.Linq;
using ContextKit.Model;
using ExporterKit.Html.Pages.CoCompiler.DomainPerAction.Coverage;
using HtmlKit;
using HtmlKit.Document;
using HtmlKit.Document.Coverage;
using HtmlKit.Helpers;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction.Coverage;

public class HtmlCellColorCalculatorCoverageDomainPerAction : IHtmlCellColorCalculator<DomainPerActionTensor>
{
    private readonly ICoverageValueExtractor _coverageValueExtractor;

    public HtmlCellColorCalculatorCoverageDomainPerAction(ICoverageValueExtractor coverageValueExtractor)
    {
        _coverageValueExtractor = coverageValueExtractor;
    }

    public string? CalculateBgColor(DomainPerActionTensor cell, IEnumerable<ContextInfo>? contextInfoList, Dictionary<string, ContextInfo>? index)
    {
        if (contextInfoList != null && contextInfoList.Any() && index != null)
        {
            var covs = contextInfoList
                .Select(method => index.TryGetValue(method.FullName, out var ctx)
                    ? _coverageValueExtractor.GetValue(ctx)
                    : 0)
                .ToList();

            if (covs.Any())
                return HeatmapColorBuilder.ToHeatmapColor(covs.Average());
        }

        if (index?.TryGetValue(cell.Action, out var actionCtx) ?? false)
        {
            var aVal = _coverageValueExtractor.GetValue(actionCtx);
            return HeatmapColorBuilder.ToHeatmapColor(aVal);
        }

        if (index?.TryGetValue(cell.Domain, out var domainCtx) ?? false)
        {
            var aVal = _coverageValueExtractor.GetValue(domainCtx);
            return HeatmapColorBuilder.ToHeatmapColor(aVal);
        }

        return null;
    }
}
