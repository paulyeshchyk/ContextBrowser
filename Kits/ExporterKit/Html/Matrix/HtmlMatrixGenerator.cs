using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using HtmlKit.Document;

namespace ExporterKit.Html.Matrix;

// context: htmlmatrix, build
public class HtmlMatrixGenerator : IHtmlMatrixGenerator
{
    private readonly IContextClassifier _contextClassifier;
    private readonly IContextKeyMap<ContextInfo> _contextKeyMap;
    private readonly MatrixOrientationType _matrixOrientation;
    private readonly UnclassifiedPriorityType _priority;

    public HtmlMatrixGenerator(IContextClassifier contextClassifier, IContextKeyMap<ContextInfo> contextKeyMap, MatrixOrientationType matrixOrientation, UnclassifiedPriorityType priority)
    {
        _contextClassifier = contextClassifier;
        _contextKeyMap = contextKeyMap;
        _matrixOrientation = matrixOrientation;
        _priority = priority;
    }

    // context: build, htmlmatrix
    public IHtmlMatrix Generate()
    {
        var rows = SortList(_contextKeyMap.GetActions().Distinct().ToList(), _contextClassifier.EmptyAction, _priority);
        var cols = SortList(_contextKeyMap.GetDomains().Distinct().ToList(), _contextClassifier.EmptyDomain, _priority);

        var resultMatrix = new HtmlMatrix(rows, cols);

        return _matrixOrientation switch
        {
            MatrixOrientationType.ActionRows => resultMatrix,
            MatrixOrientationType.DomainRows => resultMatrix.Transpose(),
            _ => throw new NotImplementedException()
        };
    }

    // context: ContextInfoMatrix, htmlmatrix, read
    internal List<string> SortList(List<string> list, string emptyValue, UnclassifiedPriorityType priority)
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