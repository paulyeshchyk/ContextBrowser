using ContextBrowser.ExporterKit;
using HtmlKit.Exporter;
using LoggerKit;
using LoggerKit.Model;

namespace ContextBrowser.Infrastructure.Samples;

// context: html, build
public static class IndexHtmlBuilder
{
    // context: html, build
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Html, LogLevel.Cntx, "--- IndexHtmlBuilder.Build ---");
        IndexGenerator.GenerateContextIndexHtml(
            model.matrix,
            model.contextLookup,
            outputFile: $"{options.outputDirectory}index.html",
            priority: options.unclassifiedPriority,
            orientation: options.matrixOrientation,
            summaryPlacement: options.summaryPlacement
            );
    }
}