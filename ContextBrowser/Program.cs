using CommandlineKit;
using CommandlineKit.Model;
using ContextBrowser.Infrastructure;
using ContextBrowser.Samples.HtmlPages;
using ContextBrowserKit.Options;
using ContextBrowserKit.Options.Export;
using ExporterKit.Uml;
using LoggerKit;
using LoggerKit.Model;
using RoslynKit.Phases;

namespace ContextBrowser.ContextCommentsParser;

// context: app, model
public static class Program
{
    // context: app, execute
    public static void Main(string[] args)
    {
        var parser = new CommandLineParser();

        if(!parser.TryParse<AppOptions>(args, out var options, out var errorMessage))
        {
            Console.WriteLine(errorMessage);
            return;
        }
        if(options == null)
        {
            Console.WriteLine("Что то пошло не так))");
            Console.WriteLine(CommandLineHelpProducer.GenerateHelpText<AppOptions>(CommandLineDefaults.SArgumentPrefix));
            return;
        }

        var appLogLevelStorage = new AppLoggerLevelStore<AppLevel>();
        appLogLevelStorage.SetLevels(options.LogConfiguration.LogLevels);

        var defaultCW = new ConsoleLogWriter();

        var dependencies = new Dictionary<AppLevel, AppLevel>();
        dependencies[AppLevel.P_Bld] = AppLevel.P_Cpl;
        dependencies[AppLevel.P_Rnd] = AppLevel.P_Cpl;

        var appLogger = new IndentedAppLogger<AppLevel>(appLogLevelStorage, defaultCW, dependencies: dependencies);

        ExportPathDirectoryPreparer.Prepare(options.Export.Paths);


        var semanticParser = new RoslynContextParser(options.Roslyn, options.Classifier, appLogger.WriteLog);


        var cacheModel = options.Export.Paths.CacheModel;
        var contextsList = ContextListFileManager.ReadContextsFromCache(cacheModel,() =>
        {
            return semanticParser.Parse(CancellationToken.None);
        }, CancellationToken.None);

        _ = Task.Run(async () =>
        {
            await ContextListFileManager.SaveContextsToCacheAsync(cacheModel, contextsList, CancellationToken.None).ConfigureAwait(false);
        });


        var contextBuilderModel = ContextModelBuildBuilder.Build(contextsList, options.Export.ExportMatrix, options.Classifier, appLogger.WriteLog);

        //ExtraDiagramsBuilder.Build(contextBuilderModel, options, options.Classifier, appLogger.WriteLog);

        ComponentDiagram.Build(contextBuilderModel, options, appLogger.WriteLog);

        ActionPerDomainDiagramBuilder.Build(contextBuilderModel, options, appLogger.WriteLog);

        ContentHtmlBuilder.Build(contextBuilderModel, options, appLogger.WriteLogObject);

        IndexHtmlBuilder.Build(contextBuilderModel, options, options.Classifier, appLogger.WriteLog);

        DimensionBuilder.Build(contextBuilderModel, options, options.Classifier, appLogger.WriteLog);

        ExtraDiagramsBuilder.Build(contextBuilderModel, options, options.Classifier, appLogger.WriteLog);

        CustomEnvironment.CopyResources(".\\output");
        CustomEnvironment.RunServers(".\\output");
    }
}
