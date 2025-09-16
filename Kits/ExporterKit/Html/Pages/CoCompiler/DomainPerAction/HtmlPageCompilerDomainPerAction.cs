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
using TensorKit.Model;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

// context: html, build
public class HtmlPageCompilerDomainPerAction : IHtmlPageCompiler
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly DomainPerActionKeyMap<ContextInfo, DomainPerActionTensor> _mapper;
    private readonly IHtmlPageIndex _indexPageProducer;

    public HtmlPageCompilerDomainPerAction(IAppLogger<AppLevel> logger, IContextInfoMapperFactory contextInfoMapperContainer, IHtmlPageIndex indexPageProducer)
    {
        _logger = logger;
        _mapper = contextInfoMapperContainer.GetMapper(GlobalMapperKeys.DomainPerAction);
        _indexPageProducer = indexPageProducer;
    }

    // context: html, build

    public async Task CompileAsync(IDomainPerActionContextClassifier contextClassifier, ExportOptions exportOptions, CancellationToken cancellationToken)
    {
        _logger.WriteLog(AppLevel.Html, LogLevel.Cntx, "--- IndexHtmlBuilder.Build ---");

        var exportMatrixOptions = exportOptions.ExportMatrix;
        var matrixGenerator = new HtmlMatrixGeneratorDomainPerAction(_mapper);

        var matrix = await matrixGenerator.GenerateAsync(contextClassifier, exportMatrixOptions.HtmlTable.Orientation, exportMatrixOptions.UnclassifiedPriority, cancellationToken);
        var result = await _indexPageProducer.ProduceAsync(matrix, exportMatrixOptions.HtmlTable, cancellationToken);

        var outputFile = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.index, "index.html");

        File.WriteAllText(outputFile, result);
    }
}
