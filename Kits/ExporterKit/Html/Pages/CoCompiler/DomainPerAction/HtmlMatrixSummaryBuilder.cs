using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit;
using ExporterKit.Html;
using HtmlKit;
using HtmlKit.Extensions;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;
using TensorKit.Factories;
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

public class HtmlMatrixSummaryBuilder<TTensor> : IHtmlMatrixSummaryBuilder
    where TTensor : notnull
{
    private readonly IContextInfoDatasetProvider<TTensor> _datasetProvider;
    private readonly IContextInfoDataset<ContextInfo, TTensor> _matrix;
    private readonly ITensorFactory<TTensor> _keyFactory;
    private readonly ITensorBuilder _keyBuilder;

    public HtmlMatrixSummaryBuilder(IContextInfoDatasetProvider<TTensor> datasetProvider, ITensorFactory<TTensor> keyFactory, ITensorBuilder keyBuilder)
    {
        _datasetProvider = datasetProvider;
        _matrix = datasetProvider.GetDatasetAsync(CancellationToken.None).GetAwaiter().GetResult();
        _keyFactory = keyFactory;
        _keyBuilder = keyBuilder;
    }

    public HtmlMatrixSummary Build(IHtmlMatrix uiMatrix, TensorPermutationType orientation)
    {
        var colsSummary = ColsSummary(uiMatrix, orientation);
        var rowsSummary = RowsSummary(uiMatrix, orientation);
        return new HtmlMatrixSummary(colsSummary: colsSummary, rowsSummary: rowsSummary);
    }

    public Dictionary<string, int> ColsSummary(IHtmlMatrix uiMatrix, TensorPermutationType orientation)
    {
        return uiMatrix.cols.ToDictionary(col => col, domain => uiMatrix.rows.Sum(action =>
        {
            var key = _keyBuilder.BuildTensor(orientation, new[] { action, domain }, _keyFactory.Create);
            return _matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }

    public Dictionary<string, int> RowsSummary(IHtmlMatrix uiMatrix, TensorPermutationType orientation)
    {
        return uiMatrix.rows.ToDictionary(row => row, action => uiMatrix.cols.Sum(domain =>
        {
            var key = _keyBuilder.BuildTensor(orientation, new[] { action, domain }, _keyFactory.Create);
            return _matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }
}
