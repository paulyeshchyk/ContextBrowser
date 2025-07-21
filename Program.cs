using ContextBrowser.exporter;
using ContextBrowser.exporter.UmlSamples;
using ContextBrowser.Extensions;
using ContextBrowser.Generator.Html;
using ContextBrowser.model;
using ContextBrowser.Parser;

namespace ContextBrowser.ContextCommentsParser;

static class Program
{
    public static void Main(string[] args)
    {
        string theSourcePath = ".\\..\\..\\..\\..\\ContextBrowser";
        string theOutputPath = ".\\output\\";

        bool includeAllStandardActions = true;
        var summaryPlacement = IndexGenerator.SummaryPlacement.AfterLast;
        var matrixOrientation = Generator.Matrix.MatrixOrientation.DomainRows;
        var unclassifiedPriority = UnclassifiedPriority.Highest;

        var contextsList = ContextParser.Parse(theSourcePath);
        var contextClassifier = new ContextClassifier();
        var matrix = ContextMatrixUmlExporter.GenerateMatrix(contextsList, contextClassifier, unclassifiedPriority, includeAllStandardActions);

        var contextLookup = contextsList
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .GroupBy(c => c.ElementType == ContextInfoElementType.method && !string.IsNullOrWhiteSpace(c.ClassOwner)
                ? $"{c.ClassOwner}.{c.Name}"
                : $"{c.Name}")
            .ToDictionary(g => g.Key, g => g.First());

        FileUtils.WipeDirectory(theOutputPath);
        FileUtils.CreateDirectoryIfNotExists(theOutputPath);


        //ContextMatrixUmlExporter.GenerateCsv(matrix, $"{theOutputPath}matrix.csv");
        //HeatmapExporter.GenerateHeatmapCsv(matrix, $"{theOutputPath}heatmap.csv", unclassifiedPriority);


        //UmlPackagesDiagram.Build(contextsList, $"{theOutputPath}uml.packages.domains.puml");
        //UmlMethodsOnlyDiagram.Build(contextsList, $"{theOutputPath}methodlinks.puml");
        //UmlMethodPerActionDomainDiagram.Build(matrix, $"{theOutputPath}uml.packages.actions.puml");

        var links = ContextMatrixUmlExporter.GenerateMethodLinks(contextsList, contextClassifier);
        //SampleLinkedDomain.GenerateLinksUml(links, $"{theOutputPath}uml.4.links.puml");


        // 1.
        UmlComponentDiagram.Build(matrix, theOutputPath);

        // 2.
        UmlActionPerDomainDiagram.Build(matrix, (action, domain) => $"composite_{action}_{domain}.puml", $"{theOutputPath}uml.heatmap.link.puml");

        // 3.
        HeatmapExporter.GenerateContextHtmlPages(matrix, theOutputPath);

        // 4.
        IndexGenerator.GenerateContextIndexHtml(
            matrix,
            contextLookup,
            outputFile: $"{theOutputPath}index.html",
            priority: unclassifiedPriority,
            orientation: matrixOrientation,
            summaryPlacement: summaryPlacement
            );

        HeatmapExporter.GenerateContextDimensionHtmlPages(matrix, theOutputPath);
    }
}
