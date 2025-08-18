using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.HtmlPageSamples;
using ExporterKit.Model;

namespace ContextBrowser.Samples.HtmlPages;

// context: html, build
public static class IndexHtmlBuilder
{
    // context: html, build
    public static void Build(ContextBuilderModel model, AppOptions options, IContextClassifier contextClassifier, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Html, LogLevel.Cntx, "--- IndexHtmlBuilder.Build ---");
        IndexGenerator.GenerateContextIndexHtml(
            contextClassifier: contextClassifier,
                       matrix: model.matrix,
               allContextInfo: model.contextLookup,
                   outputFile: ExportPathBuilder.BuildPath(options.Export.Paths, ExportPathType.index, "index.html"),
                matrixOptions: options.Export.ExportMatrix
            );
    }
}