using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ContextKit.Model;
using ContextKit.Model.Collector;
using HtmlKit.Helpers;
using HtmlKit.Model;

namespace HtmlKit.Document.Coverage;

public interface ICoverageManager
{
    int GetCoverageValue(ContextInfo? ctx);
}

public abstract class CoverageCellStyleBuilder : IHtmlCellStyleBuilder
{
    private const string SCssStyleTemplate = "style=\"background-color:{0}; color:black\"";

    public abstract int GetCoverageValue(ContextInfo? ctx);

    public string? BuildCellStyle(IContextKey cell, IContextInfoDataset<ContextInfo> dataset, Dictionary<string, ContextInfo>? index)
    {
        dataset.TryGetValue(cell, out var methods);

        var bgColor = CoverageExts.GetCoverageColorForCell(cell, methods, index, GetCoverageValue);
        var style = bgColor is null ? null : string.Format(SCssStyleTemplate, bgColor);
        return style;
    }
}

public class CoverManager : CoverageCellStyleBuilder, ICoverageManager
{
    private const string SCoverageAttributeName = "coverage";

    public override int GetCoverageValue(ContextInfo? ctx)
    {
        if (ctx == null)
            return 0;

        if (!ctx.Dimensions.TryGetValue(SCoverageAttributeName, out var raw))
        {
            return 0;
        }
        return int.TryParse(raw, out var v)
            ? v
            : 0;
    }
}

public static class CoverageExts
{
    //context: color, ContextInfo, build
    public static string? GetCoverageColorForCell(IContextKey cell, List<ContextInfo>? methods, Dictionary<string, ContextInfo>? index, Func<ContextInfo?, int> DimensionValueExtractor)
    {
        if (methods != null && methods.Count > 0 && index != null)
        {
            var covs = methods
                .Select(method => index.TryGetValue(method.FullName, out var ctx)
                    ? DimensionValueExtractor(ctx)
                    : 0)
                .ToList();

            if (covs.Any())
                return HeatmapColorBuilder.ToHeatmapColor(covs.Average());
        }

        if (index?.TryGetValue(cell.Action, out var actionCtx) ?? false)
        {
            var aVal = DimensionValueExtractor(actionCtx);
            return HeatmapColorBuilder.ToHeatmapColor(aVal);
        }

        if (index?.TryGetValue(cell.Domain, out var domainCtx) ?? false)
        {
            var aVal = DimensionValueExtractor(domainCtx);
            return HeatmapColorBuilder.ToHeatmapColor(aVal);
        }

        return null;
    }
}