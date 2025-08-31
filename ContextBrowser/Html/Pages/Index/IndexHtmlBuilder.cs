using ContextBrowser.Infrastructure;
using ContextBrowser.Services;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.HtmlPageSamples;
using LoggerKit;

namespace HtmlKit.Page.Compiler;

// context: html, build
public class HtmlIndexBuilder : IHtmlPageCompiler
{

    private readonly IAppLogger<AppLevel> _logger;
    private readonly IContextClassifier _contextClassifier;

    public HtmlIndexBuilder(IAppLogger<AppLevel> logger, IAppOptionsStore ops)
    {
        _logger = logger;
        _contextClassifier = ops.Options().Classifier;
    }

    // context: html, build

    public void Compile(IContextInfoDataset model, ExportOptions exportOptions)
    {
        _logger.WriteLog(AppLevel.Html, LogLevel.Cntx, "--- IndexHtmlBuilder.Build ---");
        HtmlIndexGenerator.GenerateContextIndexHtml(
            contextClassifier: _contextClassifier,
                       matrix: model.ContextInfoData,
               allContextInfo: model.ContextLookup,
                   outputFile: ExportPathBuilder.BuildPath(exportOptions.Paths, ExportPathType.index, "index.html"),
                matrixOptions: exportOptions.ExportMatrix);
    }
}