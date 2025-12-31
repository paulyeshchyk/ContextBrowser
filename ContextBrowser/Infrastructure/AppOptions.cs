using System.Collections.Generic;
using ContextBrowserKit.Commandline.Polyfills;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextBrowserKit.Options.Import;
using ContextKit.Model.Classifier;
using LoggerKit.Model;
using SemanticKit.Model.Options;
using TensorKit.Model;
using UmlKit.Infrastructure.Options;
using UmlKit.Infrastructure.Options.Activation;
using UmlKit.Infrastructure.Options.Indication;

namespace ContextBrowser.Infrastructure;

// context: model, appoptions
public class AppOptions
{
    [CommandLineArgument("app-execution-mode", "Application ExecutionMode")]
    public AppExecutionMode ExecutionMode { get; set; } = AppExecutionMode.Console;

    [CommandLineArgument("log-config", "JSON configuration for log levels.")]
    // context: build, appoptions
    public LogConfiguration<AppLevel, LogLevel> LogConfiguration { get; set; } = new()
    {
        LogLevels =
        {
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.App, LogLevel = LogLevel.Trace },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.file, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.R_Symbol, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.R_Syntax, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.R_Cntx, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.R_Dll, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.R_Invocation, LogLevel = LogLevel.Err },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.P_Bld, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.P_Rnd, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.P_Cpl, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.P_Tran, LogLevel = LogLevel.Warn },
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.Html, LogLevel = LogLevel.Err } }
    };

    [CommandLineArgument("roslyn-options", "The source code path.")]
    // context: model, appoptions
    public CodeParsingOptions ParsingOptions { get; set; } = new(
        semanticLanguage: "csharp",
        semanticOptions: new(
            semanticFilters: new(
                trustedFilters: new(included: string.Empty, excluded: string.Empty),
                domainFilters: new(included: "**/*", excluded: "**/net6.0/System.Text.Json.dll;**/ContextBrowser.dll;**/SemanticKit*;**/CommandlineKit*;**/ContextBrowserKit*;**/ContextKit*;**/ExporterKit*;**/GraphKit*;**/HtmlKit*;**/LoggerKit*;**/RoslynKit*;**/UmlKit*;**/api-ms-*;"),
                runtimeFilters: new(included: "**/System.Diagnostics.Process.dll;**/System.Net.NetworkInformation.dll;**/System.Net.Primitives.dll;", excluded: string.Empty)////**/ System.Resources.ResourceManager.dll;**/System.Globalization.dll
                ),
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
                SemanticMemberType.@enum,
                SemanticMemberType.@class,
                SemanticMemberType.@interface,
                SemanticMemberType.@delegate,
                SemanticMemberType.@record,
                SemanticMemberType.@struct
            },
            externalNamespaceName: "ExternalNS",
            fakeOwnerName: "privateTYPE",
            fakeMethodName: "privateMETHOD",
            customAssembliesPaths: new List<string>() { "." },
            createFailedCallees: true,
            includePseudoCode: false,
            globalUsings: "System.Text.Json; System.Diagnostics; System.Diagnostics.Process; System.Collections; System.Collections.Immutable; System.Collections.Generic; System.IO; System.Linq; System.Net.Http; System.Threading; System.Threading.Tasks"));

    [CommandLineArgument("import-options", "Параметры импорта")]
    public ImportOptions Import { get; set; } = new(
        exclude: "**/obj/**;**/*Tests*/**",
        fileExtensions: ".cs",

        //".//..//..//..//"
        //".//..//..//..//ContextBrowser//Program.cs"
        //".//..//..//..//..//ContextBrowser//Kits//"
        //".//..//..//..//ContextSamples//ContextSamples//S3//FourContextsSample.cs"
        //".//..//..//..//..//ContextBrowser//Kits//ContextBrowserKit//Extensions//FileUtils.cs"
        //"/Users/paul/projects/ContextBrowser/Kits/UmlKit/Builders/IUmlTransitionFactory.cs"
        searchPaths: new[] { ".//..//..//..//" });

    [CommandLineArgument("export-options", "Параметры экспорта")]
    public ExportOptions Export { get; set; } = new(
        exportMatrix: new ExportMatrixOptions(
                 unclassifiedPriority: UnclassifiedPriorityType.Highest,
            includeAllStandardActions: false,
                            htmlTable: new HtmlTableOptions(
                                summaryPlacement: SummaryPlacementType.AfterFirst,
                                     orientation: TensorPermutationType.Transposed)),
        filePaths: new ExportFilePaths(
            outputDirectory: ".//output",
                      paths: new Dictionary<ExportPathType, string>() { { ExportPathType.index, "." }, { ExportPathType.puml, "puml" }, { ExportPathType.pages, "pages" }, { ExportPathType.pumlExtra, "puml/extra" } },
                 cacheModel: new CacheJsonModel(renewCache: true,
                                                     input: ".//cache//roslyn.json",
                                                    output: ".//cache//roslyn.json")),
        webPaths: new ExportWebPaths(
            outputDirectory: "http://localhost:5500",
                      paths: new Dictionary<ExportPathType, string>() { { ExportPathType.index, "." }, { ExportPathType.puml, "puml" }, { ExportPathType.pages, "pages" }, { ExportPathType.pumlExtra, "puml/extra" } },
                 cacheModel: new CacheJsonModel(renewCache: true,
                                                     input: ".//cache//roslyn.json",
                                                    output: ".//cache//roslyn.json")),
        pumlOptions: new ExportPumlOptions(injectionType: PumlInjectionType.inject));

    [CommandLineArgument("contexttransition-diagram-options", "Представление контекстной диаграммы")]
    public DiagramBuilderOptions DiagramBuilder { get; set; } = new(
                                              debug: false,
                                 diagramDetailLevel: DiagramDetailLevel.Method,
                                   diagramDirection: DiagramDirection.BiDirectional,
                                        diagramType: DiagramBuilderKeys.Transition,
                                         activation: new DiagramActivationOptions(useActivation: true, useActivationCall: true),
                                  transitionOptions: new DiagramTransitionOptions(useCall: true, useDone: true),
                                   invocationOption: new DiagramInvocationOption(useInvocation: true, useReturn: false),
                                         indication: new DiagramIndicationOption(useAsync: true));

    [CommandLineArgument("context-classifier", "Определение контекста представления")]
    public ITensorClassifierDomainPerActionContext Classifier { get; set; } = new DomainPerActionContextTensorClassifier(
        emptyDimensionClassifier: new EmptyDimensionClassifierDomainPerAction(emptyAction: "EmptyAction", emptyDomain: "EmptyDomain"),
         fakeDimensionClassifier: new FakeDimensionClassifierDomainPerAction(fakeAction: "_fakeAction", fakeDomain: "_fakeDomain"),
                       metaItems: new[] { "Action;Domain;Elements" },
              wordRoleClassifier: new ContextClassifier(
                 standardActions: new[] { "create", "read", "update", "delete", "validate", "share", "build", "model", "execute", "convert", "_fakeAction" }
             ));
}