using ContextBrowser.ExporterKit;
using LoggerKit;
using LoggerKit.Model;
using UmlKit.Exporter;

namespace ContextBrowser.Infrastructure.Samples;

// context: uml, build
public static class ActionPerDomainDiagramBuilder
{
    // context: uml, build
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Cntx, "--- ActionPerDomainDiagramBuilder.Build ---");
        UmlContextActionPerDomainDiagram.Build(model.matrix,(action, domain) => $"composite_{action}_{domain}.puml", $"{options.outputDirectory}uml.heatmap.link.puml", options.contextTransitionDiagramBuilderOptions);
    }
}