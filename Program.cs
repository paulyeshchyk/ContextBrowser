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
        bool includeUnclassified = true;
        var unclassifiedPriority = UnclassifiedPriority.Highest;

        var result = ContextParser.Parse(theSourcePath);
        var cc = new ContextClassifier();
        var matrix = ContextMatrixUmlExporter.GenerateMatrix(result, cc, includeUnclassified, includeAllStandardActions);

        Console.WriteLine($"Parsed elements: {result.Count}");
        foreach (var ctx in result)
            Console.WriteLine($"[{ctx.ElementType}] {ctx.ClassOwner}.{ctx.Name} — coverage: {ctx.Dimensions.GetValueOrDefault("coverage", "none")}");

        var contextLookup = result
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .GroupBy(c => c.ElementType == ContextInfoElementType.method && !string.IsNullOrWhiteSpace(c.ClassOwner)
                ? $"{c.ClassOwner}.{c.Name}"
                : c.Name!)
            .ToDictionary(g => g.Key, g => g.First());

        //var result = ContextCommentsParser.ContextParser.ParseFile(thePath);
        //ReferenceParser.EnrichWithReferences(result, thePath);

        FileUtils.WipeDirectory(theOutputPath);
        FileUtils.CreateDirectoryIfNotExists(theOutputPath);

        SampleLinkedDomains2.GenerateUml(result, $"{theOutputPath}uml.packages.domains.puml");
        UmlMethodLinks.GenerateMethodLinks(result, $"{theOutputPath}methodlinks.puml");
        SampleContextMap.GenerateMethodsUml(matrix, $"{theOutputPath}uml.packages.actions.puml");
        ContextMatrixUmlExporter.GenerateCsv(matrix, $"{theOutputPath}matrix.csv");
        HeatmapExporter.GenerateHeatmapCsv(matrix, $"{theOutputPath}heatmap.csv", includeUnclassified);
        UmlHeatmap.GenerateHeatmapUml(matrix, $"{theOutputPath}uml.heatmap.puml");
        UmlSample.GeneratePerCellDiagrams(matrix, $"{theOutputPath}");
        UmlHeatmap.GenerateHeatmapUmlWithLinks(matrix, (action, domain) => $"composite_{action}_{domain}.puml", $"{theOutputPath}uml.heatmap.link.puml");
        HeatmapExporter.GenerateContextHtmlPages(matrix, $"{theOutputPath}");

        IndexGenerator.GenerateContextIndexHtml(
            matrix,
            contextLookup,
            outputFile: $"{theOutputPath}index.html",
            priority: unclassifiedPriority,
            orientation: matrixOrientation,
            summaryPlacement: summaryPlacement
            );

        HeatmapExporter.GenerateContextDimensionHtmlPages(matrix, $"{theOutputPath}");

        var links = ContextMatrixUmlExporter.GenerateMethodLinks(result, cc);
        SampleLinkedDomain.GenerateLinksUml(links, $"{theOutputPath}uml.4.links.puml");
    }
}
