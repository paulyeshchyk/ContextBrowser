using System;
using System.Collections.Generic;
using System.Linq;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit;
using ExporterKit.Html;
using ExporterKit.Infrastucture;
using HtmlKit.Document;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

// context: htmlmatrix, build
public class HtmlMatrixGeneratorDomainPerAction : IHtmlMatrixGenerator
{
    private readonly IContextClassifier _contextClassifier;
    private readonly IContextKeyMap<ContextInfo, IContextKey> _mapper;
    private readonly MatrixOrientationType _matrixOrientation;
    private readonly UnclassifiedPriorityType _priority;

    public HtmlMatrixGeneratorDomainPerAction(IContextClassifier contextClassifier, IContextKeyMap<ContextInfo, IContextKey> mapper, MatrixOrientationType matrixOrientation, UnclassifiedPriorityType priority)
    {
        _contextClassifier = contextClassifier;
        _mapper = mapper;
        _matrixOrientation = matrixOrientation;
        _priority = priority;
    }

    // context: build, htmlmatrix
    public IHtmlMatrix Generate()
    {
        var rows = SortList(_mapper.GetActions().Distinct().ToList(), _contextClassifier.EmptyAction, _priority);
        var cols = SortList(_mapper.GetDomains().Distinct().ToList(), _contextClassifier.EmptyDomain, _priority);

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