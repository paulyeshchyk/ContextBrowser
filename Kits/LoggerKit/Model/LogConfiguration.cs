using System;
using System.Collections.Generic;
using ContextBrowserKit.Commandline.Polyfills;

namespace LoggerKit.Model;

// context: log, model
public class LogConfiguration<TAppLevel, TLogLevel>
    where TAppLevel : notnull
    where TLogLevel : notnull
{
    [CommandLineArgument("LogLevels", "Доступные уровни логирования")]
    public List<LogConfigEntry<TAppLevel, TLogLevel>> LogLevels { get; set; } = new List<LogConfigEntry<TAppLevel, TLogLevel>>();
}