using ContextBrowser.ContextKit.Matrix;
using ContextBrowser.ContextKit.Model;
using ContextBrowser.DiagramFactory;
using ContextBrowser.DiagramFactory.Builders;
using ContextBrowser.exporter;
using ContextBrowser.exporter.HtmlPageSamples;
using ContextBrowser.Extensions;
using ContextBrowser.HtmlKit.Exporter;
using ContextBrowser.HtmlKit.Model;
using ContextBrowser.SourceKit.Roslyn;
using ContextBrowser.UmlKit.Diagrams;
using ContextBrowser.UmlKit.Exporter;

namespace ContextBrowser.ContextCommentsParser;

// context: app, model
public static class Program
{
    // context: app, file, delete
    public static void Main(string[] args)
    {
#warning revert path \\Samples\\Test
        string theSourcePath = ".\\..\\..\\..\\..\\ContextBrowser";
        string outputDirectory = ".\\output\\";

        bool includeAllStandardActions = true;
        var summaryPlacement = SummaryPlacement.AfterFirst;
        var matrixOrientation = MatrixOrientation.DomainRows;
        var unclassifiedPriority = UnclassifiedPriority.Highest;

        var contextsList = RoslynContextParser.Parse(theSourcePath, new List<string>() { outputDirectory });

        var contextClassifier = new ContextClassifier();
        var matrix = ContextMatrixUmlExporter.GenerateMatrix(contextsList, contextClassifier, unclassifiedPriority, includeAllStandardActions);

        var contextLookup = contextsList
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .GroupBy(c => c.ElementType == ContextInfoElementType.method && !string.IsNullOrWhiteSpace(c.ClassOwner?.Name)
                ? $"{c.ClassOwner?.Name}.{c.Name}"
                : $"{c.Name}")
            .ToDictionary(g => g.Key, g => g.First());

        FileUtils.WipeDirectory(outputDirectory);
        FileUtils.CreateDirectoryIfNotExists(outputDirectory);


        //ContextMatrixUmlExporter.GenerateCsv(matrix, $"{theOutputPath}matrix.csv");
        //HeatmapExporter.GenerateHeatmapCsv(matrix, $"{theOutputPath}heatmap.csv", unclassifiedPriority);


        UmlContextPackagesDiagram.Build(contextsList, $"{outputDirectory}uml.packages.domains.puml");
        UmlContextMethodsOnlyDiagram.Build(contextsList, $"{outputDirectory}methodlinks.puml");
        UmlContextMethodPerActionDomainDiagram.Build(matrix, $"{outputDirectory}uml.packages.actions.puml");

        var links = ContextMatrixUmlExporter.GenerateMethodLinks(contextsList, contextClassifier);
        UmlContextRelationDiagram.GenerateLinksUml(links, $"{outputDirectory}uml.4.links.puml");


        // 1.
        UmlContextComponentDiagram.Build(matrix, outputDirectory);

        // 2.
        UmlContextActionPerDomainDiagram.Build(matrix,(action, domain) => $"composite_{action}_{domain}.puml", $"{outputDirectory}uml.heatmap.link.puml");

        // 3.
        HtmlIndexPage.GenerateContextHtmlPages(matrix, outputDirectory);

        // 4.
        IndexGenerator.GenerateContextIndexHtml(
            matrix,
            contextLookup,
            outputFile: $"{outputDirectory}index.html",
            priority: unclassifiedPriority,
            orientation: matrixOrientation,
            summaryPlacement: summaryPlacement
            );


        var callback = AdaptToDomainCallback(
            contextItems: contextsList,
            classifier: contextClassifier,
            outputPath: outputDirectory,
            factory:() => ContextDiagramFactory.Get("context-transition")!
        );

        HtmlContextDimensionBuilder builder = new(
            matrix,
            contextsList,
            outputDirectory,
            callback,
            () => ContextDiagramFactory.Transition);

        builder.Build();
    }


    public static Func<string, bool> AdaptToDomainCallback(List<ContextInfo> contextItems, ContextClassifier classifier, string outputPath, Func<IContextDiagramBuilder> factory)
    {
        return domain =>
        {
            var diagram = new UmlDiagramSequence();
            diagram.SetTitle($"Domain: {domain}");
            diagram.SetSkinParam("componentStyle", "rectangle");

            var builder = factory();
            var success = builder.Build(domain, contextItems, classifier, diagram);

            if(success)
            {
                var path = Path.Combine(outputPath, $"sequence_domain_{domain}.puml");
                diagram.WriteToFile(path);
            }

            return success;
        };
    }
}
