using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Syntax.Parser.Wrappers;

public record CSharpTypeSyntaxWrapper
{
    public string Name { get; private set; }

    public string Namespace { get; private set; }

    public int SpanEnd { get; private set; }

    public int SpanStart { get; private set; }

    public ISymbol? Symbol { get; private set; }

    public string FullName { get; private set; }

    public TypeDeclarationSyntax? Syntax { get; private set; }

    public CSharpTypeSyntaxWrapper(ISymbol symbol, TypeDeclarationSyntax syntax)
    {
        Symbol = symbol;
        Syntax = syntax;
        Name = syntax.GetDeclarationName();
        FullName = symbol.GetFullMemberName();
        SpanStart = syntax.Span.Start;
        SpanEnd = syntax.Span.End;
        Namespace = syntax.GetNamespaceName();
    }

    public CSharpTypeSyntaxWrapper(string name, string fullName, int spanStart, int spanEnd, string nameSpace)
    {
        Name = name;
        FullName = fullName;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
        Namespace = nameSpace;
    }

    public IContextInfo GetContextInfoDto()
    {
        return new ContextInfoDto(ContextInfoElementType.@class, Name, FullName, Namespace, $"{Name}", SpanStart, SpanEnd);
    }
}