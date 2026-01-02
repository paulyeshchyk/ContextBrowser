using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using HtmlKit.Document;
using HtmlKit.Matrix;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

// context: htmlmatrix, build
public class HtmlMatrixGenerator<TTensor> : IHtmlMatrixGenerator
    where TTensor : notnull
{
    private readonly IContextInfo2DMap<ContextInfo, TTensor> _mapper;
    private readonly IAppOptionsStore _optionsStore;

    public HtmlMatrixGenerator(IContextInfo2DMap<ContextInfo, TTensor> mapper, IAppOptionsStore optionsStore)
    {
        _mapper = mapper;
        _optionsStore = optionsStore;
    }

    // context: build, htmlmatrix
    public Task<IHtmlMatrix> GenerateAsync(CancellationToken cancellationToken)
    {
        var matrixOptions = _optionsStore.GetOptions<ExportMatrixOptions>();
        var matrixOrientation = matrixOptions.HtmlTable.Orientation;
        var priority = matrixOptions.UnclassifiedPriority;
        var emptyDimensionClassifier = _optionsStore.GetOptions<IEmptyDimensionClassifier>();

        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rows = SortList(_mapper.GetRows().Distinct().ToList(), emptyDimensionClassifier.EmptyAction, priority);
            var cols = SortList(_mapper.GetCols().Distinct().ToList(), emptyDimensionClassifier.EmptyDomain, priority);

            var resultMatrix = new HtmlMatrixDomainPerAction(rows, cols);

            return matrixOrientation switch
            {
                TensorPermutationType.Standard => resultMatrix,
                TensorPermutationType.Transposed => resultMatrix.Transpose(),
                _ => throw new NotImplementedException()
            };
        }, cancellationToken);
    }

    // context: ContextInfoMatrix, htmlmatrix, read
    internal static List<ILabeledValue> SortList(List<ILabeledValue> list, string emptyValue, UnclassifiedPriorityType priority)
    {
        return priority switch
        {
            UnclassifiedPriorityType.Highest => list.OrderBy(v => !v.LabeledData.Equals(emptyValue)).ThenBy(v => v.LabeledData).ToList(),
            UnclassifiedPriorityType.Lowest => list.OrderBy(v => v.LabeledData.Equals(emptyValue)).ThenBy(v => v.LabeledData).ToList(),
            UnclassifiedPriorityType.None => list.OrderBy(v => v.LabeledData).ToList(),
            _ => list.OrderBy(v => v.LabeledData).ToList()
        };
    }
}