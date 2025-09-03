using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ContextKit.Model.Matrix;

namespace ExporterKit.Html.Matrix;

// context: htmlmatrix, build
public static class HtmlMatrixGenerator
{
    // context: build, htmlmatrix
    public static HtmlMatrix Generate(IContextClassifier contextClassifier, IContextInfoData matrix, MatrixOrientationType matrixOrientation, UnclassifiedPriorityType priority)
    {
        var rows = SortList(matrix.GetActions().Distinct().ToList(), contextClassifier.EmptyAction, priority);
        var cols = SortList(matrix.GetDomains().Distinct().ToList(), contextClassifier.EmptyDomain, priority);

        var resultMatrix = new HtmlMatrix(rows, cols);

        return matrixOrientation switch
        {
            MatrixOrientationType.ActionRows => resultMatrix,
            MatrixOrientationType.DomainRows => resultMatrix.Transpose(),
            _ => throw new NotImplementedException()
        };
    }

    // context: ContextInfoMatrix, htmlmatrix, read
    internal static List<string> SortList(List<string> list, string emptyValue, UnclassifiedPriorityType priority)
    {
        return priority switch
        {
            UnclassifiedPriorityType.Highest => list.OrderBy(v => !v.Equals(emptyValue)).ThenBy(v => v).ToList(),
            UnclassifiedPriorityType.Lowest => list.OrderBy(v => v.Equals(emptyValue)).ThenBy(v => v).ToList(),
            UnclassifiedPriorityType.None => list.OrderBy(v => v).ToList(),
            _ => list.OrderBy(v => v).ToList()
        };
    }
}