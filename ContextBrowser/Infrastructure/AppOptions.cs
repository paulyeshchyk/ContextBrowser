using ContextBrowser.ContextKit.Matrix;
using ContextBrowser.ContextKit.Model;
using ContextBrowser.DiagramFactory;
using ContextBrowser.DiagramFactory.Builders;
using ContextBrowser.HtmlKit.Model;
using ContextBrowser.Infrastructure.Polyfills;
using ContextBrowser.SourceKit.Roslyn;
using ContextBrowser.UmlKit.Model;

namespace ContextBrowser.Infrastructure;

// context: model, appoptions, app
public class AppOptions
{
    [CommandLineArgument("log-config", "JSON configuration for log levels.")]
    public string LogLevelConfig { get; set; } = "{\"LogLevels\":[{\"AppLevel\":\"file\",\"LogLevel\":\"Err\"},{\"AppLevel\":\"Roslyn\",\"LogLevel\":\"Dbg\"},{\"AppLevel\":\"Puml\",\"LogLevel\":\"Dbg\"},{\"AppLevel\":\"PumlTransition\",\"LogLevel\":\"Dbg\"},{\"AppLevel\":\"Html\",\"LogLevel\":\"Err\"}]}";


    [CommandLineArgument("source-path", "The source code path.")]
    public required string theSourcePath { get; set; } = ".\\..\\..\\..\\..\\ContextBrowser";

    [CommandLineArgument("output-dir", "The output directory for reports.")]
    public string outputDirectory { get; set; } = ".\\output\\";


    [CommandLineArgument("unclassified-priority", "Priority for unclassified items.")]
    public UnclassifiedPriority unclassifiedPriority { get; set; } = UnclassifiedPriority.Highest;

    internal static IEnumerable<string> AssembliesPaths { get; set; } = new List<string>() { "." };

    public IEnumerable<string> assemblyPaths = AssembliesPaths;
    public bool includeAllStandardActions = true;
    public RoslynCodeParserOptions roslynCodeparserOptions = RoslynCodeParserOptions.Default(AssembliesPaths);
    public DiagramBuilderKeys diagramType = DiagramBuilderKeys.Transition;
    public SummaryPlacement summaryPlacement = SummaryPlacement.AfterFirst;
    public MatrixOrientation matrixOrientation = MatrixOrientation.DomainRows;


    [CommandLineArgument("contexttransition-diagram-options", "Представление контекстной диаграммы")]
    public ContextTransitionDiagramBuilderOptions contextTransitionDiagramBuilderOptions
    {
        get;
        set;
    } = new(
        detailLevel: DiagramDetailLevel.Summary,
        direction: DiagramDirection.BiDirectional,
        defaultParticipantKeyword: UmlParticipantKeyword.Actor,
        useMethodAsParticipant: true);
}

