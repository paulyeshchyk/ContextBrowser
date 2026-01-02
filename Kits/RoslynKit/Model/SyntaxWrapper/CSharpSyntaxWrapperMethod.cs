using System;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Signature.SignatureBuilder;
using SemanticKit.Model.Signature;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Model.SyntaxWrapper;

public record CSharpSyntaxWrapperMethod : ISyntaxWrapper
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

    public CSharpSyntaxWrapperMethod(object symbol, MethodDeclarationSyntax syntax)
    {
        if (symbol is ISymbol isymbol)
        {
            ShortName = isymbol.BuildShortName();
            Identifier = isymbol.BuildFullMemberName();
            Name = isymbol.BuildNameAndClassOwnerName();
            FullName = isymbol.BuildFullMemberName();
            SpanStart = syntax.Span.Start;
            SpanEnd = syntax.Span.End;
            Namespace = isymbol.GetNamespaceOrGlobal();
        }
        else
        {
            throw new Exception("symbol is not isymbol");
        }
    }

    public CSharpSyntaxWrapperMethod(ISyntaxWrapper wrapper)
    {
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