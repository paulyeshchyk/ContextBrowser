namespace LoggerKit.Model;

// context: log, model
public class LogConfigEntry<TAppLevel, TLogLevel>
    where TAppLevel : notnull
    where TLogLevel : notnull
{
    public TAppLevel? AppLevel { get; set; }

    public TLogLevel? LogLevel { get; set; }
}
