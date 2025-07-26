namespace ContextBrowser.DiagramFactory.Builders;

public readonly record struct UmlTransitionDto
{
    public string CallerId { get; init; }

    public string CalleeId { get; init; }

    public string Domain { get; init; }

    public string? CallerName { get; init; }

    public string? CalleeName { get; init; }

    public string? CallerMethod { get; init; }

    public string? CalleeMethod { get; init; }

    public string? RunContext { get; init; }

    public static implicit operator (string Caller, string Callee, string Domain)(UmlTransitionDto value) => (value.CallerId, value.CalleeId, value.Domain);

    public static implicit operator UmlTransitionDto((string Caller, string Callee, string Domain) value) => new() { CallerId = value.Caller, CalleeId = value.Callee, Domain = value.Domain };

    public override int GetHashCode() => HashCode.Combine(CallerId, CalleeId, Domain);

    public bool Equals(UmlTransitionDto other) =>
        string.Equals(CallerId, other.CallerId, StringComparison.OrdinalIgnoreCase) &&
        string.Equals(CalleeId, other.CalleeId, StringComparison.OrdinalIgnoreCase) &&
        string.Equals(Domain, other.Domain, StringComparison.OrdinalIgnoreCase);
}
