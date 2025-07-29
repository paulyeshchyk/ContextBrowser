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
using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.ContextCommentsParser;

// context: app, model
public static class Program
{
    // context: app, execute
    public static void Main(string[] args)
    {
        var options = new AppOptions();

        DirectorePreparator.Prepare(options);

        var contextBuilderModel = ContextModelBuildBuilder.Build(options);

        ExtraDiagramsBuilder.Build(contextBuilderModel, options);

        ComponentDiagram.Build(contextBuilderModel, options);

        ActionPerDomainDiagramBuilder.Build(contextBuilderModel, options);

        ContentHtmlBuilder.Build(contextBuilderModel, options);

        IndexHtmlBuilder.Build(contextBuilderModel, options);

        DimensionBuilder.Build(contextBuilderModel, options);
    }
}

public class AppOptions
{
    public string theSourcePath = ".\\..\\..\\..\\..\\ContextBrowser";
    public string outputDirectory = ".\\output\\";
    public UnclassifiedPriority unclassifiedPriority = UnclassifiedPriority.Highest;
    public bool includeAllStandardActions = true;
    public SummaryPlacement summaryPlacement = SummaryPlacement.AfterFirst;
    public MatrixOrientation matrixOrientation = MatrixOrientation.DomainRows;
    public ContextTransitionDiagramBuilderOptions contextTransitionDiagramBuilderOptions = new ContextTransitionDiagramBuilderOptions(
        detailLevel: DiagramDetailLevel.Full,
        direction: DiagramDirection.BiDirectional,
        defaultParticipantKeyword: UmlParticipantKeyword.Actor,
        useMethodAsParticipant: false);
}

public static class DirectorePreparator
{
    public static void Prepare(AppOptions options)
    {
        FileUtils.CreateDirectoryIfNotExists(options.outputDirectory);
        FileUtils.WipeDirectory(options.outputDirectory);
    }
}

public class ContextBuilderModel
{
    public List<ContextInfo> contextsList;
    public Dictionary<ContextContainer, List<string>> matrix;
    public Dictionary<string, ContextInfo> contextLookup;

    public ContextBuilderModel(List<ContextInfo> contextsList, Dictionary<ContextContainer, List<string>> matrix, Dictionary<string, ContextInfo> contextLookup)
    {
        this.contextsList = contextsList;
        this.matrix = matrix;
        this.contextLookup = contextLookup;
    }
}

public static class ExtraDiagramsBuilder
{
    public static void Build(ContextBuilderModel model, AppOptions options)
    {
        //ContextMatrixUmlExporter.GenerateCsv(matrix, $"{theOutputPath}matrix.csv");
        //HeatmapExporter.GenerateHeatmapCsv(matrix, $"{theOutputPath}heatmap.csv", unclassifiedPriority);


        UmlContextPackagesDiagram.Build(model.contextsList, $"{options.outputDirectory}uml.packages.domains.puml");
        UmlContextMethodsOnlyDiagram.Build(model.contextsList, $"{options.outputDirectory}methodlinks.puml");
        UmlContextMethodPerActionDomainDiagram.Build(model.matrix, $"{options.outputDirectory}uml.packages.actions.puml");

        var links = ContextMatrixUmlExporter.GenerateMethodLinks(model.contextsList, new ContextClassifier());
        UmlContextRelationDiagram.GenerateLinksUml(links, $"{options.outputDirectory}uml.4.links.puml");
    }
}

public static class ContextModelBuildBuilder
{
    public static ContextBuilderModel Build(AppOptions options)
    {
        var contextsList = RoslynContextParser.Parse(options.theSourcePath, new List<string>() { options.outputDirectory });

        var matrix = ContextMatrixUmlExporter.GenerateMatrix(contextsList, new ContextClassifier(), options.unclassifiedPriority, options.includeAllStandardActions);

        var contextLookup = GenerateContextLookup(contextsList);
        return new ContextBuilderModel(contextsList, matrix, contextLookup);
    }

    private static Dictionary<string, ContextInfo> GenerateContextLookup(List<ContextInfo> contextsList)
    {
        return contextsList
            .Where(c => !string.IsNullOrWhiteSpace(c.Name))
            .GroupBy(c => c.ElementType == ContextInfoElementType.method && !string.IsNullOrWhiteSpace(c.ClassOwner?.Name)
                ? $"{c.ClassOwner?.Name}.{c.Name}"
                : $"{c.Name}")
            .ToDictionary(g => g.Key, g => g.First());
    }
}

public static class IndexHtmlBuilder
{
    // context: step3, build
    public static void Build(ContextBuilderModel model, AppOptions options)
    {
        IndexGenerator.GenerateContextIndexHtml(
            model.matrix,
            model.contextLookup,
            outputFile: $"{options.outputDirectory}index.html",
            priority: options.unclassifiedPriority,
            orientation: options.matrixOrientation,
            summaryPlacement: options.summaryPlacement
            );
    }
}

// context: step3, build
public static class ContentHtmlBuilder
{
    // context: step3, build
    public static void Build(ContextBuilderModel model, AppOptions options)
    {
        HtmlIndexPage.GenerateContextHtmlPages(model.matrix, options.outputDirectory);
    }
}

public static class ComponentDiagram
{
    public static void Build(ContextBuilderModel model, AppOptions options)
    {
        UmlContextComponentDiagram.Build(model.matrix, options.outputDirectory);
    }
}

// context: step2, build
public static class ActionPerDomainDiagramBuilder
{
    public static void Build(ContextBuilderModel model, AppOptions options)
    {
        UmlContextActionPerDomainDiagram.Build(model.matrix,(action, domain) => $"composite_{action}_{domain}.puml", $"{options.outputDirectory}uml.heatmap.link.puml");
    }
}

// context: step5, build
public static class DimensionBuilder
{
    // context: step5, build
    public static void Build(ContextBuilderModel model, AppOptions options)
    {
        var callback = AdaptToDomainCallback(
            contextItems: model.contextsList,
            classifier: new ContextClassifier(),
            outputPath: options.outputDirectory,
            factory:() => ContextDiagramFactory.Transition(options.contextTransitionDiagramBuilderOptions)!
        );

        var builder = new HtmlContextDimensionBuilder(
            model.matrix,
            model.contextsList,
            options.outputDirectory,
            callback,
            () => ContextDiagramFactory.Transition(options.contextTransitionDiagramBuilderOptions)!);

        builder.Build();
    }

    // context: step5, build
    internal static Func<string, bool> AdaptToDomainCallback(List<ContextInfo> contextItems, ContextClassifier classifier, string outputPath, Func<IContextDiagramBuilder> factory)
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
