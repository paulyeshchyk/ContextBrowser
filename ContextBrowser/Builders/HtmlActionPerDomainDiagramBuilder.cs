using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using UmlKit.Exporter;

namespace ContextBrowser.Samples.HtmlPages;

// context: uml, build
public static class HtmlActionPerDomainDiagramBuilder
{
    // context: uml, build
    public static void Build(IContextInfoDataset model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Cntx, "--- ActionPerDomainDiagramBuilder.Build ---");
        UmlContextActionPerDomainDiagram.Build(model.ContextInfoData, (action, domain) => GetCompositeName(action, domain, options.Export), GetHeatmapPath(options.Export), options.DiagramBuilder);

        static string GetCompositeName(string action, string domain, ExportOptions options)
        {
            return ExportPathBuilder.BuildPath(options.Paths, ExportPathType.puml, $"class_{action}_{domain}.puml");
        }

        static string GetHeatmapPath(ExportOptions options)
        {
            return ExportPathBuilder.BuildPath(options.Paths, ExportPathType.pumlExtra, "uml.heatmap.link.puml");
        }
    }
}