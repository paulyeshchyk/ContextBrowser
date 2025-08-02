using ContextBrowser.ExporterKit;
using LoggerKit;
using LoggerKit.Model;
using UmlKit.Exporter;

namespace ContextBrowser.Infrastructure.Samples;

public static class ComponentDiagram
{
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Puml, LogLevel.Cntx, "--- ComponentDiagram.Build ---");
        UmlContextComponentDiagram.Build(model.matrix, options.outputDirectory);
    }
}