using System;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.AWrappers;
using RoslynKit.Signature;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Syntax;

public record CSharpTypeSyntaxWrapper : ISyntaxWrapper
{
    public string Name { get; set; }

    public string Namespace { get; set; }

    public int SpanEnd { get; private set; }

    public int SpanStart { get; private set; }

    public string FullName { get; set; }

    public string Identifier { get; set; }

    public bool IsPartial { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string ShortName { get; set; }

    public bool IsValid { get; set; } = true;

    public ICustomMethodSignature? Signature { get; set; }

    public CSharpTypeSyntaxWrapper(ISymbol symbol, TypeDeclarationSyntax syntax)
    {
        Identifier = symbol.GetFullMemberName(includeParams: true);
        Name = symbol.GetNameAndClassOwnerName();
        FullName = symbol.GetFullMemberName(includeParams: true);
        SpanStart = syntax.Span.Start;
        SpanEnd = syntax.Span.End;
        Namespace = symbol.GetNamespaceOrGlobal();
        ShortName = symbol.GetShortName();
    }

    public CSharpTypeSyntaxWrapper(ISyntaxWrapper syntaxWrapper, int spanStart, int spanEnd)
    {
        SpanStart = spanStart;
        SpanEnd = spanEnd;

        if (syntaxWrapper.Signature is not null)
        {
            Name = syntaxWrapper.Signature.ClassName;
            ShortName = syntaxWrapper.Signature.ClassName;
            Namespace = syntaxWrapper.Signature.Namespace;
            Identifier = $"{syntaxWrapper.Signature.Namespace}.{syntaxWrapper.Signature.ClassName}";
            FullName = $"{syntaxWrapper.Signature.Namespace}.{syntaxWrapper.Signature.ClassName}";

            Signature = new CSharpMethodSignature
            (
                ResultType: syntaxWrapper.Signature.ResultType,
                Namespace: syntaxWrapper.Signature.Namespace,
                ClassName: syntaxWrapper.Signature.ClassName,
                MethodName: syntaxWrapper.Signature.MethodName,
                Arguments: syntaxWrapper.Signature.Arguments,
                Raw: syntaxWrapper.Signature.Raw)
            {
            };
        }
        else
        {
            var parsedSignature = SignatureUtils.Parse(syntaxWrapper.FullName);

            FullName = $"{parsedSignature.Namespace}.{parsedSignature.ClassName}";
            Identifier = $"{parsedSignature.Namespace}.{parsedSignature.ClassName}";
            ShortName = parsedSignature.ClassName;
            Name = parsedSignature.ClassName;
            Namespace = parsedSignature.Namespace;
        }
    }

    public IContextInfo GetContextInfoDto()
    {
        return new ContextInfoDto(
            elementType: ContextInfoElementType.@class,
               fullName: FullName,
                   name: Name,
              shortName: ShortName,
              nameSpace: Namespace,
             identifier: FullName,
              spanStart: SpanStart,
                spanEnd: SpanEnd);
    }
}