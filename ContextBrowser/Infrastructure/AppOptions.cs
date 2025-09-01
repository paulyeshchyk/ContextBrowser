﻿using System.Collections.Generic;
using ContextBrowserKit.Commandline.Polyfills;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextBrowserKit.Options.Import;
using ContextKit.Model;
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
            new LogConfigEntry<AppLevel, LogLevel>() { AppLevel = AppLevel.Html, LogLevel = LogLevel.Err }
        }
    };

    [CommandLineArgument("roslyn-options", "The source code path.")]
    public CodeParsingOptions ParsingOptions { get; set; } = new(

        semanticOptions: new(
            semanticFilters: new(
                trustedFilters: new(included: string.Empty, excluded: string.Empty),
                domainFilters: new(included: string.Empty, excluded: "**/ContextBrowser.dll;**/SemanticKit*;**/CommandlineKit*;**/ContextBrowserKit*;**/ContextKit*;**/ExporterKit*;**/GraphKit*;**/HtmlKit*;**/LoggerKit*;**/RoslynKit*;**/UmlKit*"),
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
            createFailedCallees: true,
            includePseudoCode: false));

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
                                     orientation: MatrixOrientationType.DomainRows)),
        paths: new ExportPaths(
            outputDirectory: ".//output",
                 cacheModel: new CacheJsonModel(
                    renewCache: true,
                         input: ".//cache//roslyn.json",
                        output: ".//cache//roslyn.json"),
            new ExportPathItem(ExportPathType.index, "."),
            new ExportPathItem(ExportPathType.puml, "puml"),
            new ExportPathItem(ExportPathType.pages, "pages"),
            new ExportPathItem(ExportPathType.pumlExtra, "puml/extra")).BuildFullPath());

    [CommandLineArgument("contexttransition-diagram-options", "Представление контекстной диаграммы")]
    public DiagramBuilderOptions DiagramBuilder { get; set; } = new(
                                              debug: false,
                                 diagramDetailLevel: DiagramDetailLevel.Method,
                                   diagramDirection: DiagramDirection.BiDirectional,
                                        diagramType: DiagramBuilderKeys.Transition,
                                         activation: new DiagramActivationOptions(useActivation: true, useActivationCall: true),
                                  transitionOptions: new DiagramTransitionOptions(useCall: true, useDone: true),
                                   invocationOption: new DiagramInvocationOption(useInvocation: true, useReturn: true),
                                         indication: new DiagramIndicationOption(useAsync: true));

    [CommandLineArgument("context-classifier", "Определение контекста представления")]
    public IContextClassifier Classifier { get; set; } = new ContextClassifier(
            emptyAction: "NoAction",
            emptyDomain: "NoDomain",
             fakeAction: "_fakeAction",
             fakeDomain: "_fakeDomain",
        standardActions: new[] { "create", "read", "update", "delete", "validate", "share", "build", "model", "execute", "convert", "_fakeAction" },
              metaItems: new[] { "Action;Domain;Elements" })
    { };
}