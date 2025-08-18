using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit.HtmlPageSamples;
using ExporterKit.Model;

namespace ContextBrowser.Samples.HtmlPages;

// context: contextInfo, build, html
public static class DimensionBuilder
{
    // context: contextInfo, build, html
    public static void Build(ContextBuilderModel model, AppOptions options, IContextClassifier contextClassifier, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Html, LogLevel.Cntx, "--- DimensionBuilder.Build ---");

        var builder = new HtmlContextDimensionBuilder(
            model.matrix,
            model.contextsList,
            contextClassifier,
            options.Export,
            options.DiagramBuilder,
            onWriteLog);

        builder.BuildDimension();
    }
}