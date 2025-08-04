namespace ContextBrowser.DiagramFactory.Model;

public readonly record struct UmlTransitionDto
{
    public UmlTransitionDto(string? callerId, string? callerName, string? callerClassName, string? callerMethod, string? calleeId, string? calleeName, string? calleeClassName, string? calleeMethod, string? domain, string? runContext, string? ownerClass, string? ownerMethod)
    {
        CallerId = callerId;
        CallerName = callerName;
        CallerClassName = callerClassName;
        CallerMethod = callerMethod;
        CalleeId = calleeId;
        CalleeName = calleeName;
        CalleeClassName = calleeClassName;
        CalleeMethod = calleeMethod;
        Domain = domain;
        RunContext = runContext;
        OwnerClass = ownerClass;
        OwnerMethod = ownerMethod;
    }

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

    public string? OwnerClass { get; init; }

    public string? OwnerMethod { get; init; }

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