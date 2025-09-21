using System.Collections.Generic;

namespace HtmlKit.Matrix;

public readonly record struct HtmlMatrixSummary
{
    public readonly Dictionary<object, int> ColsSummary;
    public readonly Dictionary<object, int> RowsSummary;

    public HtmlMatrixSummary(Dictionary<object, int> colsSummary, Dictionary<object, int> rowsSummary)
    {
        RowsSummary = rowsSummary;
        ColsSummary = colsSummary;
    }
}