using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
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
            contextClassifier,
            model.matrix,
            model.contextLookup,
            outputFile: $"{options.outputDirectory}index.html",
            priority: options.matrixOptions.unclassifiedPriority,
            orientation: options.matrixOrientation,
            summaryPlacement: options.summaryPlacement
            );
    }
}