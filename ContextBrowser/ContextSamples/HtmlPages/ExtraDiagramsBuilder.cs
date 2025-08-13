using ContextBrowser.DiagramFactory.Exporters;
using ContextBrowser.Infrastructure;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextKit.Model;
using ExporterKit.Model;
using UmlKit.Exporter;

namespace ContextBrowser.Samples.HtmlPages;

// context uml, build
public static class ExtraDiagramsBuilder
{
    // context uml, build
    public static void Build(ContextBuilderModel model, AppOptions options, IContextClassifier contextClassifier, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.P_Bld, LogLevel.Cntx, "--- ExtraDiagramsBuilder.Build ---");
        //ContextMatrixUmlExporter.GenerateCsv(matrix, $"{theOutputPath}matrix.csv");
        //HeatmapExporter.GenerateHeatmapCsv(matrix, $"{theOutputPath}heatmap.csv", unclassifiedPriority);

        UmlContextPackagesDiagram.Build(model.contextsList, $"{options.Export.OutputDirectory}uml.packages.domains.puml", options.DiagramBuilder);
        UmlContextMethodsOnlyDiagram.Build(model.contextsList, $"{options.Export.OutputDirectory}methodlinks.puml", options.DiagramBuilder);
        UmlContextMethodPerActionDomainDiagram.Build(model.matrix, $"{options.Export.OutputDirectory}uml.packages.actions.puml", options.DiagramBuilder);

        var links = ContextMatrixUmlExporter.GenerateMethodLinks(model.contextsList, contextClassifier);
        UmlContextRelationDiagram.GenerateLinksUml(links, $"{options.Export.OutputDirectory}uml.4.links.puml", options.DiagramBuilder);
    }
}