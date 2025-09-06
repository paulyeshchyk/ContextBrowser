using System;
using System.Collections.Generic;
using System.Linq;
using ContextKit.Model;

namespace HtmlKit.Helpers;

public static class CoverageExts
{
    //context: color, ContextInfo, build
    public static string? GetCoverageColorForCell(IContextKey cell, List<ContextInfo>? methods, Dictionary<string, ContextInfo> contextLookup, Func<ContextInfo?, int> DimensionValueExtractor)
    {
        if (methods != null && methods.Count > 0)
        {
            var covs = methods
                .Select(method => contextLookup.TryGetValue(method.FullName, out var ctx)
                    ? DimensionValueExtractor(ctx)
                    : 0)
                .ToList();

            if (covs.Any())
                return HeatmapColorBuilder.ToHeatmapColor(covs.Average());
        }

        if (contextLookup.TryGetValue(cell.Action, out var actionCtx))
        {
            var aVal = DimensionValueExtractor(actionCtx);
            return HeatmapColorBuilder.ToHeatmapColor(aVal);
        }

        if (contextLookup.TryGetValue(cell.Domain, out var domainCtx))
        {
            var aVal = DimensionValueExtractor(domainCtx);
            return HeatmapColorBuilder.ToHeatmapColor(aVal);
        }

        return null;
    }
}