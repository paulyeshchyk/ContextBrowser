namespace ContextBrowserKit.Filters;

public record FilterPatterns
{
    public string Excluded { get; }

    public string Included { get; }

    public FilterPatterns(string excluded, string included)
    {
        Excluded = excluded;
        Included = included;
    }
}
