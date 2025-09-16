using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit;
using ExporterKit.Html;
using ExporterKit.Infrastucture;
using HtmlKit.Document;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

// context: htmlmatrix, build
public class HtmlMatrixGeneratorDomainPerAction : IHtmlMatrixGenerator
{
    private readonly DomainPerActionKeyMap<ContextInfo, DomainPerActionTensor> _mapper;

    public HtmlMatrixGeneratorDomainPerAction(DomainPerActionKeyMap<ContextInfo, DomainPerActionTensor> mapper)
    {
        _mapper = mapper;
    }

    // context: build, htmlmatrix
    public Task<IHtmlMatrix> GenerateAsync(IDomainPerActionContextClassifier contextClassifier, TensorPermutationType matrixOrientation, UnclassifiedPriorityType priority, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rows = SortList(_mapper.GetActions().Distinct().ToList(), contextClassifier.EmptyAction, priority);
            var cols = SortList(_mapper.GetDomains().Distinct().ToList(), contextClassifier.EmptyDomain, priority);

            var resultMatrix = new HtmlMatrix(rows, cols);

            return matrixOrientation switch
            {
                TensorPermutationType.Standard => resultMatrix,
                TensorPermutationType.Transposed => resultMatrix.Transpose(),
                _ => throw new NotImplementedException()
            };
        }, cancellationToken);
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