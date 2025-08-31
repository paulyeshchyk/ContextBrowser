using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.HtmlPageSamples;
using LoggerKit;

namespace ContextBrowser.Html.Pages.Index;

// context: html, build
public static class HtmlIndexBuilder
{
    // context: html, build
    public static void Build(IContextInfoDataset model, AppOptions options, IContextClassifier contextClassifier, IAppLogger<AppLevel> _logger)
    {
        _logger.WriteLog(AppLevel.Html, LogLevel.Cntx, "--- IndexHtmlBuilder.Build ---");
        HtmlIndexGenerator.GenerateContextIndexHtml(
            contextClassifier: contextClassifier,
                       matrix: model.ContextInfoData,
               allContextInfo: model.ContextLookup,
                   outputFile: ExportPathBuilder.BuildPath(options.Export.Paths, ExportPathType.index, "index.html"),
                matrixOptions: options.Export.ExportMatrix);
    }
}