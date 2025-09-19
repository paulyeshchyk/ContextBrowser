using System.Collections.Generic;
using ContextKit.Model;
using TensorKit.Model;

namespace HtmlKit.Document.Coverage;

public class HtmlCellStyleBuilder<TKey> : IHtmlCellStyleBuilder<TKey>
    where TKey : notnull
{
    private const string SCssStyleTemplate = "style=\"background-color:{0}; color:black\"";
    private readonly IHtmlCellColorCalculator<TKey> _colorCalculator;

    public HtmlCellStyleBuilder(IHtmlCellColorCalculator<TKey> colorCalculator)
    {
        _colorCalculator = colorCalculator;
    }

    public string? BuildCellStyle(TKey cell, IEnumerable<ContextInfo>? stInfo, Dictionary<string, ContextInfo>? index)
    {
        var bgColor = _colorCalculator.CalculateBgColor(cell, stInfo, index);
        return bgColor is null
            ? null
            : string.Format(SCssStyleTemplate, bgColor);
    }
}