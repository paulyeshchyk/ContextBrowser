using ContextBrowser.Parser;

namespace ContextBrowser.Generator.Matrix;

// context: ContextBrowser
public static class UiMatrixGenerator
{
    // context: build
    public static UiMatrix Generate(Dictionary<ContextContainer, List<string>> matrix, MatrixOrientation matrixOrientation = MatrixOrientation.DomainRows, UnclassifiedPriority priority = UnclassifiedPriority.None)
    {
        var actions = matrix.Keys.Select(k => k.Action).Distinct().ToList();
        var domains = matrix.Keys.Select(k => k.Domain).Distinct().ToList();


        if(priority == UnclassifiedPriority.Highest)
        {
            actions = actions.OrderBy(highest).ToList();
            domains = domains.OrderBy(highest).ToList();
        }
        else if(priority == UnclassifiedPriority.Lowest)
        {
            actions = actions.OrderBy(lowest).ToList();
            domains = domains.OrderBy(lowest).ToList();
        }
        else
        {
            actions.Sort();
            domains.Sort();
        }

        return matrixOrientation switch
        {
            MatrixOrientation.ActionRows => new UiMatrix() { cols = domains, rows = actions },
            MatrixOrientation.DomainRows => new UiMatrix() { cols = actions, rows = domains },
            _ => throw new NotImplementedException()
        };
    }

    // context: build
    private static string highest(string v)
    {
        return v == ContextClassifier.EmptyAction || v == ContextClassifier.EmptyDomain ? string.Empty : v;
    }

    // context: build
    private static string lowest(string v)
    {
        return v == ContextClassifier.EmptyAction || v == ContextClassifier.EmptyDomain ? ContextClassifier.LowestName : v;
    }
}

public record UiMatrix
{
    public List<string> rows = null!;
    public List<string> cols = null!;
}

public enum MatrixOrientation
{
    ActionRows,   // строки = действия, колонки = домены
    DomainRows    // строки = домены, колонки = действия
}

public static class UiMatrixExtensions
{
    public static Dictionary<string, int>? RowsSummary(this UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix, MatrixOrientation orientation)
    {
        return uiMatrix.rows.ToDictionary(row => row, row => uiMatrix.cols.Sum(col =>
        {
            var key = orientation == MatrixOrientation.ActionRows ? (row, col) : (col, row);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }

    public static Dictionary<string, int>? ColsSummary(this UiMatrix uiMatrix, Dictionary<ContextContainer, List<string>> matrix, MatrixOrientation orientation)
    {
        return uiMatrix.cols.ToDictionary(col => col, col => uiMatrix.rows.Sum(row =>
        {
            var key = orientation == MatrixOrientation.ActionRows ? (row, col) : (col, row);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }
}