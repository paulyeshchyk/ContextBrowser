using ContextBrowser.exporter;
using ContextBrowser.Extensions;
using ContextBrowser.Generator.Html;
using ContextBrowser.Parser;

namespace ContextBrowser.ContextCommentsParser;

public static class Program
{
    public static void Main(string[] args)
    {
        string theSourcePath = ".\\..\\..\\..\\..\\ContextBrowser";
        string theOutputPath = ".\\output\\";

        bool includeUnclassified = true;
        bool includeAllStandardActions = true;
        var result = ContextParser.Parse(theSourcePath);
        var cc = new ContextClassifier();
        var matrix = ContextMatrixUmlExporter.GenerateMatrix(result, cc, includeUnclassified, includeAllStandardActions);

        Console.WriteLine($"Parsed elements: {result.Count}");
        foreach(var ctx in result)
            Console.WriteLine($"[{ctx.ElementType}] {ctx.ClassOwner}.{ctx.Name} — coverage: {ctx.Dimensions.GetValueOrDefault("coverage", "none")}");

        var contextLookup = result
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .GroupBy(c => c.ElementType == "method" && !string.IsNullOrWhiteSpace(c.ClassOwner)
                ? $"{c.ClassOwner}.{c.Name}"
                : c.Name!)
            .ToDictionary(g => g.Key, g => g.First());

        //var result = ContextCommentsParser.ContextParser.ParseFile(thePath);
        //ReferenceParser.EnrichWithReferences(result, thePath);

        FileUtils.WipeDirectory(theOutputPath);
        FileUtils.CreateDirectoryIfNotExists(theOutputPath);

        PlantUmlExporter.GenerateUml(result, $"{theOutputPath}uml.packages.domains.puml");
        ReferenceUmlExporter.GenerateMethodLinks(result, $"{theOutputPath}methodlinks.puml");
        ContextMatrixUmlExporter.GenerateMethodsUml(matrix, $"{theOutputPath}uml.packages.actions.puml");
        ContextMatrixUmlExporter.GenerateCsv(matrix, $"{theOutputPath}matrix.csv");
        HeatmapExporter.GenerateHeatmapCsv(matrix, $"{theOutputPath}heatmap.csv", includeUnclassified);
        HeatmapExporter.GenerateHeatmapUml(matrix, $"{theOutputPath}uml.heatmap.puml");
        HeatmapExporter.GeneratePerCellDiagrams(matrix, $"{theOutputPath}");
        HeatmapExporter.GenerateHeatmapUmlWithLinks(matrix,(action, domain) => $"composite_{action}_{domain}.puml", $"{theOutputPath}uml.heatmap.link.puml");
        HeatmapExporter.GenerateContextHtmlPages(matrix, $"{theOutputPath}");

        IndexGenerator.GenerateContextIndexHtml(
            matrix,
            contextLookup,
            outputFile: $"{theOutputPath}index.html",
            priority: UnclassifiedPriority.Highest,
            orientation: Generator.Matrix.MatrixOrientation.DomainRows,
            summaryPlacement: IndexGenerator.SummaryPlacement.AfterFirst
            );

        HeatmapExporter.GenerateContextDimensionHtmlPages(matrix, $"{theOutputPath}");

        //var links = ContextMatrixUmlExporter.GenerateMethodLinks(result);
        //ContextMatrixUmlExporter.GenerateLinksUml(links, $"{theOutputPath}uml.4.links.puml");
    }
}
