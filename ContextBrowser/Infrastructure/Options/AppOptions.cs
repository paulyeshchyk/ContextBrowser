using System.Collections.Generic;
using System.Text.Json.Serialization;
using ContextBrowser;
using ContextBrowser.Infrastructure;
using ContextBrowser.Infrastructure.Options;
using ContextBrowserKit.Commandline.Polyfills;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Model;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextBrowserKit.Options.Import;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using LoggerKit.Model;
using SemanticKit.Model.Options;
using TensorKit.Model;
using UmlKit.Infrastructure.Options;
using UmlKit.Infrastructure.Options.Activation;
using UmlKit.Infrastructure.Options.Indication;

namespace ContextBrowser.Infrastructure.Options;

// context: model, appoptions
public class AppOptions
{
    [CommandLineArgument("executionMode", "Application ExecutionMode")]
    public AppExecutionMode ExecutionMode { get; set; } = AppExecutionMode.Console;

    // context: build, appoptions
    [CommandLineArgument("logConfiguration", "JSON configuration for log levels.")]
    public LogConfiguration<AppLevel, LogLevel> LogConfiguration { get; set; } = DefaultLogConfiguration();

    // context: model, appoptions
    [CommandLineArgument("parsingOptions", "Code parsing options")]
    public CodeParsingOptions ParsingOptions { get; set; } = new(
        SemanticLanguage: "csharp",
        SemanticOptions: new(
            semanticFilters: new(
                trustedFilters: new(included: string.Empty, excluded: string.Empty),
                domainFilters: new(included: "**/*", excluded: "**/net8.0/System.Text.Json.dll;**/ContextBrowser.dll;**/ContextSamples.dll;**/SemanticKit.dll;**/TensorKit.dll;**/CommandlineKit.dll;**/ContextBrowserKit.dll;**/ContextKit.dll;**/CustomServers.dll;**/ExporterKit.dll;**/GraphKit.dll;**/HtmlKit.dll;**/LoggerKit.dll;**/RoslynKit.dll;**/UmlKit.dll;**/api-ms-*;"),
                runtimeFilters: new(included: "**/*", excluded: string.Empty)//"**/System.Diagnostics.Process.dll;**/System.Net.NetworkInformation.dll;**/System.Net.Primitives.dll;"
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
            maxDegreeOfParallelism: 12,
            externalNaming: new SemanticExternalNaming(
                namespaceName: "ExternalNamespace",
                className: "ExternalClass",
                methodName: "ExternalMethod",
                resultTypeName: "void"
            ),
            fakeNaming: new SemanticFakeNaming(
                ownerName: "privateTYPE",
                methodName: "privateMETHOD",
                namespaceName: "FakeNamespace",
                className: "FakeClass",
                resultTypeName: "void"
            ),
            customAssembliesPaths: new List<string>() { "." },
            createFailedCallees: true,
            includePseudoCode: false,
            globalUsings: "System.Text.Json; System.Diagnostics; System.Diagnostics.Process; System.Collections; System.Collections.Immutable; System.Collections.Generic; System.IO; System.Linq; System.Net.Http; System.Threading; System.Threading.Tasks"));

    [CommandLineArgument("import-options", "Параметры импорта")]
    public ImportOptions Import { get; set; } = new(
        exclude: "**/obj/**;**/*Tests*/**;",
        fileExtensions: ".cs",

        //".//..//..//..//"
        //".//..//..//..//ContextBrowser//Program.cs"
        //".//..//..//..//..//ContextBrowser//Kits//"
        //".//..//..//..//ContextBrowser//ContextBrowser"
        //".//..//..//..//..//ContextBrowser//ContextSamples//ContextSamples//S6//"
        //".//..//..//..//..//ContextBrowser//Kits//ContextBrowserKit//Extensions//FileUtils.cs"
        //"/Users/paul/projects/ContextBrowser/Kits/UmlKit/Builders/IUmlTransitionFactory.cs"
        searchPaths: [".//..//..//..//"]);

    [CommandLineArgument("export-options", "Параметры экспорта")]
    public ExportOptions Export { get; set; } = new(
        exportMatrix: new ExportMatrixOptions(
                 unclassifiedPriority: UnclassifiedPriorityType.Highest,
            includeAllStandardActions: false,
                            htmlTable: new HtmlTableOptions(
                                summaryPlacement: SummaryPlacementType.AfterFirst,
                                     orientation: TensorPermutationType.Transposed)),
        filePaths: new ExportFilePaths(
            outputDirectory: $".//output//Default//site",
                      paths: new Dictionary<ExportPathType, string>() { { ExportPathType.index, "." }, { ExportPathType.puml, "puml" }, { ExportPathType.pages, "pages" }, { ExportPathType.pumlExtra, "puml/extra" } },
                 cacheModel: new CacheJsonModel(renewCache: false,
                                                     input: $".//output//Default//cache//roslyn.json",
                                                    output: $".//output//Default//cache//roslyn.json")),
        webPaths: new ExportWebPaths(
            outputDirectory: "http://localhost:5500",
                      paths: new Dictionary<ExportPathType, string>() { { ExportPathType.index, "." }, { ExportPathType.puml, "puml" }, { ExportPathType.pages, "pages" }, { ExportPathType.pumlExtra, "puml/extra" } },
                 cacheModel: new CacheJsonModel(renewCache: false,
                                                     input: $".//output//Default//cache//roslyn.json",
                                                    output: $".//output//Default//cache//roslyn.json")),
        pumlOptions: new ExportPumlOptions(injectionType: PumlInjectionType.reference));

    [CommandLineArgument("contexttransition-diagram-options", "Представление контекстной диаграммы")]
    public DiagramBuilderOptions DiagramBuilder { get; set; } = new(
                                              debug: false,
                                 diagramDetailLevel: DiagramDetailLevel.Method,
                                   diagramDirection: DiagramDirection.BiDirectional,
                                        diagramType: DiagramBuilderKeys.Transition,
                                         activation: new DiagramActivationOptions(useActivation: true, useActivationCall: true),
                            calleeTransitionOptions: new DiagramTransitionOptions(useCall: true, useDone: true),
                                  invocationOptions: new DiagramInvocationOption(useInvocation: true, useReturn: false),
                                         indication: new DiagramIndicationOption(useAsync: true));

    [CommandLineArgument("classifier", "Определение контекста представления", typeof(DomainPerActionContextTensorClassifier))]
    public ITensorClassifierDomainPerActionContext<ContextInfo> Classifier { get; set; } = new DomainPerActionContextTensorClassifier(
        emptyDimensionClassifier: new EmptyDimensionClassifierDomainPerAction(emptyAction: "EmptyAction", emptyDomain: "EmptyDomain"),
         fakeDimensionClassifier: new FakeDimensionClassifierDomainPerAction(fakeAction: "_fakeAction", fakeDomain: "_fakeDomain"),
                       metaItems: ["Action;Domain;Elements"],
              wordRoleClassifier: new ContextClassifier(
                 standardActions: ["create", "read", "update", "delete", "validate", "share", "build", "model", "execute", "convert", "_fakeAction"]
             ));

    public static LogConfiguration<AppLevel, LogLevel> DefaultLogConfiguration()
    {
        var levels = new List<LogConfigEntry<AppLevel, LogLevel>>() {
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.App, LogLevel = LogLevel.Warn },
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
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.Html, LogLevel = LogLevel.Err } };
        return new LogConfiguration<AppLevel, LogLevel>() { LogLevels = levels };
    }
}