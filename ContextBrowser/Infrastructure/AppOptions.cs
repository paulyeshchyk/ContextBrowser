using ContextBrowserKit.Commandline.Polyfills;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextBrowserKit.Options.Import;
using HtmlKit.Options;
using LoggerKit.Model;
using SemanticKit.Model.Options;
using UmlKit.Infrastructure.Options;
using UmlKit.Infrastructure.Options.Activation;
using UmlKit.Infrastructure.Options.Indication;

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
    public CodeParsingOptions Semantic { get; set; } = new(

        semanticOptions: new(

            methodModifierTypes: new()
            {
                SemanticAccessorModifierType.@public,
                SemanticAccessorModifierType.@protected,
                SemanticAccessorModifierType.@private,
                SemanticAccessorModifierType.@internal
            },
            classModifierTypes: new()
            {
                SemanticAccessorModifierType.@public,
                SemanticAccessorModifierType.@protected,
                SemanticAccessorModifierType.@private,
                SemanticAccessorModifierType.@internal
            },
            memberTypes: new()
            {
                //RoslynCodeParserMemberType.@enum,
                SemanticMemberType.@class,
                SemanticMemberType.@interface,
                SemanticMemberType.@delegate,
                SemanticMemberType.@record,
                //RoslynCodeParserMemberType.@struct
            },
            externalNamespaceName: "ExternalNS",
            fakeOwnerName: "privateTYPE",
            fakeMethodName: "privateMETHOD",
            customAssembliesPaths: new List<string>() { "." },
            createFailedCallees: true
        )
    );

    [CommandLineArgument("import-options", "Параметры импорта")]
    public ImportOptions Import { get; set; } = new(
        fileExtensions: ".cs",

        //".//..//..//..//"
        //".//..//..//..//ContextBrowser//Program.cs"
        //".//..//..//..//..//ContextBrowser//Kits//"
        //".//..//..//..//ContextSamples//ContextSamples//S3//FourContextsSample.cs"
        //".//..//..//..//..//ContextBrowser//Kits//ContextBrowserKit//Extensions//FileUtils.cs"
        //"/Users/paul/projects/ContextBrowser/Kits/UmlKit/Builders/IUmlTransitionFactory.cs"
        searchPaths: new[] { ".//..//..//..//" }
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
        paths: new ExportPaths(
            outputDirectory: ".//output",
                 cacheModel: new CacheJsonModel(
                    renewCache: true,
                         input: ".//cache//roslyn.json",
                        output: ".//cache//roslyn.json"
                ),
            new ExportPathItem(ExportPathType.index, "."),
            new ExportPathItem(ExportPathType.puml, "puml"),
            new ExportPathItem(ExportPathType.pages, "pages"),
            new ExportPathItem(ExportPathType.pumlExtra, "puml/extra")
        ).BuildFullPath()
    );

    [CommandLineArgument("contexttransition-diagram-options", "Представление контекстной диаграммы")]
    public DiagramBuilderOptions DiagramBuilder { get; set; } = new(
                                              debug: false,
                                        detailLevel: DiagramDetailLevel.Full,
                                          direction: DiagramDirection.Outgoing,
                                         activation: new DiagramActivationOptions(useActivation: true, useActivationCall: true),
                                  transitionOptions: new DiagramTransitionOptions(useCall: true, useDone: true),
                                   invocationOption: new DiagramInvocationOption(useInvocation: true, useReturn: true),
                                         indication: new DiagramIndicationOption(useAsync: true),
                                           treeMode: DiagramBuilderTreeMode.FromParentToChild,
                                        diagramType: DiagramBuilderKeys.Transition);

    [CommandLineArgument("context-classifier", "Определение контекста представления")]
    public ContextClassifier Classifier { get; set; } = new(
            emptyAction: "NoAction",
            emptyDomain: "NoDomain",
             fakeAction: "_fakeAction",
             fakeDomain: "_fakeDomain",
        standardActions: new[] { "create", "read", "update", "delete", "validate", "share", "build", "model", "execute", "convert", "_fakeAction" },
              metaItems: new[] { "Action;Domain;Elements" }
        )
    { };
}