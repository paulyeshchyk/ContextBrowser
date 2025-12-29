using SemanticKit.Model;
using SemanticKit.Model.Signature;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Wrappers.Syntax;

public record CSharpInvocationSyntaxWrapper : ISyntaxWrapper
{
    public bool IsPartial { get; init; }

    public int SpanEnd { get; init; }

    public int SpanStart { get; init; }

    public bool IsValid { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string ShortName { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Namespace { get; init; } = string.Empty;

    public string Identifier { get; init; } = string.Empty;

    public ISignature? Signature { get; init; }

}
