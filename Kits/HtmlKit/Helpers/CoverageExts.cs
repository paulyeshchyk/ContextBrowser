using System;
using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;
using ContextKit.Model.Collector;

namespace HtmlKit.Helpers;

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