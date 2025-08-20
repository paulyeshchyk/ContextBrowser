using CommandlineKit;
using CommandlineKit.Model;
using ContextBrowser.Infrastructure;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ContextKit.Model;
using ExporterKit.Uml;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Route;
using RoslynKit.Route.Tree;
using RoslynKit.Route.Wrappers.Meta;

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

            await RunAsync(options, tokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nОтмена по требованию");
        }
    }

    private static async Task RunAsync(AppOptions options, CancellationToken cancellationToken)
    {
        var appLogLevelStorage = new AppLoggerLevelStore<AppLevel>();
        appLogLevelStorage.SetLevels(options.LogConfiguration.LogLevels);

        var defaultCW = new ConsoleLogWriter();

        var dependencies = new Dictionary<AppLevel, AppLevel>();
        dependencies[AppLevel.P_Bld] = AppLevel.P_Cpl;
        dependencies[AppLevel.P_Rnd] = AppLevel.P_Cpl;

        var appLogger = new IndentedAppLogger<AppLevel>(appLogLevelStorage, defaultCW, dependencies: dependencies);

        ExportPathDirectoryPreparer.Prepare(options.Export.Paths);

        var treeWrapBuilder = new RoslynSyntaxTreeWrapperBuilder();

        var semanticParser = new RoslynContextParser(treeWrapBuilder, options.Semantic, options.Classifier, appLogger.WriteLog);


        var cacheModel = options.Export.Paths.CacheModel;
        List<ContextInfo> contextsList = await ContextListFileManager.ReadContextsFromCache(cacheModel, (cancellationToken) =>
        {
            var sourcePaths = options.Import.SearchPaths;
            var filePaths = PathAnalyzer.GetFilePaths(sourcePaths, options.Import.FileExtensions, appLogger.WriteLog);

            return semanticParser.ParseAsync(filePaths, cancellationToken);
        }, cancellationToken);

        _ = Task.Run(async () =>
        {
            await ContextListFileManager.SaveContextsToCacheAsync(cacheModel, contextsList, cancellationToken).ConfigureAwait(false);
        }, cancellationToken);


        var contextBuilderModel = ContextModelBuildBuilder.Build(contextsList, options.Export.ExportMatrix, options.Classifier, appLogger.WriteLog);

        //ExtraDiagramsBuilder.Build(contextBuilderModel, options, options.Classifier, appLogger.WriteLog);

        ComponentDiagram.Build(contextBuilderModel, options, appLogger.WriteLog);

        ActionPerDomainDiagramBuilder.Build(contextBuilderModel, options, appLogger.WriteLog);

        ContentHtmlBuilder.Build(contextBuilderModel, options, appLogger.WriteLogObject);

        IndexHtmlBuilder.Build(contextBuilderModel, options, options.Classifier, appLogger.WriteLog);

        DimensionBuilder.Build(contextBuilderModel, options, options.Classifier, appLogger.WriteLog);

        ExtraDiagramsBuilder.Build(contextBuilderModel, options, options.Classifier, appLogger.WriteLog);

        CustomEnvironment.CopyResources(".//output");
        CustomEnvironment.RunServers(".//output");

    }
}
