using System.Collections.Generic;
using ContextKit.Model;

namespace HtmlKit.Document;

public class HtmlCellStyleBuilder<TTensor> : IHtmlCellStyleBuilder<TTensor>
    where TTensor : notnull
{
    private const string SCssStyleTemplate = "background-color:{0}; color:black;spacing:0";
    private readonly IHtmlCellColorCalculator<TTensor> _colorCalculator;

    public HtmlCellStyleBuilder(IHtmlCellColorCalculator<TTensor> colorCalculator)
    {
        _colorCalculator = colorCalculator;
    }

    public string? BuildCellStyleValue(TTensor cell, IEnumerable<ContextInfo>? stInfo, Dictionary<object, ContextInfo>? index)
    {
        var bgColor = _colorCalculator.CalculateBgColor(cell, stInfo, index);
        return bgColor is null
            ? null
            : string.Format(SCssStyleTemplate, bgColor);
    }
}