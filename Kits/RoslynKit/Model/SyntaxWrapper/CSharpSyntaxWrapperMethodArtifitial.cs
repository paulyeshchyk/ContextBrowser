using ContextKit.Model;
using SemanticKit.Model.Signature;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Model.SyntaxWrapper;

public record CSharpSyntaxWrapperMethodArtifitial : ISyntaxWrapper
{
    public string Name { get; set; }

    public string FullName { get; set; }

    public string Namespace { get; set; }

    public int SpanEnd { get; set; }

    public int SpanStart { get; set; }

    public string Identifier { get; set; }

    public string ShortName { get; set; }

    public bool IsValid { get; set; } = true;

    public ISignature? Signature { get; set; }

    public IContextInfo ContextOwner { get; set; }

#warning incorrect mapping for void Console.Writeline(string? value)
    public CSharpSyntaxWrapperMethodArtifitial(ISyntaxWrapper wrapper, IContextInfo contextOwner)
    {
        ContextOwner = contextOwner;
        Identifier = wrapper.Identifier;
        Name = wrapper.Name;
        FullName = wrapper.FullName;
        SpanStart = wrapper.SpanStart;
        SpanEnd = wrapper.SpanEnd;
        Namespace = wrapper.Namespace;
        ShortName = wrapper.ShortName;
        if (wrapper.Signature is not null)
        {
            Signature = new SignatureDefault
            (
                ResultType: wrapper.Signature.ResultType,
                Namespace: wrapper.Signature.Namespace,
                ClassName: wrapper.Signature.ClassName,
                MethodName: wrapper.Signature.MethodName,
                Arguments: wrapper.Signature.Arguments,
                Raw: wrapper.Signature.Raw);
        }
    }

    public IContextInfo GetContextInfoDto()
    {
        var result = new ContextInfoDto(
            elementType: ContextInfoElementType.method,
               fullName: this.FullName,
                   name: this.Name,
              shortName: this.ShortName,
              nameSpace: this.Namespace,
             identifier: this.FullName,
              spanStart: this.SpanStart,
                spanEnd: this.SpanEnd);

        result.ClassOwner = ContextOwner;
        result.MethodOwner = result;// делаем себя же владельцем метода

        return result;
    }
}