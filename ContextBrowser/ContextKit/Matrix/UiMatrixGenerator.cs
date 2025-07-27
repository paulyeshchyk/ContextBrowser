using ContextBrowser.ContextKit.Model;

namespace ContextBrowser.ContextKit.Matrix;

// context: matrix, build
public static class UiMatrixGenerator
{
    // context: build, matrix
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

    // context: matrix, build
    private static string highest(string v)
    {
        return v == ContextClassifier.EmptyAction || v == ContextClassifier.EmptyDomain ? string.Empty : v;
    }

    // context: matrix, build
    private static string lowest(string v)
    {
        return v == ContextClassifier.EmptyAction || v == ContextClassifier.EmptyDomain ? ContextClassifier.LowestName : v;
    }
}
