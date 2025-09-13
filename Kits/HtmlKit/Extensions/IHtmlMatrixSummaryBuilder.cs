using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using HtmlKit.Document;

namespace HtmlKit.Extensions;

public interface IHtmlMatrixSummaryBuilder
{
    HtmlMatrixSummary Build(IHtmlMatrix uiMatrix, MatrixOrientationType orientation);

    Dictionary<string, int> ColsSummary(IHtmlMatrix uiMatrix, MatrixOrientationType orientation);

    Dictionary<string, int> RowsSummary(IHtmlMatrix uiMatrix, MatrixOrientationType orientation);
}

public readonly record struct HtmlMatrixSummary
{
    public readonly Dictionary<string, int> ColsSummary;
    public readonly Dictionary<string, int> RowsSummary;

    public HtmlMatrixSummary(Dictionary<string, int> colsSummary, Dictionary<string, int> rowsSummary)
    {
        RowsSummary = rowsSummary;
        ColsSummary = colsSummary;
    }
}