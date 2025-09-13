using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Matrix;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ExporterKit;
using ExporterKit.Html;
using ExporterKit.Infrastucture;
using HtmlKit;
using HtmlKit.Document;
using HtmlKit.Document.Coverage;
using HtmlKit.Extensions;
using HtmlKit.Helpers;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;
using LoggerKit;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

// context: html, build
public class HtmlPageCompilerDomainPerAction : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfoDatasetProvider _datasetProvider;
    private readonly IContextInfoMapperFactory _contextInfoMapperContainer;
    private readonly IContextInfoIndexerProvider _flatMapperProvider;
    private readonly IContextKeyMap<ContextInfo, IContextKey> _mapper;
    private readonly IFixedHtmlContentManager _fixedHtmlManager;
    private readonly IHrefManager _hrefManager;
    private readonly IHtmlPageDataProducer _htmlPageDataProducer;
    private readonly IHtmlMatrixWriterFactory _htmlMatrixWriterFactory;

    public HtmlPageCompilerDomainPerAction(IAppLogger<AppLevel> logger, IContextInfoMapperFactory contextInfoMapperContainer, IContextInfoDatasetProvider datasetProvider, IContextInfoIndexerProvider flatMapperProvider, IFixedHtmlContentManager fixedHtmlManager, IHrefManager hrefManager, IHtmlPageDataProducer htmlPageDataProducer, IHtmlMatrixWriterFactory htmlMatrixWriterFactory)
    {
        _logger = logger;
        _contextInfoMapperContainer = contextInfoMapperContainer;
        _datasetProvider = datasetProvider;
        _flatMapperProvider = flatMapperProvider;
        _mapper = _contextInfoMapperContainer.GetMapper(GlobalMapperKeys.DomainPerAction);
        _fixedHtmlManager = fixedHtmlManager;
        _hrefManager = hrefManager;
        _htmlPageDataProducer = htmlPageDataProducer;
        _htmlMatrixWriterFactory = htmlMatrixWriterFactory;
    }

    // context: html, build

    public async Task CompileAsync(IContextClassifier contextClassifier, ExportOptions exportOptions, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.Html, LogLevel.Cntx, "--- IndexHtmlBuilder.Build ---");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var uiMatrixSummaryBuilder = new HtmlMatrixSummaryBuilderDomainPerAction();

        var matrixGenerator = new HtmlMatrixGeneratorDomainPerAction(contextClassifier, _mapper, exportOptions.ExportMatrix.HtmlTable.Orientation, exportOptions.ExportMatrix.UnclassifiedPriority);
        var indexer = await _flatMapperProvider.GetIndexerAsync(GlobalMapperKeys.NameClassName, CancellationToken.None).ConfigureAwait(false);
        var producerh = new HtmlPageDataProducerDomainAction(_datasetProvider);
        var _cellStyleBuilder = new CoverManager();

        var dcBuilder = new HtmlDataCellBuilder<ContextKey>(_htmlPageDataProducer, _hrefManager, _cellStyleBuilder, indexer, _datasetProvider);

        var producer = new HtmlPageProducerMatrix(
            htmlMatrixGenerator: matrixGenerator, 
            dataset: dataset, 
            indexer: indexer,
            matrixSummaryBuilder: uiMatrixSummaryBuilder, 
            options: exportOptions.ExportMatrix.HtmlTable,
            fixedHtmlManager: _fixedHtmlManager,
            hrefManager: _hrefManager,
            htmlPageDataProducer: producerh,
            datacellBuilder: dcBuilder,
            htmlMatrixWriterFactory: _htmlMatrixWriterFactory);

        // producer.Title = "Контекстная матрица";
        var result = producer.ToHtmlString();
        var outputFile = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.index, "index.html");

        File.WriteAllText(outputFile, result);
    }
}
