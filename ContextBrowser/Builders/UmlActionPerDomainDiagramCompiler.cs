using System.Collections.Generic;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.DiagramCompiler;
using ExporterKit.HtmlPageSamples;
using LoggerKit;
using UmlKit.Exporter;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser.Samples.HtmlPages;

// context: uml, build
public class UmlActionPerDomainDiagramCompiler : IDiagramCompiler
{
    protected readonly IAppLogger<AppLevel> _logger;

    public UmlActionPerDomainDiagramCompiler(IAppLogger<AppLevel> logger)
    {
        _logger = logger;
    }

    // context: uml, build
    public Dictionary<string, bool> Compile(IContextInfoDataset contextInfoDataSet, IContextClassifier contextClassifier, ExportOptions exportOptions, DiagramBuilderOptions diagramBuilderOptions)
    {
        _logger.WriteLog(AppLevel.P_Bld, LogLevel.Cntx, "--- ActionPerDomainDiagramBuilder.Build ---");
        UmlContextActionPerDomainDiagram.Build(contextInfoDataSet.ContextInfoData, (action, domain) => GetCompositeName(action, domain, exportOptions), GetHeatmapPath(exportOptions), diagramBuilderOptions);
        return new() { };
    }


    private static string GetCompositeName(string action, string domain, ExportOptions options)
    {
        return ExportPathBuilder.BuildPath(options.Paths, ExportPathType.puml, $"class_{action}_{domain}.puml");
    }

    private static string GetHeatmapPath(ExportOptions options)
    {
        return ExportPathBuilder.BuildPath(options.Paths, ExportPathType.pumlExtra, "uml.heatmap.link.puml");
    }
}