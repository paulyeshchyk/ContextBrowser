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

        UmlContextPackagesDiagram.Build(model.contextsList, $"{options.outputDirectory}uml.packages.domains.puml", options.contextTransitionDiagramBuilderOptions);
        UmlContextMethodsOnlyDiagram.Build(model.contextsList, $"{options.outputDirectory}methodlinks.puml", options.contextTransitionDiagramBuilderOptions);
        UmlContextMethodPerActionDomainDiagram.Build(model.matrix, $"{options.outputDirectory}uml.packages.actions.puml", options.contextTransitionDiagramBuilderOptions);

        var links = ContextMatrixUmlExporter.GenerateMethodLinks(model.contextsList, contextClassifier);
        UmlContextRelationDiagram.GenerateLinksUml(links, $"{options.outputDirectory}uml.4.links.puml", options.contextTransitionDiagramBuilderOptions);
    }
}