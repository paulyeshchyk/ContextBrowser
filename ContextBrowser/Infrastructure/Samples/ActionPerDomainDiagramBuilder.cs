using ContextBrowser.extensions;
using ContextBrowser.LoggerKit;
using ContextBrowser.UmlKit.Exporter;

namespace ContextBrowser.Infrastructure.Samples;

// context: step2, build
public static class ActionPerDomainDiagramBuilder
{
    // context: step2, build
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Puml, LogLevel.Cntx, "--- ActionPerDomainDiagramBuilder.Build ---");
        UmlContextActionPerDomainDiagram.Build(model.matrix,(action, domain) => $"composite_{action}_{domain}.puml", $"{options.outputDirectory}uml.heatmap.link.puml");
    }
}
