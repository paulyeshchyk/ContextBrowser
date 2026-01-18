using System.Collections.Generic;
using ContextBrowserKit.Log;
using ContextBrowserKit.Log.Options;
using ContextBrowserKit.Options;
using LoggerKit.Extensions;
using LoggerKit.Model;

namespace LoggerKit;

public static class AppLoggerFactory
{
    public static IndentedAppLogger<TAppLevel> DefaultIndentedLogger<TAppLevel>()
        where TAppLevel : notnull
    {
        var defaultLogLevels = new AppLoggerLevelStore<TAppLevel>();
        var defaultConsoleWriter = new ConsoleLogWriter();
        var defaultDependencies = new Dictionary<TAppLevel, TAppLevel>();
        return new IndentedAppLogger<TAppLevel>(defaultLogLevels, defaultConsoleWriter, dependencies: defaultDependencies);
    }
}