using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;

namespace ExporterKit.Matrix;

// context: matrix, build
public static class UiMatrixGenerator
{
    // context: build, matrix
    public static UiMatrix Generate(IContextClassifier contextClassifier, Dictionary<ContextContainer, List<string>> matrix, MatrixOrientationType matrixOrientation = MatrixOrientationType.DomainRows, UnclassifiedPriorityType priority = UnclassifiedPriorityType.None)
    {
        var matrix2d = DoSort(contextClassifier, matrix, priority);

        return matrixOrientation switch
        {
            MatrixOrientationType.ActionRows => new UiMatrix() { cols = matrix2d.domains, rows = matrix2d.actions },
            MatrixOrientationType.DomainRows => new UiMatrix() { cols = matrix2d.actions, rows = matrix2d.domains },
            _ => throw new NotImplementedException()
        };
    }

    private static Matrix2D DoSort(IContextClassifier contextClassifier, Dictionary<ContextContainer, List<string>> matrix, UnclassifiedPriorityType priority)
    {
        var actions = matrix.Keys.Select(k => k.Action).Distinct().ToList();
        var domains = matrix.Keys.Select(k => k.Domain).Distinct().ToList();

        actions = SortList(actions, contextClassifier.EmptyAction, priority);
        domains = SortList(domains, contextClassifier.EmptyDomain, priority);

        return new Matrix2D(actions, domains);
    }

    // Вспомогательный метод SortList
    private static List<string> SortList(List<string> list, string emptyValue, UnclassifiedPriorityType priority)
    {
        return priority switch
        {
            UnclassifiedPriorityType.Highest => list
                .OrderBy(v => v != emptyValue)
                .ThenBy(v => v)
                .ToList(),
            UnclassifiedPriorityType.Lowest => list
                .OrderBy(v => v == emptyValue)
                .ThenBy(v => v)
                .ToList(),
            _ => list.OrderBy(v => v).ToList() // Использование OrderBy для default-сортировки
        };
    }

    internal record Matrix2D
    {
        public Matrix2D(List<string> actions, List<string> domains)
        {
            this.actions = actions;
            this.domains = domains;
        }

        public List<string> actions { get; set; }

        public List<string> domains { get; set; }
    }
}