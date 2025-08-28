using CommandlineKit;
using CommandlineKit.Model;
using ContextBrowser.Html.Composite;
using ContextBrowser.Html.Pages.Index;
using ContextBrowser.Infrastructure;
using ContextBrowser.Model;
using ContextBrowser.Roslyn;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ContextKit.Model.Collector;
using ContextKit.Model.Factory;
using ContextKit.Stategies;
using ExporterKit.HtmlPageSamples;
using ExporterKit.Puml;
using ExporterKit.Uml;
using HtmlKit.Model;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Assembly;
using RoslynKit.Phases;
using RoslynKit.Phases.ContextInfoBuilder;
using RoslynKit.Phases.Invocations;
using RoslynKit.Phases.Syntax.Parsers;
using RoslynKit.Tree;
using RoslynKit.Wrappers.Extractor;
using SemanticKit.Model;

namespace ContextBrowser.ContextCommentsParser;

// context: app, model
public static class Program
{
    // context: app, execute
    public static async Task Main(string[] args)
    {
        var parser = new CommandLineParser();

        if (!parser.TryParse<AppOptions>(args, out var options, out var errorMessage))
        {
            Console.WriteLine(errorMessage);
            return;
        }
        if (options == null)
        {
            Console.WriteLine("Что то пошло не так))");
            Console.WriteLine(CommandLineHelpProducer.GenerateHelpText<AppOptions>(CommandLineDefaults.SArgumentPrefix));
            return;
        }

        using var tokenSource = new CancellationTokenSource();

        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            tokenSource.Cancel();
        };

        try
        {
            await RunAsync(options, tokenSource.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nОтмена по требованию");
        }
    }

    // context: execute
    internal static async Task RunAsync(AppOptions appOptions, CancellationToken cancellationToken)
    {
        var appLogLevelStorage = new AppLoggerLevelStore<AppLevel>();
        appLogLevelStorage.SetLevels(appOptions.LogConfiguration.LogLevels);

        var defaultCW = new ConsoleLogWriter();

        var dependencies = new Dictionary<AppLevel, AppLevel>();
        dependencies[AppLevel.P_Bld] = AppLevel.P_Cpl;
        dependencies[AppLevel.P_Rnd] = AppLevel.P_Cpl;

        var appLogger = new IndentedAppLogger<AppLevel>(appLogLevelStorage, defaultCW, dependencies: dependencies);

        ExportPathDirectoryPreparer.Prepare(appOptions.Export.Paths);

        var strategies = new List<ICommentParsingStrategy<ContextInfo>>() {
            new CoverageStrategy<ContextInfo>(),
            new ContextValidationDecorator<ContextInfo>(appOptions.Classifier, new ContextStrategy<ContextInfo>(appOptions.Classifier), appLogger.WriteLog),
        };

        var processor = new ContextInfoCommentProcessor<ContextInfo>(strategies);

        var syntaxTreeWrapperBuilder = new RoslynSyntaxTreeWrapperBuilder();

        var semanticModelStorage = new SemanticModelStorage(0, appLogger.WriteLog);
        var compilationBuilder = new RoslynCompilationBuilder(appOptions.ParsingOptions.SemanticOptions, appLogger.WriteLog);
        var modelBuilder = new SemanticTreeModelBuilder(compilationBuilder, semanticModelStorage, syntaxTreeWrapperBuilder, appLogger.WriteLog);

        // 1. collector общий для всех парсеров
        var collector = new ContextInfoCollector<ContextInfo>();

        // 2. router (для декларативного парсера)
        var factory = new ContextInfoFactory<ContextInfo>();
        var dependenciesFactory = new CSharpPhaseParserDependenciesFactory<ContextInfo>(collector, factory, processor, appOptions.ParsingOptions.SemanticOptions, appLogger.WriteLog);
        var router = dependenciesFactory.CreateRouter();

        // 3. декларативный парсер
        var semanticDeclarationParser = new SemanticDeclarationParser<ContextInfo>(
            modelBuilder: modelBuilder,
            onWriteLog: appLogger.WriteLog,
            appOptions.ParsingOptions.SemanticOptions,
            router,
            collector);

        // 4. оборачиваем декларативный парсер в IFileParser
        var declarationFileParser = new SemanticDeclarationFileParser(semanticDeclarationParser);

        // 5. референс-парсер (InvocationParser через ReferenceParserBuilder)
        var referenceCollector = new ContextInfoReferenceCollector<ContextInfo>(collector.Collection);
        var semanticInvocationResolver = new RoslynInvocationSemanticResolver(semanticModelStorage);
        var invocationSyntaxExtractor = new RoslynInvocationSyntaxExtractor(semanticInvocationResolver, appOptions.ParsingOptions.SemanticOptions, appLogger.WriteLog);

        var invocationReferenceBuilder = new RoslynInvocationReferenceBuilder<ContextInfo>(appLogger.WriteLog, factory, invocationSyntaxExtractor, appOptions.ParsingOptions.SemanticOptions, referenceCollector);

        var referenceParser = new RoslynInvocationParser<ContextInfo>(
                             collector: referenceCollector,
              semanticTreeModelStorage: semanticModelStorage,
              syntaxTreeWrapperBuilder: syntaxTreeWrapperBuilder,
            invocationReferenceBuilder: invocationReferenceBuilder,
                               options: appOptions.ParsingOptions.SemanticOptions,
                            onWriteLog: appLogger.WriteLog);

        // 6. оборачиваем в IFileParser
        var referenceFileParser = new ReferenceFileParser(referenceParser);

        var fileParsers = new SortedList<int, IFileParser>() { { 0, declarationFileParser }, { 1, referenceFileParser } };

        var contextParser = new RoslynContextParser(fileParsers);

        var cacheModel = appOptions.Export.Paths.CacheModel;
        var contextsList = await ContextListFileManager.ReadContextsFromCache(cacheModel, (ct) => DoParseCode(appOptions, appLogger, contextParser, ct), cancellationToken).ConfigureAwait(false);

        _ = Task.Run(async () =>
        {
            await ContextListFileManager.SaveContextsToCacheAsync(cacheModel, contextsList, cancellationToken).ConfigureAwait(false);
        }, cancellationToken);

        var contextInfoDataset = ContextInfoDatasetBuilder.Build(contextsList, appOptions.Export.ExportMatrix, appOptions.Classifier, appLogger.WriteLog);

        //ExtraDiagramsBuilder.Build(contextInfoMatrixModel, options, options.Classifier, appLogger.WriteLog);

        ComponentDiagram.Build(contextInfoDataset, appOptions, appLogger.WriteLog);

        IndexHtmlBuilder.Build(contextInfoDataset, appOptions, appOptions.Classifier, appLogger.WriteLog);

        HtmlActionPerDomainDiagramBuilder.Build(contextInfoDataset, appOptions, appLogger.WriteLog);

        //
        var actionStateGenerator = new UmlStateActionDiagramCompiler(contextInfoDataset.ContextInfoData, appOptions.Classifier, appOptions.Export, appOptions.DiagramBuilder, appLogger.WriteLog);
        _ = actionStateGenerator.Generate(contextInfoDataset.ContextsList);

        var actionSequenceGenerator = new UmlSequenceActionDiagramCompiler(contextInfoDataset.ContextInfoData, appOptions.Classifier, appOptions.Export, appOptions.DiagramBuilder, appLogger.WriteLog);
        _ = actionSequenceGenerator.Generate(contextInfoDataset.ContextsList);

        var domainSequenceGenerator = new UmlSequenceDomainDiagramCompiler(contextInfoDataset.ContextInfoData, appOptions.Classifier, appOptions.Export, appOptions.DiagramBuilder, appLogger.WriteLog);
        _ = domainSequenceGenerator.Generate(contextInfoDataset.ContextsList);

        // action per domain
        ActionPerDomainPageBuilder.Build(contextInfoDataset, appOptions, appLogger.WriteLogObject);

        // action only
        ActionOnlyPageBuilder.Build(contextInfoDataset, appOptions, appLogger.WriteLogObject);

        // domain only
        DomainOnlyPageBuilder.Build(contextInfoDataset, appOptions, appLogger.WriteLogObject);


        ExtraDiagramsBuilder.Build(contextInfoDataset, appOptions, appOptions.Classifier, appLogger.WriteLog);

        CustomEnvironment.CopyResources(appOptions.Export.Paths.OutputDirectory);
        CustomEnvironment.RunServers(appOptions.Export.Paths.OutputDirectory);
    }

    // context: app
    internal static Task<IEnumerable<ContextInfo>> DoParseCode(AppOptions appOptions, IndentedAppLogger<AppLevel> appLogger, RoslynContextParser contextParser, CancellationToken cancellationToken)
    {
        var sourcePaths = appOptions.Import.SearchPaths;
        var filePaths = PathAnalyzer.GetFilePaths(sourcePaths, appOptions.Import.FileExtensions, appLogger.WriteLog);

        var filtered = PathFilter.FilteroutPaths(filePaths, appOptions.Import.Exclude, (thePath) => thePath);

        if (!filtered.Any())
        {
            throw new Exception("No files to parse");
        }
        return contextParser.ParseAsync(filtered, cancellationToken);
    }
}