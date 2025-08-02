using CommandlineKit.Polyfills;
using ContextBrowser.DiagramFactory.Model;
using ContextKit.Matrix;
using ContextKit.Model;
using HtmlKit.Model;
using RoslynKit.Model;
using UmlKit.Model;

namespace ContextBrowser.Infrastructure;

// context: model, appoptions, app
public class AppOptions
{
    [CommandLineArgument("log-config", "JSON configuration for log levels.")]
    public string LogLevelConfig { get; set; } = Resources.DefaultLogLevelConfigJson;

    [CommandLineArgument("source-path", "The source code path.")]
    public string theSourcePath { get; set; } = ".\\..\\..\\..\\..\\ContextBrowser";//.\\..\\..\\..\\..\\ContextBrowser\\Program.cs

    [CommandLineArgument("output-dir", "The output directory for reports.")]
    public string outputDirectory { get; set; } = ".\\output\\";

    [CommandLineArgument("unclassified-priority", "Priority for unclassified items.")]
    public UnclassifiedPriority unclassifiedPriority { get; set; } = UnclassifiedPriority.Highest;

    internal static IEnumerable<string> AssembliesPaths { get; set; } = new List<string>() { "." };

    public IEnumerable<string> assemblyPaths { get; set; } = AssembliesPaths;

    public bool includeAllStandardActions { get; set; } = true;

    public RoslynCodeParserOptions roslynCodeparserOptions
    {
        get;
        set;
    } = new(

        MethodModifierTypes: new()
        {
            RoslynCodeParserAccessorModifierType.@public,
            RoslynCodeParserAccessorModifierType.@protected,
            RoslynCodeParserAccessorModifierType.@internal
        },
        ClassModifierTypes: new()
        {
            RoslynCodeParserAccessorModifierType.@public,
            RoslynCodeParserAccessorModifierType.@protected,
            RoslynCodeParserAccessorModifierType.@internal
        },
        MemberTypes: new()
        {
            RoslynCodeParserMemberType.@enum,
            RoslynCodeParserMemberType.@class,
            RoslynCodeParserMemberType.@interface,
            RoslynCodeParserMemberType.@record,
            RoslynCodeParserMemberType.@struct
        },
        CustomAssembliesPaths: AssembliesPaths ?? Enumerable.Empty<string>(),
        CreateFailedCallees: true
    );

    public DiagramBuilderKeys diagramType { get; set; } = DiagramBuilderKeys.Transition;

    public SummaryPlacement summaryPlacement { get; set; } = SummaryPlacement.AfterFirst;

    public MatrixOrientation matrixOrientation { get; set; } = MatrixOrientation.DomainRows;

    [CommandLineArgument("contexttransition-diagram-options", "Представление контекстной диаграммы")]
    public ContextTransitionDiagramBuilderOptions contextTransitionDiagramBuilderOptions
    {
        get;
        set;
    } = new ContextTransitionDiagramBuilderOptions(
                detailLevel: DiagramDetailLevel.Summary,
                direction: DiagramDirection.BiDirectional,
                defaultParticipantKeyword: UmlParticipantKeyword.Actor,
                useMethodAsParticipant: true);
}