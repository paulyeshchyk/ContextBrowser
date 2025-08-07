using CommandlineKit.Polyfills;
using ContextKit.Matrix;
using ContextKit.Model;
using HtmlKit.Model;
using LoggerKit.Model;
using RoslynKit.Model;
using UmlKit.Model.Options;

namespace ContextBrowser.Infrastructure;

// context: model, appoptions, app
public class AppOptions
{
    [CommandLineArgument("log-config", "JSON configuration for log levels.")]
    public LogConfiguration<AppLevel, LogLevel> LogConfiguration
    {
        get;
        set;
    } = new()
    {
        LogLevels =
        {
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.file, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.Roslyn, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.P_Bld, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.P_Rnd, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.P_Cpl, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.Html, LogLevel = LogLevel.Err }
        }
    };

    [CommandLineArgument("source-path", "The source code path.")]
    public string theSourcePath { get; set; } = ".\\..\\..\\..\\..\\";

    //".\\..\\..\\..\\..\\"
    //".\\..\\..\\..\\..\\ContextBrowser\\Samples\\Orchestra\\FourContextsSample.cs"
    //".\\..\\..\\..\\..\\ContextBrowser\\Samples\\AlphaClass.cs";

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
            RoslynCodeParserAccessorModifierType.@private,
            RoslynCodeParserAccessorModifierType.@internal
        },
        ClassModifierTypes: new()
        {
            RoslynCodeParserAccessorModifierType.@public,
            RoslynCodeParserAccessorModifierType.@protected,
            RoslynCodeParserAccessorModifierType.@private,
            RoslynCodeParserAccessorModifierType.@internal
        },
        MemberTypes: new()
        {
            RoslynCodeParserMemberType.@enum,
            RoslynCodeParserMemberType.@class,
            RoslynCodeParserMemberType.@interface,
            RoslynCodeParserMemberType.@delegate,
            RoslynCodeParserMemberType.@record,
            RoslynCodeParserMemberType.@struct
        },
        FakeNamespaceName: "privateNS",
        FakeOwnerName: "privateTYPE",
        FakeMethodName: "privateMETHOD",
        CustomAssembliesPaths: AssembliesPaths ?? Enumerable.Empty<string>(),
        CreateFailedCallees: true,
        ShowForeignInstancies: false
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
                useMethodAsParticipant: false,
                useActivation: false,
                useReturn: false,
                useSelfCallContinuation: true,
                useContextTransitionTreeBuilderMode: ContextTransitionTreeBuilderMode.FromParentToChild,
                collapseCalleeClassIfSameAsCaller: true);
}