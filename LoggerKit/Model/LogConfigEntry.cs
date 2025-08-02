namespace LoggerKit.Model;

// context: log, model
public class LogConfigEntry<A, L>
    where A : notnull
    where L : notnull
{
    public A? AppLevel { get; set; }

    public L? LogLevel { get; set; }
}
