namespace LoggerKit.Model;

// context: log, model
public class LogConfiguration<TAppLevel, TLogLevel>
    where TAppLevel : notnull
    where TLogLevel : notnull
{
    public List<LogConfigEntry<TAppLevel, TLogLevel>> LogLevels { get; set; } = new List<LogConfigEntry<TAppLevel, TLogLevel>>();
}
