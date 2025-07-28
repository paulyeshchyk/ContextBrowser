namespace ContextBrowser.DiagramFactory.Builders;

public readonly record struct UmlTransitionDto
{
    public string? CallerId { get; init; }

    public string? CallerName { get; init; }

    public string? CallerClassName { get; init; }

    public string? CallerMethod { get; init; }

    public string? CalleeId { get; init; }

    public string? CalleeName { get; init; }

    public string? CalleeClassName { get; init; }

    public string? CalleeMethod { get; init; }

    public string? Domain { get; init; }

    public string? RunContext { get; init; }

    public override int GetHashCode()
    {
        return HashCode.Combine(CallerId, CalleeId, Domain);
    }

    public bool Equals(UmlTransitionDto other)
    {
        return string.Equals(CallerId, other.CallerId, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(CalleeId, other.CalleeId, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Domain, other.Domain, StringComparison.OrdinalIgnoreCase);
    }
}