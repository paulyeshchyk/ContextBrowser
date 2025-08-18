using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ExporterKit.HtmlPageSamples;
using ExporterKit.Model;

namespace ContextBrowser.Samples.HtmlPages;

// context: html, build
public static class ContentHtmlBuilder
{
    // context: html, build
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLogObject? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.P_Bld, new LogObject(LogLevel.Cntx, "--- ContentHtmlBuilder.Build ---", LogLevelNode.None));
        HtmlIndexPage.GenerateContextHtmlPages(model.matrix, options.Export);
    }
}