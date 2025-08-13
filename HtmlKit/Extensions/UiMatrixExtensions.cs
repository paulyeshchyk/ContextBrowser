using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;

namespace HtmlKit.Extensions;

// context: matrix, build
// pattern: DTO
// pattern note: utility
public static class UiMatrixExtensions
{
    // context: matrix, build
    public static Dictionary<string, int>? RowsSummary(this UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix, MatrixOrientationType orientation)
    {
        return uiMatrix.rows.ToDictionary(row => row, row => uiMatrix.cols.Sum(col =>
        {
            var key = orientation == MatrixOrientationType.ActionRows ? (row, col) : (col, row);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }

    // context: matrix, build
    public static Dictionary<string, int>? ColsSummary(this UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix, MatrixOrientationType orientation)
    {
        return uiMatrix.cols.ToDictionary(col => col, col => uiMatrix.rows.Sum(row =>
        {
            var key = orientation == MatrixOrientationType.ActionRows ? (row, col) : (col, row);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }
}