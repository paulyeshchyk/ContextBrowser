using ContextBrowser.DiagramFactory.Exporters;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using UmlKit.Exporter;

namespace ContextBrowser.Samples.HtmlPages;

// context:uml, build
public static class ExtraDiagramsBuilder
{
    // context:uml, build
    public static void Build(IContextInfoDataset model, AppOptions options, IContextClassifier contextClassifier, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Cntx, "--- ExtraDiagramsBuilder.Build ---");

        //ContextMatrixUmlExporter.GenerateCsv(matrix, $"{theOutputPath}matrix.csv");
        //HeatmapExporter.GenerateHeatmapCsv(matrix, $"{theOutputPath}heatmap.csv", unclassifiedPriority);

        UmlContextPackagesDiagram.Build(model.ContextsList, ExportPathBuilder.BuildPath(options.Export.Paths, ExportPathType.pumlExtra, "uml.packages.domains.puml"), options.DiagramBuilder);
        UmlContextMethodsOnlyDiagram.Build(model.ContextsList, ExportPathBuilder.BuildPath(options.Export.Paths, ExportPathType.pumlExtra, "methodlinks.puml"), options.DiagramBuilder);
        UmlContextMethodPerActionDomainDiagram.Build(model.ContextInfoData, ExportPathBuilder.BuildPath(options.Export.Paths, ExportPathType.pumlExtra, "uml.packages.actions.puml"), options.DiagramBuilder);

        var linkGenerator = new ContextInfoDataLinkGenerator(contextClassifier, model.ContextsList);
        var links = linkGenerator.Generate();

        UmlContextRelationDiagram.GenerateLinksUml(links, ExportPathBuilder.BuildPath(options.Export.Paths, ExportPathType.pumlExtra, "uml.4.links.puml"), options.DiagramBuilder);
    }
}