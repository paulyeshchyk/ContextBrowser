using CommandlineKit.Polyfills;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ExporterKit.Options;
using HtmlKit.Options;
using LoggerKit.Model;
using RoslynKit.Model;
using UmlKit.Infrastructure.Options;

namespace ContextBrowser.Infrastructure;

// context: model, appoptions
public class AppOptions
{
    [CommandLineArgument("log-config", "JSON configuration for log levels.")]
    // context: build, appoptions
    public LogConfiguration<AppLevel, LogLevel> LogConfiguration { get; set; } = new()
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
    public RoslynOptions Roslyn { get; set; } = new(

    //".\\..\\..\\..\\..\\ContextBrowser\\Program.cs"
    //".\\..\\..\\..\\..\\ContextBrowser\\ContextSamples\\Orchestra\\FourContextsSample.cs"
    //".\\..\\..\\..\\..\\ContextBrowser\\ContextSamples\\AlphaClass.cs";
        sourcePath: ".\\..\\..\\..\\..\\ContextBrowser\\ContextSamples\\Orchestra\\FourContextsSample.cs",
        roslynCodeParser: new(

            methodModifierTypes: new()
            {
                RoslynCodeParserAccessorModifierType.@public,
                RoslynCodeParserAccessorModifierType.@protected,
                RoslynCodeParserAccessorModifierType.@private,
                RoslynCodeParserAccessorModifierType.@internal
            },
            classModifierTypes: new()
            {
                RoslynCodeParserAccessorModifierType.@public,
                RoslynCodeParserAccessorModifierType.@protected,
                RoslynCodeParserAccessorModifierType.@private,
                RoslynCodeParserAccessorModifierType.@internal
            },
            memberTypes: new()
            {
                //RoslynCodeParserMemberType.@enum,
                RoslynCodeParserMemberType.@class,
                RoslynCodeParserMemberType.@interface,
                RoslynCodeParserMemberType.@delegate,
                RoslynCodeParserMemberType.@record,
                //RoslynCodeParserMemberType.@struct
            },
            externalNamespaceName: "ExternalNS",
            fakeOwnerName: "privateTYPE",
            fakeMethodName: "privateMETHOD",
            customAssembliesPaths: new List<string>() { "." },
            createFailedCallees: true
        )
    );

    [CommandLineArgument("export-options", "Параметры экспорта")]
    public ExportOptions Export { get; set; } = new(
        exportMatrix: new ExportMatrixOptions(
                 unclassifiedPriority: UnclassifiedPriorityType.Highest,
            includeAllStandardActions: true,
                            htmlTable: new HtmlTableOptions(
                                summaryPlacement: SummaryPlacementType.AfterFirst,
                                     orientation: MatrixOrientationType.DomainRows
                                )
        ),
        outputDirectory: ".\\output\\"
    );


    [CommandLineArgument("contexttransition-diagram-options", "Представление контекстной диаграммы")]
    public DiagramBuilderOptions DiagramBuilder { get; set; } = new(
                                              debug: true,
                                        detailLevel: DiagramDetailLevel.Summary,
                                          direction: DiagramDirection.Outgoing,
                                      useActivation: true,
                                      indication: new DiagramBuilderIndication(
                                          useReturn: true,
                                           useAsync: true,
                            useSelfCallContinuation: true,
                                useCalleeActivation: true,

                                useCalleeInvocation: true,
                                            useDone: true,
                                pushAnnotation: false
                                          ),
                useContextTransitionTreeBuilderMode: DiagramBuilderOptions.TreeBuilderMode.FromParentToChild,
                                        diagramType: DiagramBuilderKeys.Transition);

    [CommandLineArgument("context-classifier", "Определение контекста представления")]
    public ContextClassifier Classifier { get; set; } = new(
            emptyAction: "NoAction",
            emptyDomain: "NoDomain",
             fakeAction: "_fakeAction",
             fakeDomain: "_fakeDomain",
        standardActions: new[] { "create", "read", "update", "delete", "validate", "share", "build", "model", "execute" },
              metaItems: new[] { "Action;Domain;Elements" }
        )
    { };
}