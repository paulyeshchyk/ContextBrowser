using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Basics.Semantic;
using RoslynKit.Extensions;

namespace RoslynKit.Syntax.Parser.Wrappers;

public record CSharpMethodSyntaxWrapper
{
    public string Name { get; private set; }

    public string FullName { get; private set; }

    public string Namespace { get; private set; }

    public int SpanEnd { get; private set; }

    public int SpanStart { get; private set; }

    public ISymbol? Symbol { get; private set; }

    public MethodDeclarationSyntax? Syntax { get; private set; }


    public CSharpMethodSyntaxWrapper(ISymbol symbol, MethodDeclarationSyntax syntax)
    {
        Symbol = symbol;
        Syntax = syntax;

        Name = Syntax.Identifier.Text;
        FullName = Symbol.GetFullMemberName();
        SpanStart = Syntax.Span.Start;
        SpanEnd = Syntax.Span.End;
        Namespace = Syntax.GetNamespaceName();
    }

    public CSharpMethodSyntaxWrapper(ISymbol? symbol, string name, int spanStart, int spanEnd, string nameSpace)
    {
        Symbol = symbol;
        Syntax = null;

        Name = name;
        FullName = Symbol?.GetFullMemberName() ?? name;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
        Namespace = nameSpace;
    }

    public ISymInfoLoader GetSymInfoDto()
    {
        return new SymInfoDto(ContextInfoElementType.method, FullName, Name, Namespace, $"{FullName}", SpanStart, SpanEnd, null, null);
    }
}
