using ContextKit.Model;
using HtmlKit.Helpers;

namespace HtmlKit.Document.Coverage;

public class CoverManager : ICoverageManager
{
    private const string SCssStyleTemplate = "style=\"background-color:{0}; color:black\"";
    private const string SCoverageAttributeName = "coverage";

    public string? BuildCellStyle(ContextContainer cell, List<string>? methods, Dictionary<string, ContextInfo> contextLookup)
    {
        var bgColor = CoverageExts.GetCoverageColorForCell(cell, methods, contextLookup, GetCoverageValue);
        var style = bgColor is null ? null : string.Format(SCssStyleTemplate, bgColor);
        return style;
    }

    public int GetCoverageValue(ContextInfo? ctx)
    {
        if(ctx == null)
            return 0;

        if(!ctx.Dimensions.TryGetValue(SCoverageAttributeName, out var raw))
        {
            return 0;
        }
        return int.TryParse(raw, out var v)
            ? v
            : 0;
    }
}