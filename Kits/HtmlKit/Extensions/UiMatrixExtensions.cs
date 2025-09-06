using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;

namespace HtmlKit.Extensions;

// context: htmlmatrix, build
// pattern: DTO
// pattern note: utility
public static class UiMatrixExtensions
{
    // context: htmlmatrix, read
    public static Dictionary<string, int>? RowsSummary(this HtmlMatrix uiMatrix, IContextInfoDataset matrix, MatrixOrientationType orientation)
    {
        return uiMatrix.rows.ToDictionary(row => row, row => uiMatrix.cols.Sum(col =>
        {
            var key = orientation == MatrixOrientationType.ActionRows ? new ContextKey(row, col) : new ContextKey(col, row);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }

    // context: htmlmatrix, read
    public static Dictionary<string, int>? ColsSummary(this HtmlMatrix uiMatrix, IContextInfoDataset matrix, MatrixOrientationType orientation)
    {
        return uiMatrix.cols.ToDictionary(col => col, col => uiMatrix.rows.Sum(row =>
        {
            var key = orientation == MatrixOrientationType.ActionRows ? new ContextKey(row, col) : new ContextKey(col, row);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }
}