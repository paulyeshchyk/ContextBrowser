using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
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
        UmlContextActionPerDomainDiagram.Build(model.matrix,(action, domain) => GetCompositeName(action, domain, options.Export), GetHeatmapPath(options.Export), options.DiagramBuilder);

        static string GetCompositeName(string action, string domain, ExportOptions options)
        {
            return ExportPathBuilder.BuildPath(options.Paths, ExportPathType.puml, $"composite_{action}_{domain}.puml");
        }

        static string GetHeatmapPath(ExportOptions options)
        {
            return ExportPathBuilder.BuildPath(options.Paths, ExportPathType.pumlExtra, "uml.heatmap.link.puml");
        }
    }
}