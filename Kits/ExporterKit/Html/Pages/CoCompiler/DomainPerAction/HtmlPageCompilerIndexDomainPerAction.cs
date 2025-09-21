using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ContextBrowserKit.Log.Options;
using HtmlKit.Matrix;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using ContextKit.Model.Collector;
using ExporterKit;
using ExporterKit.Html;
using ExporterKit.Infrastucture;
using HtmlKit;
using HtmlKit.Document;
using HtmlKit.Document.Coverage;
using HtmlKit.Helpers;
using HtmlKit.Page;
using HtmlKit.Page.Compiler;
using HtmlKit.Writer;
using LoggerKit;
using TensorKit.Model;
using TensorKit.Model.DomainPerAction;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

// context: html, build
public class HtmlPageCompilerIndexDomainPerAction : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfo2DMap<ContextInfo, DomainPerActionTensor> _mapper;
    private readonly IHtmlPageIndexProducer<DomainPerActionTensor> _indexPageProducer;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IHtmlMatrixGenerator _matrixGenerator;

    public HtmlPageCompilerIndexDomainPerAction(IAppLogger<AppLevel> logger, IContextInfoMapperFactory<DomainPerActionTensor> contextInfoMapperContainer, IHtmlPageIndexProducer<DomainPerActionTensor> indexPageProducer, IAppOptionsStore optionsStore, IHtmlMatrixGenerator matrixGenerator)
    {
        _logger = logger;
        _mapper = contextInfoMapperContainer.GetMapper(GlobalMapperKeys.DomainPerAction);
        _indexPageProducer = indexPageProducer;
        _optionsStore = optionsStore;
        _matrixGenerator = matrixGenerator;
    }

    // context: html, build
    public async Task CompileAsync(CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.Html, LogLevel.Cntx, "--- IndexHtmlBuilder.Build ---");

        var exportOptions = _optionsStore.GetOptions<ExportOptions>();
        var exportMatrixOptions = exportOptions.ExportMatrix;
        var contextClassifier = _optionsStore.GetOptions<ITensorClassifierDomainPerActionContext>();

        var matrix = await _matrixGenerator.GenerateAsync(cancellationToken);
        var result = await _indexPageProducer.ProduceAsync(matrix, cancellationToken);

        var outputFile = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.index, "index.html");

        File.WriteAllText(outputFile, result);
    }
}
