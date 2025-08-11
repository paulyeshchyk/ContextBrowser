using CommandlineKit.Polyfills;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ExporterKit.Options;
using HtmlKit.Model;
using LoggerKit.Model;
using RoslynKit.Model;
using UmlKit.Infrastructure.Options;

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
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.R_Parse, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.P_Bld, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.P_Rnd, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.P_Cpl, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.P_Tran, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.Html, LogLevel = LogLevel.Err }
        }
    };


    [CommandLineArgument("roslyn-options", "The source code path.")]

    public RoslynOptions roslynOptions
    {
        get;
        set;
    } = new(

    //".\\..\\..\\..\\..\\"
    //".\\..\\..\\..\\..\\ContextBrowser\\Samples\\Orchestra\\FourContextsSample.cs"
    //".\\..\\..\\..\\..\\ContextBrowser\\Samples\\AlphaClass.cs";
        theSourcePath: ".\\..\\..\\..\\..\\",
        roslynCodeparserOptions: new(

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
                //RoslynCodeParserMemberType.@enum,
                RoslynCodeParserMemberType.@class,
                //RoslynCodeParserMemberType.@interface,
                RoslynCodeParserMemberType.@delegate,
                //RoslynCodeParserMemberType.@record,
                //RoslynCodeParserMemberType.@struct
            },
            ExternalNamespaceName: "ExternalNS",
            FakeOwnerName: "privateTYPE",
            FakeMethodName: "privateMETHOD",
            CustomAssembliesPaths: AssembliesPaths ?? Enumerable.Empty<string>(),
            CreateFailedCallees: true
        )
    );

    public ExportMatrixOptions matrixOptions
    {
        get;
        set;
    } = new(
        unclassifiedPriority: UnclassifiedPriority.Highest,
        includeAllStandardActions: true
        );


    [CommandLineArgument("output-dir", "The output directory for reports.")]
    public string outputDirectory { get; set; } = ".\\output\\";


    internal static IEnumerable<string> AssembliesPaths { get; set; } = new List<string>() { "." };

    public SummaryPlacement summaryPlacement { get; set; } = SummaryPlacement.AfterFirst;

    public MatrixOrientation matrixOrientation { get; set; } = MatrixOrientation.DomainRows;

    [CommandLineArgument("contexttransition-diagram-options", "Представление контекстной диаграммы")]
    public ContextTransitionDiagramBuilderOptions contextTransitionDiagramBuilderOptions
    {
        get;
        set;
    } = new ContextTransitionDiagramBuilderOptions(
                                        detailLevel: DiagramDetailLevel.Summary,
                                          direction: DiagramDirection.Outgoing,
                                      useActivation: false,
                                          useReturn: false,
                                           useAsync: false,
                            useSelfCallContinuation: false,
                useContextTransitionTreeBuilderMode: ContextTransitionTreeBuilderMode.FromParentToChild,
                                        diagramType: DiagramBuilderKeys.Transition);
}