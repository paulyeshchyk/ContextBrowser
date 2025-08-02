using CommandlineKit;
using CommandlineKit.Model;
using ContextBrowser.ExporterKit;
using ContextBrowser.Infrastructure;
using ContextBrowser.Infrastructure.Extensions;
using ContextBrowser.Infrastructure.Samples;
using LoggerKit;
using LoggerKit.Model;

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

        var defaultConfig = AppLoggerLevelStoreConfigurationLoader.LoadConfigurationJson<AppLevel, LogLevel>(options.LogLevelConfig, defaultValue: LogLevel.None);
        var appLogLevelStorage = new AppLoggerLevelStore<AppLevel>();
        appLogLevelStorage.SetLevels(defaultConfig);

        var defaultCW = new ConsoleLogWriter();

        var appLogger = new IndentedAppLogger<AppLevel>(appLogLevelStorage, defaultCW);

        DirectoryUtils.Prepare(options, appLogger.WriteLog);

        var contextBuilderModel = ContextModelBuildBuilder.Build(options, appLogger.WriteLog, CancellationToken.None);

        ExtraDiagramsBuilder.Build(contextBuilderModel, options, appLogger.WriteLog);

        ComponentDiagram.Build(contextBuilderModel, options, appLogger.WriteLog);

        ActionPerDomainDiagramBuilder.Build(contextBuilderModel, options, appLogger.WriteLog);

        ContentHtmlBuilder.Build(contextBuilderModel, options, appLogger.WriteLog);

        IndexHtmlBuilder.Build(contextBuilderModel, options, appLogger.WriteLog);

        DimensionBuilder.Build(contextBuilderModel, options, appLogger.WriteLog);
    }
}