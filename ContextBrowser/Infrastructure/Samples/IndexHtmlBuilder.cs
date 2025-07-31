using ContextBrowser.extensions;
using ContextBrowser.HtmlKit.Exporter;
using ContextBrowser.LoggerKit;

namespace ContextBrowser.Infrastructure.Samples;

public static class IndexHtmlBuilder
{
    // context: step3, build
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
