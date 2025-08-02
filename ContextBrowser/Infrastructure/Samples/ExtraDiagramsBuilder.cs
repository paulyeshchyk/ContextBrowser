using ContextBrowser.DiagramFactory.Exporters;
using ContextBrowser.ExporterKit;
using ContextKit.Model;
using LoggerKit;
using LoggerKit.Model;
using UmlKit.Exporter;

namespace ContextBrowser.Infrastructure.Samples;

// context uml, build
public static class ExtraDiagramsBuilder
{
    // context uml, build
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Puml, LogLevel.Cntx, "--- ExtraDiagramsBuilder.Build ---");
        //ContextMatrixUmlExporter.GenerateCsv(matrix, $"{theOutputPath}matrix.csv");
        //HeatmapExporter.GenerateHeatmapCsv(matrix, $"{theOutputPath}heatmap.csv", unclassifiedPriority);

        UmlContextPackagesDiagram.Build(model.contextsList, $"{options.outputDirectory}uml.packages.domains.puml");
        UmlContextMethodsOnlyDiagram.Build(model.contextsList, $"{options.outputDirectory}methodlinks.puml");
        UmlContextMethodPerActionDomainDiagram.Build(model.matrix, $"{options.outputDirectory}uml.packages.actions.puml");

        var links = ContextMatrixUmlExporter.GenerateMethodLinks(model.contextsList, new ContextClassifier());
        UmlContextRelationDiagram.GenerateLinksUml(links, $"{options.outputDirectory}uml.4.links.puml");
    }
}