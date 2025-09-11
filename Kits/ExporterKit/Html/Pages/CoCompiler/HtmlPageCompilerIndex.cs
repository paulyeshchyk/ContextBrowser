using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ExporterKit.Html.Matrix;
using HtmlKit.Document;
using HtmlKit.Extensions;
using LoggerKit;

namespace HtmlKit.Page.Compiler;

// context: html, build
public class HtmlPageCompilerIndex : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;

    public HtmlPageCompilerIndex(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: html, build

    public void Compile(IContextInfoDataset<ContextInfo> dataset, IContextClassifier contextClassifier, ExportOptions exportOptions)
    {
        _logger.WriteLog(AppLevel.Html, LogLevel.Cntx, "--- IndexHtmlBuilder.Build ---");

        var uiMatrixSummaryBuilder = new UiMatrixSummaryBuilder();

        var indexer = new HtmlMatrixIndexerByNameWithClassOwnerName<ContextInfo>(dataset);

        var matrixGenerator = new HtmlMatrixGenerator(contextClassifier, dataset, exportOptions.ExportMatrix.HtmlTable.Orientation, exportOptions.ExportMatrix.UnclassifiedPriority);

        var producer = new HtmlPageMatrix(matrixGenerator, dataset: dataset, indexer: indexer, uiMatrixSummaryBuilder, exportOptions.ExportMatrix.HtmlTable);

        // producer.Title = "Контекстная матрица";
        var result = producer.ToHtmlString();
        var outputFile = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.index, "index.html");

        File.WriteAllText(outputFile, result);
    }
}

internal class HtmlMatrixIndexerByNameWithClassOwnerName<TContext> : IHtmlMatrixIndexer<TContext>
    where TContext : IContextWithReferences<TContext>
{
    private readonly IContextInfoDataset<TContext> _dataset;
    private Dictionary<string, TContext>? _index;

    public HtmlMatrixIndexerByNameWithClassOwnerName(IContextInfoDataset<TContext> dataset)
    {
        _dataset = dataset;
    }

    public Dictionary<string, TContext> Build()
    {
        if (_index == null)
        {
            _index = _dataset.GetAll()
                .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                .GroupBy(c => c.NameWithClassOwnerName)
                .ToDictionary(
                    g => g.Key,
                    g => g.First());
        }

        return _index;
    }
}

internal class UiMatrixSummaryBuilder : IUiMatrixSummaryBuilder
{
    public Dictionary<string, int>? ColsSummary(IHtmlMatrix uiMatrix, IContextInfoDataset<ContextInfo> matrix, MatrixOrientationType orientation)
    {
        return uiMatrix.cols.ToDictionary(col => col, col => uiMatrix.rows.Sum(row =>
        {
            var key = orientation == MatrixOrientationType.ActionRows ? new ContextKey(row, col) : new ContextKey(col, row);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }

    public Dictionary<string, int>? RowsSummary(IHtmlMatrix uiMatrix, IContextInfoDataset<ContextInfo> matrix, MatrixOrientationType orientation)
    {
        return uiMatrix.rows.ToDictionary(row => row, row => uiMatrix.cols.Sum(col =>
        {
            var key = orientation == MatrixOrientationType.ActionRows ? new ContextKey(row, col) : new ContextKey(col, row);
            return matrix.TryGetValue(key, out var methods) ? methods.Count : 0;
        }));
    }
}
