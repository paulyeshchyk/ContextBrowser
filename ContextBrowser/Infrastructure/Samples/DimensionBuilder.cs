using ContextBrowser.DiagramFactory;
using ContextBrowser.ExporterKit;
using LoggerKit;
using LoggerKit.Model;

namespace ContextBrowser.Infrastructure.Samples;

// context: contextInfo, build, html
public static class DimensionBuilder
{
    // context: contextInfo, build, html
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Html, LogLevel.Cntx, "--- DimensionBuilder.Build ---");

        var builder = new HtmlContextDimensionBuilder(
            model.matrix,
            model.contextsList,
            options.outputDirectory,
            options,
            onWriteLog);

        builder.Build();
    }
}