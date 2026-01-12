using System;
using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Signature;
using RoslynKit.Signature.SignatureBuilder;
using SemanticKit.Model.Signature;
using SemanticKit.Model.SyntaxWrapper;

namespace RoslynKit.Model.SyntaxWrapper;

public record CSharpSyntaxWrapperType : ISyntaxWrapper
{
    public string Name { get; set; }

    public string Namespace { get; set; }

    public int SpanEnd { get; set; }

    public int SpanStart { get; set; }

    public string FullName { get; set; }

    public string Identifier { get; set; }

    public string ShortName { get; set; }

    public bool IsValid { get; set; } = true;

    public ISignature? Signature { get; set; }

    public CSharpSyntaxWrapperType(string name, string @namespace, int spanEnd, int spanStart, string fullName, string identifier, string shortName, bool isValid, ISignature? signature = null)
    {
        Name = name;
        Namespace = @namespace;
        SpanEnd = spanEnd;
        SpanStart = spanStart;
        FullName = fullName;
        Identifier = identifier;
        ShortName = shortName;
        IsValid = isValid;
        Signature = signature;
    }

    public IContextInfo GetContextInfoDto()
    {
        return new ContextInfoDto(
            elementType: ContextInfoElementType.@class,
      elementVisibility: ContentInfoElementVisibility.@public,
               fullName: FullName,
                   name: Name,
              shortName: ShortName,
              nameSpace: Namespace,
             identifier: FullName,
              spanStart: SpanStart,
                spanEnd: SpanEnd);
    }
}