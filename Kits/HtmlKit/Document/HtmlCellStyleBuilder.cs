using System.Collections.Generic;
using ContextKit.Model;

namespace HtmlKit.Document.Coverage;

public class HtmlCellStyleBuilder : IHtmlCellStyleBuilder
{
    private const string SCssStyleTemplate = "style=\"background-color:{0}; color:black\"";
    private readonly IHtmlCellColorCalculator _colorCalculator;

    public HtmlCellStyleBuilder(IHtmlCellColorCalculator colorCalculator)
    {
        _colorCalculator = colorCalculator;
    }

    public string? BuildCellStyle(IContextKey cell, IEnumerable<ContextInfo>? stInfo, Dictionary<string, ContextInfo>? index)
    {
        var bgColor = _colorCalculator.CalculateBgColor(cell, stInfo, index);
        return bgColor is null
            ? null
            : string.Format(SCssStyleTemplate, bgColor);
    }
}