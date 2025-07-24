using ContextBrowser.ContextKit.Model;

namespace ContextBrowser.ContextKit.Matrix;

// context: matrix, build
// pattern: DTO
// pattern note: utility
public static class UiMatrixExtensions
{
    // context: matrix, build
    public static Dictionary<string, int>? RowsSummary(this UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix, MatrixOrientation orientation)
    {
        return uiMatrix.rows.ToDictionary(row => row, row => uiMatrix.cols.Sum(col =>
        {
            var key = orientation == MatrixOrientation.ActionRows ? (row, col) : (col, row);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }

    // context: matrix, build
    public static Dictionary<string, int>? ColsSummary(this UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix, MatrixOrientation orientation)
    {
        return uiMatrix.cols.ToDictionary(col => col, col => uiMatrix.rows.Sum(row =>
        {
            var key = orientation == MatrixOrientation.ActionRows ? (row, col) : (col, row);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }
}