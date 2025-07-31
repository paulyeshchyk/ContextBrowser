using ContextBrowser.exporter.HtmlPageSamples;
using ContextBrowser.extensions;
using ContextBrowser.LoggerKit;

namespace ContextBrowser.Infrastructure.Samples;

// context: step3, build
public static class ContentHtmlBuilder
{
    // context: step3, build
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Puml, LogLevel.Cntx, "--- ContentHtmlBuilder.Build ---");
        HtmlIndexPage.GenerateContextHtmlPages(model.matrix, options.outputDirectory);
    }
}
