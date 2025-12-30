using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextKit.Model;
using HtmlKit.Matrix;
using TensorKit.Factories;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

public class HtmlMatrixSummaryBuilder<TTensor> : IHtmlMatrixSummaryBuilder<TTensor>
    where TTensor : notnull
{
    private readonly IContextInfoDatasetProvider<TTensor> _datasetProvider;
    private readonly ITensorFactory<TTensor> _keyFactory;
    private readonly ITensorBuilder _keyBuilder;

    public HtmlMatrixSummaryBuilder(IContextInfoDatasetProvider<TTensor> datasetProvider, ITensorFactory<TTensor> keyFactory, ITensorBuilder keyBuilder)
    {
        _datasetProvider = datasetProvider;
        _keyFactory = keyFactory;
        _keyBuilder = keyBuilder;
    }

    public async Task<HtmlMatrixSummary> BuildAsync(IHtmlMatrix uiMatrix, TensorPermutationType orientation, CancellationToken cancellationToken)
    {
        var matrix = await _datasetProvider.GetDatasetAsync(cancellationToken).ConfigureAwait(false);
        var colsSummary = ColsSummary(matrix, uiMatrix, orientation);
        var rowsSummary = RowsSummary(matrix, uiMatrix, orientation);
        return new HtmlMatrixSummary(colsSummary: colsSummary, rowsSummary: rowsSummary);
    }

    private Dictionary<object, int> ColsSummary(IContextInfoDataset<ContextInfo, TTensor> matrix, IHtmlMatrix uiMatrix, TensorPermutationType orientation)
    {
        return uiMatrix.cols.DistinctBy(c => c.LabeledData).ToDictionary(col => col.LabeledData, domain => uiMatrix.rows.Sum(action =>
        {
            var key = _keyBuilder.BuildTensor(orientation, new[] { action.LabeledData, domain.LabeledData }, _keyFactory.Create);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }

    private Dictionary<object, int> RowsSummary(IContextInfoDataset<ContextInfo, TTensor> matrix, IHtmlMatrix uiMatrix, TensorPermutationType orientation)
    {
        return uiMatrix.rows.DistinctBy(r => r.LabeledData).ToDictionary(row => row.LabeledData, action => uiMatrix.cols.Sum(domain =>
        {
            var key = _keyBuilder.BuildTensor(orientation, new[] { action.LabeledData, domain.LabeledData }, _keyFactory.Create);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }
}
