using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ExporterKit.Model;
using UmlKit.Exporter;

namespace ContextBrowser.Samples.HtmlPages;

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