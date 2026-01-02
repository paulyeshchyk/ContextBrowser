using ContextKit.Model;
using SemanticKit.Model.Signature;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Model.SyntaxWrapper;

public record CSharpSyntaxWrapperInvocation : ISyntaxWrapper
{
    public int SpanEnd { get; init; }

    public int SpanStart { get; init; }

    public bool IsValid { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string ShortName { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Namespace { get; init; } = string.Empty;

    public string Identifier { get; init; } = string.Empty;

    public ISignature? Signature { get; init; }

    public IContextInfo GetContextInfoDto()
    {
        return new ContextInfoDto(
            elementType: ContextInfoElementType.method,
               fullName: this.FullName,
                   name: this.Name,
              shortName: this.ShortName,
              nameSpace: this.Namespace,
             identifier: this.FullName,
              spanStart: this.SpanStart,
                spanEnd: this.SpanEnd);
    }
}
