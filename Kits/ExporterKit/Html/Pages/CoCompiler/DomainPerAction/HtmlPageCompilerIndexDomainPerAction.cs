using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Infrastucture;
using HtmlKit.Document;
using LoggerKit;

namespace ExporterKit.Html.Pages.CoCompiler.DomainPerAction;

// context: html, build
public class HtmlPageCompilerIndexDomainPerAction<TDataTensor> : IHtmlPageCompiler
    where TDataTensor : notnull
{
    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextInfo2DMap<ContextInfo, TDataTensor> _mapper;
    private readonly IHtmlPageIndexProducer<TDataTensor> _indexPageProducer;
    private readonly IAppOptionsStore _optionsStore;
    private readonly IHtmlMatrixGenerator _matrixGenerator;

    public HtmlPageCompilerIndexDomainPerAction(IAppLogger<AppLevel> logger, IContextInfoMapperFactory<TDataTensor> contextInfoMapperContainer, IHtmlPageIndexProducer<TDataTensor> indexPageProducer, IAppOptionsStore optionsStore, IHtmlMatrixGenerator matrixGenerator)
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

        var matrix = await _matrixGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);
        var result = await _indexPageProducer.ProduceAsync(matrix, cancellationToken).ConfigureAwait(false);

        var outputFile = exportOptions.FilePaths.BuildAbsolutePath(ExportPathType.index, "index.html");

        File.WriteAllText(outputFile, result);
    }
}
