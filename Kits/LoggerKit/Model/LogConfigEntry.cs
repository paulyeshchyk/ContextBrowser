using ContextBrowserKit.Commandline.Polyfills;

namespace LoggerKit.Model;

// context: log, model
public class LogConfigEntry<TAppLevel, TLogLevel>
    where TAppLevel : notnull
    where TLogLevel : notnull
{
    [CommandLineArgument("appLevel", "Уровнень приложения")]
    public TAppLevel? AppLevel { get; set; }

    [CommandLineArgument("logLevel", "Уровнень лога")]
    public TLogLevel? LogLevel { get; set; }
}
