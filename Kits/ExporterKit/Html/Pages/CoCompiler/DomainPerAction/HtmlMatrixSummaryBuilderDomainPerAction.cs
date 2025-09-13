using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit;
using ExporterKit.Html;
using HtmlKit;
using HtmlKit.Extensions;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

public class HtmlMatrixSummaryBuilderDomainPerAction : IHtmlMatrixSummaryBuilder
{
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IContextInfoDataset<ContextInfo> matrix;

    public HtmlMatrixSummaryBuilderDomainPerAction(IContextInfoDatasetProvider datasetProvider)
    {
        _datasetProvider = datasetProvider;
        matrix = datasetProvider.GetDatasetAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    public HtmlMatrixSummary Build(IHtmlMatrix uiMatrix, MatrixOrientationType orientation)
    {
        var colsSummary = ColsSummary(uiMatrix, orientation);
        var rowsSummary = RowsSummary(uiMatrix, orientation);
        return new HtmlMatrixSummary(colsSummary: colsSummary, rowsSummary: rowsSummary);
    }

    public Dictionary<string, int> ColsSummary(IHtmlMatrix uiMatrix, MatrixOrientationType orientation)
    {
        return uiMatrix.cols.ToDictionary(col => col, col => uiMatrix.rows.Sum(row =>
        {
            var key = orientation == MatrixOrientationType.ActionRows ? new ContextKey(row, col) : new ContextKey(col, row);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }

    public Dictionary<string, int> RowsSummary(IHtmlMatrix uiMatrix, MatrixOrientationType orientation)
    {
        return uiMatrix.rows.ToDictionary(row => row, row => uiMatrix.cols.Sum(col =>
        {
            var key = orientation == MatrixOrientationType.ActionRows ? new ContextKey(row, col) : new ContextKey(col, row);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }
}
