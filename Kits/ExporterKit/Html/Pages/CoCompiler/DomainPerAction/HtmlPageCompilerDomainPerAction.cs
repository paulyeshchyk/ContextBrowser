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
using HtmlKit.Extensions;
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

    public HtmlPageCompilerDomainPerAction(IAppLogger<AppLevel> logger, IContextInfoMapperFactory contextInfoMapperContainer, IContextInfoDatasetProvider datasetProvider, IContextInfoIndexerProvider flatMapperProvider)
    {
        _logger = logger;
        _contextInfoMapperContainer = contextInfoMapperContainer;
        _datasetProvider = datasetProvider;
        _flatMapperProvider = flatMapperProvider;
    }

    // context: html, build

    public async Task CompileAsync(IContextClassifier contextClassifier, ExportOptions exportOptions, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.Html, LogLevel.Cntx, "--- IndexHtmlBuilder.Build ---");

        var dataset = await _datasetProvider.GetDatasetAsync(cancellationToken);

        var uiMatrixSummaryBuilder = new HtmlMatrixSummaryBuilderDomainPerAction();

        var matrixGenerator = new HtmlMatrixGeneratorDomainPerAction(contextClassifier, _contextInfoMapperContainer, exportOptions.ExportMatrix.HtmlTable.Orientation, exportOptions.ExportMatrix.UnclassifiedPriority);

        var producer = new HtmlPageProducerMatrix(matrixGenerator, dataset: dataset, flatMapperProvider: _flatMapperProvider, uiMatrixSummaryBuilder, exportOptions.ExportMatrix.HtmlTable);

        // producer.Title = "Контекстная матрица";
        var result = producer.ToHtmlString();
        var outputFile = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.index, "index.html");

        File.WriteAllText(outputFile, result);
    }
}
