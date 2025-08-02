using ContextBrowser.ExporterKit;
using ContextBrowser.ExporterKit.HtmlPageSamples;
using LoggerKit;
using LoggerKit.Model;

namespace ContextBrowser.Infrastructure.Samples;

// context: html, build
public static class ContentHtmlBuilder
{
    // context: html, build
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Puml, LogLevel.Cntx, "--- ContentHtmlBuilder.Build ---");
        HtmlIndexPage.GenerateContextHtmlPages(model.matrix, options.outputDirectory);
    }
}