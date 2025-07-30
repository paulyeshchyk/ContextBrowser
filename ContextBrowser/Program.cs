using ContextBrowser.ContextKit.Matrix;
using ContextBrowser.ContextKit.Model;
using ContextBrowser.DiagramFactory;
using ContextBrowser.DiagramFactory.Builders;
using ContextBrowser.exporter;
using ContextBrowser.exporter.HtmlPageSamples;
using ContextBrowser.extensions;
using ContextBrowser.Extensions;
using ContextBrowser.HtmlKit.Exporter;
using ContextBrowser.HtmlKit.Model;
using ContextBrowser.LoggerKit;
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
        var appLogLevelStorage = new AppLoggerLevelStore<AppLevel>();
        appLogLevelStorage.SetLevel(AppLevel.file, LogLevel.Error);
        appLogLevelStorage.SetLevel(AppLevel.Csharp, LogLevel.Error);
        appLogLevelStorage.SetLevel(AppLevel.Puml, LogLevel.Error);
        appLogLevelStorage.SetLevel(AppLevel.PumlTransition, LogLevel.Info);
        appLogLevelStorage.SetLevel(AppLevel.Html, LogLevel.Error);

        var appLogger = new AppLogger<AppLevel>(appLogLevelStorage, Console.WriteLine);

        var options = new AppOptions();

        DirectorePreparator.Prepare(options, appLogger.WriteLog);

        var contextBuilderModel = ContextModelBuildBuilder.Build(options, appLogger.WriteLog);

        ExtraDiagramsBuilder.Build(contextBuilderModel, options, appLogger.WriteLog);

        ComponentDiagram.Build(contextBuilderModel, options, appLogger.WriteLog);

        ActionPerDomainDiagramBuilder.Build(contextBuilderModel, options, appLogger.WriteLog);

        ContentHtmlBuilder.Build(contextBuilderModel, options, appLogger.WriteLog);

        IndexHtmlBuilder.Build(contextBuilderModel, options, appLogger.WriteLog);

        DimensionBuilder.Build(contextBuilderModel, options, appLogger.WriteLog);
    }
}
// context: model, appoptions, app

public class AppOptions
{
    public string theSourcePath = ".\\..\\..\\..\\..\\ContextBrowser";
    public string outputDirectory = ".\\output\\";
    public UnclassifiedPriority unclassifiedPriority = UnclassifiedPriority.Highest;
    public bool includeAllStandardActions = true;
    public DiagramBuilderKeys diagramType = DiagramBuilderKeys.Transition;
    public SummaryPlacement summaryPlacement = SummaryPlacement.AfterFirst;
    public MatrixOrientation matrixOrientation = MatrixOrientation.DomainRows;
    public ContextTransitionDiagramBuilderOptions contextTransitionDiagramBuilderOptions = new(
        detailLevel: DiagramDetailLevel.Full,
        direction: DiagramDirection.Incoming,
        defaultParticipantKeyword: UmlParticipantKeyword.Actor,
        useMethodAsParticipant: true);
}

// context: file, read, delete, create
public static class DirectorePreparator
{
    // context: file, read, delete, create
    public static void Prepare(AppOptions options, OnWriteLog? onWriteLog = default)
    {
        FileUtils.CreateDirectoryIfNotExists(options.outputDirectory, onWriteLog: onWriteLog);
        FileUtils.WipeDirectory(options.outputDirectory, onWriteLog: onWriteLog);
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
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Puml, LogLevel.Context, "--- ExtraDiagramsBuilder.Build ---");
        //ContextMatrixUmlExporter.GenerateCsv(matrix, $"{theOutputPath}matrix.csv");
        //HeatmapExporter.GenerateHeatmapCsv(matrix, $"{theOutputPath}heatmap.csv", unclassifiedPriority);


        UmlContextPackagesDiagram.Build(model.contextsList, $"{options.outputDirectory}uml.packages.domains.puml");
        UmlContextMethodsOnlyDiagram.Build(model.contextsList, $"{options.outputDirectory}methodlinks.puml");
        UmlContextMethodPerActionDomainDiagram.Build(model.matrix, $"{options.outputDirectory}uml.packages.actions.puml");

        var links = ContextMatrixUmlExporter.GenerateMethodLinks(model.contextsList, new ContextClassifier());
        UmlContextRelationDiagram.GenerateLinksUml(links, $"{options.outputDirectory}uml.4.links.puml");
    }
}

// context: ContextInfo, build
public static class ContextModelBuildBuilder
{
    // context: ContextInfo, build
    public static ContextBuilderModel Build(AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Csharp, LogLevel.Context, "--- ContextModelBuildBuilder.Build ---");

        var contextsList = RoslynContextParser.Parse(options.theSourcePath, new List<string>() { options.outputDirectory }, onWriteLog);

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
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Html, LogLevel.Context, "--- IndexHtmlBuilder.Build ---");
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
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Puml, LogLevel.Context, "--- ContentHtmlBuilder.Build ---");
        HtmlIndexPage.GenerateContextHtmlPages(model.matrix, options.outputDirectory);
    }
}

public static class ComponentDiagram
{
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Puml, LogLevel.Context, "--- ComponentDiagram.Build ---");
        UmlContextComponentDiagram.Build(model.matrix, options.outputDirectory);
    }
}

// context: step2, build
public static class ActionPerDomainDiagramBuilder
{
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Puml, LogLevel.Context, "--- ActionPerDomainDiagramBuilder.Build ---");
        UmlContextActionPerDomainDiagram.Build(model.matrix,(action, domain) => $"composite_{action}_{domain}.puml", $"{options.outputDirectory}uml.heatmap.link.puml");
    }
}

// context: contextInfo, build
public static class DimensionBuilder
{
    // context: contextInfo, build
    public static void Build(ContextBuilderModel model, AppOptions options, OnWriteLog? onWriteLog = null)
    {
        onWriteLog?.Invoke(AppLevel.Html, LogLevel.Context, "--- DimensionBuilder.Build ---");
        var callback = AdaptToDomainCallback(
            contextItems: model.contextsList,
            classifier: new ContextClassifier(),
            outputPath: options.outputDirectory,
            onWriteLog: onWriteLog,
            factory:(owl) => ContextDiagramFactory.Custom(options.diagramType, options.contextTransitionDiagramBuilderOptions, owl)
        );

        var builder = new HtmlContextDimensionBuilder(
            model.matrix,
            model.contextsList,
            options.outputDirectory,
            callback,
            () => ContextDiagramFactory.Transition(options.contextTransitionDiagramBuilderOptions, onWriteLog));

        builder.Build();
    }

    // context: contextInfo, build
    internal static Func<string, bool> AdaptToDomainCallback(List<ContextInfo> contextItems, ContextClassifier classifier, string outputPath, OnWriteLog? onWriteLog, Func<OnWriteLog?, IContextDiagramBuilder> factory)
    {
        return domain =>
        {
            var diagram = new UmlDiagramSequence();
            diagram.SetTitle($"Domain: {domain}");
            diagram.SetSkinParam("componentStyle", "rectangle");

            var builder = factory(onWriteLog);
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
