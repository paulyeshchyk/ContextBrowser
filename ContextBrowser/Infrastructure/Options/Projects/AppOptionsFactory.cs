using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CommandlineKit;
using ContextBrowser;
using ContextBrowser.Infrastructure;
using ContextBrowser.Infrastructure.Options;
using ContextBrowser.Infrastructure.Options.Projects;
using ContextBrowserKit.Extensions;
using ContextBrowserKit.Options.Export;
using ContextBrowserKit.Options.Import;
using ContextKit.Model;
using ContextKit.Model.Classifier;
using LoggerKit.Model;

namespace ContextBrowser.Infrastructure.Options.Projects;

public static class AppOptionsFactory
{
    public static AppOptions CreateDefault(string projectKey)
    {
        return projectKey.ToLower() switch
        {
            "gulf" => CreateGulfOptions(),
            "contextbrowser" => CreateContextBrowserOptions(),
            _ => new AppOptions() // Базовые пустые настройки
        };
    }

    private static AppOptions CreateGulfOptions()
    {
        const string path = "C:\\projects\\ascon\\GULF_Backend";
        const string outputPath = ".//output//GULF_Backend";

        var options = new AppOptions();

        options.Import = new ImportOptions(
            searchPaths: [path],
            exclude: "**/obj/**;",
            fileExtensions: ".cs"
        );

        options.Export.FilePaths.OutputDirectory = $"{outputPath}//site";
        options.Export.FilePaths.CacheModel = new CacheJsonModel(renewCache: false,
                                                     input: $"{outputPath}//cache//roslyn.json",
                                                    output: $"{outputPath}//cache//roslyn.json");

        options.Export.WebPaths.CacheModel = new CacheJsonModel(renewCache: false,
                                                     input: $"{outputPath}//cache//roslyn.json",
                                                    output: $"{outputPath}//cache//roslyn.json");
        return options;
    }

    private static AppOptions CreateContextBrowserOptions()
    {
        const string path = ".//..//..//..//";
        const string outputPath = ".//output//ContextBrowser";

        var options = new AppOptions();

        options.Import = new ImportOptions(
            searchPaths: [path],
            exclude: "**/obj/**;",
            fileExtensions: ".cs"
        );

        options.Export.FilePaths.OutputDirectory = $"{outputPath}//site";
        options.Export.FilePaths.CacheModel = new CacheJsonModel(renewCache: false,
                                                     input: $"{outputPath}//cache//roslyn.json",
                                                    output: $"{outputPath}//cache//roslyn.json");

        options.Export.WebPaths.CacheModel = new CacheJsonModel(renewCache: false,
                                                     input: $"{outputPath}//cache//roslyn.json",
                                                    output: $"{outputPath}//cache//roslyn.json");
        return options;
    }
}