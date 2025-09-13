using ContextBrowserKit.Matrix;
using ContextKit.Model;
using HtmlKit.Extensions;
using HtmlKit.Helpers;
using HtmlKit.Options;

namespace HtmlKit.Document;

public interface IHtmlMatrixWriterFactory
{
    IHtmlMatrixWriter Create(IHtmlMatrix matrix, HtmlTableOptions options);
}

public class HtmlMatrixWriterFactory : IHtmlMatrixWriterFactory
{
    private readonly IHtmlDataCellBuilder<ContextKey> _dataCellBuilder;
    private readonly IHrefManager _hRefManager;
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IFixedHtmlContentManager _fixedHtmlContentManager;
    private readonly IHtmlMatrixSummaryBuilder _summaryBuilder;

    public HtmlMatrixWriterFactory(
        IHtmlDataCellBuilder<ContextKey> dataCellBuilder,
        IHrefManager hRefManager,
        IFixedHtmlContentManager fixedHtmlContentManager,
        IHtmlMatrixSummaryBuilder summaryBuilder,
        IContextInfoDatasetProvider datasetProvider)
    {
        _dataCellBuilder = dataCellBuilder;
        _hRefManager = hRefManager;
        _fixedHtmlContentManager = fixedHtmlContentManager;
        _summaryBuilder = summaryBuilder;
        _datasetProvider = datasetProvider;
    }

    public IHtmlMatrixWriter Create(IHtmlMatrix matrix, HtmlTableOptions options)
    {
        return new HtmlMatrixWriter(
            dataCellBuilder: _dataCellBuilder,
            hrefManager: _hRefManager,
            matrix: matrix,
            datasetProvider: _datasetProvider,
            fixedHtmlContentManager: _fixedHtmlContentManager,
            summaryBuilder: _summaryBuilder,
            options: options);
    }
}