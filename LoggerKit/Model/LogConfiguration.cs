namespace LoggerKit.Model;

// context: log, model
public class LogConfiguration<A, L>
    where A : notnull
    where L : notnull
{
    public List<LogConfigEntry<A, L>> LogLevels { get; set; } = new List<LogConfigEntry<A, L>>();
}
