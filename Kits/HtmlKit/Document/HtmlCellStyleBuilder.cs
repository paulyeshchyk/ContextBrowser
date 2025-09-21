using System.Collections.Generic;
using ContextKit.Model;
using TensorKit.Model;

namespace HtmlKit.Document.Coverage;

public class HtmlCellStyleBuilder<TTensor> : IHtmlCellStyleBuilder<TTensor>
    where TTensor : notnull
{
    private const string SCssStyleTemplate = "style=\"background-color:{0}; color:black\"";
    private readonly IHtmlCellColorCalculator<TTensor> _colorCalculator;

    public HtmlCellStyleBuilder(IHtmlCellColorCalculator<TTensor> colorCalculator)
    {
        _colorCalculator = colorCalculator;
    }

    public string? BuildCellStyle(TTensor cell, IEnumerable<ContextInfo>? stInfo, Dictionary<object, ContextInfo>? index)
    {
        var bgColor = _colorCalculator.CalculateBgColor(cell, stInfo, index);
        return bgColor is null
            ? null
            : string.Format(SCssStyleTemplate, bgColor);
    }
}