namespace SemanticKit.Model;

public interface IInvocationSyntaxWrapper : ISyntaxWrapper
{
    bool IsPartial { get; init; }

    string ShortName { get; init; }

    bool IsValid { get; }

    public string? ToDisplayString();
}