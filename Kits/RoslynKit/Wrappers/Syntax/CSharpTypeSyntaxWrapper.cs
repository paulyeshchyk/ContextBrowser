using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Syntax;

public record CSharpTypeSyntaxWrapper : BaseSyntaxWrapper
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

    public CSharpTypeSyntaxWrapper(string identifier, string name, string fullName, int spanStart, int spanEnd, string nameSpace)
    {
        Identifier = identifier;
        Name = name;
        FullName = fullName;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
        Namespace = nameSpace;
        ShortName = name;
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

    public string? ToDisplayString()
    {
        throw new NotImplementedException();
    }
}