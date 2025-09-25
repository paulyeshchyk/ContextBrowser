using SemanticKit.Model;

namespace RoslynKit.Wrappers.Syntax;

public record CSharpInvocationSyntaxWrapper : ISyntaxWrapper
{
    public bool IsPartial { get; set; } = false;

    public int SpanEnd { get; set; } = 0;

    public int SpanStart { get; set; } = 0;

    public bool IsValid { get; set; } = false;

    public string FullName { get; set; } = string.Empty;

    public string ShortName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Namespace { get; set; } = string.Empty;

    public string Identifier { get; set; } = string.Empty;

    public ICustomMethodSignature? Signature { get; set; } = null;

    public CSharpInvocationSyntaxWrapper()
    {
    }
}
