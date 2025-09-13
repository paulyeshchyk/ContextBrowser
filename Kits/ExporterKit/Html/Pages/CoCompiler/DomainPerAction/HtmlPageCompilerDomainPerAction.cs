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
using HtmlKit.Writer;
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
    private readonly IHtmlCellDataProducer _htmlPageDataProducer;
    private readonly IHtmlMatrixWriter _htmlMatrixWriter;
    private readonly IHtmlMatrixSummaryBuilder _htmlMatrixSummaryBuilder;
    private readonly IHtmlContentInjector _cellWithCoverageBuilder;
    private readonly IHtmlPageIndex _indexPageProducer;

    public HtmlPageCompilerDomainPerAction(IAppLogger<AppLevel> logger, IContextInfoMapperFactory contextInfoMapperContainer, IContextInfoDatasetProvider datasetProvider, IContextInfoIndexerProvider flatMapperProvider, IFixedHtmlContentManager fixedHtmlManager, IHrefManager hrefManager, IHtmlCellDataProducer htmlPageDataProducer, IHtmlMatrixWriter htmlMatrixWriter, IHtmlContentInjector cellWithCoverageBuilder, IHtmlPageIndex indexPageProducer)
    {
        _logger = logger;
        _contextInfoMapperContainer = contextInfoMapperContainer;
        _datasetProvider = datasetProvider;
        _flatMapperProvider = flatMapperProvider;
        _mapper = _contextInfoMapperContainer.GetMapper(GlobalMapperKeys.DomainPerAction);
        _fixedHtmlManager = fixedHtmlManager;
        _hrefManager = hrefManager;
        _htmlPageDataProducer = htmlPageDataProducer;
        _htmlMatrixWriter = htmlMatrixWriter;
        _htmlMatrixSummaryBuilder = new HtmlMatrixSummaryBuilderDomainPerAction(_datasetProvider);
        _cellWithCoverageBuilder = cellWithCoverageBuilder;
        _indexPageProducer = indexPageProducer;
    }

    // context: html, build

    public Task CompileAsync(IContextClassifier contextClassifier, ExportOptions exportOptions, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.Html, LogLevel.Cntx, "--- IndexHtmlBuilder.Build ---");

        var matrixGenerator = new HtmlMatrixGeneratorDomainPerAction(contextClassifier, _mapper, exportOptions.ExportMatrix.HtmlTable.Orientation, exportOptions.ExportMatrix.UnclassifiedPriority);
        var matrix = matrixGenerator.Generate();

        var _cellStyleBuilder = new CoverManager();

        var summary = _htmlMatrixSummaryBuilder.Build(matrix, exportOptions.ExportMatrix.HtmlTable.Orientation);

        var result = _indexPageProducer.Produce(matrix, summary, exportOptions.ExportMatrix.HtmlTable);

        var outputFile = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.index, "index.html");

        File.WriteAllText(outputFile, result);

        return Task.CompletedTask;
    }
}
