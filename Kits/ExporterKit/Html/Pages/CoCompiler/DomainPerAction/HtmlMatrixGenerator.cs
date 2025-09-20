using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit;
using ExporterKit.Html;
using ExporterKit.Infrastucture;
using HtmlKit.Document;
using HtmlKit.Options;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

// context: htmlmatrix, build
public class HtmlMatrixGenerator<TKey> : IHtmlMatrixGenerator
    where TKey : notnull
{
    private readonly IContextInfo2DMap<ContextInfo, TKey> _mapper;
    private readonly IAppOptionsStore _optionsStore;

    public HtmlMatrixGenerator(IContextInfo2DMap<ContextInfo, TKey> mapper, IAppOptionsStore optionsStore)
    {
        _mapper = mapper;
        _optionsStore = optionsStore;
    }

    // context: build, htmlmatrix
    public Task<IHtmlMatrix> GenerateAsync(CancellationToken cancellationToken)
    {
        var contextClassifier = _optionsStore.GetOptions<IDomainPerActionContextTensorClassifier>();
        var matrixOptions = _optionsStore.GetOptions<ExportMatrixOptions>();
        var matrixOrientation = matrixOptions.HtmlTable.Orientation;
        var priority = matrixOptions.UnclassifiedPriority;

        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rows = SortList(_mapper.GetRows().Distinct().ToList(), contextClassifier.EmptyAction, priority);
            var cols = SortList(_mapper.GetCols().Distinct().ToList(), contextClassifier.EmptyDomain, priority);

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