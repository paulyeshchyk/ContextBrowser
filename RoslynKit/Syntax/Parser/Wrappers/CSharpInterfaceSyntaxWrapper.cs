using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Syntax.Parser.Wrappers;

public record CSharpInterfaceSyntaxWrapper
{
    public string FullName { get; private set; }

    public string Name { get; private set; }

    public string Namespace { get; private set; }

    public int SpanEnd { get; private set; }

    public int SpanStart { get; private set; }

    public ISymbol? Symbol { get; private set; }

    public InterfaceDeclarationSyntax? Syntax { get; private set; }

    public CSharpInterfaceSyntaxWrapper(ISymbol symbol, InterfaceDeclarationSyntax syntax)
    {
        Symbol = symbol;
        Syntax = syntax;

        Name = Syntax.Identifier.Text;
        FullName = Symbol.GetFullMemberName();
        SpanStart = Syntax.Span.Start;
        SpanEnd = Syntax.Span.End;
        Namespace = Syntax.GetNamespaceName();
    }

    public CSharpInterfaceSyntaxWrapper(ISymbol? symbol, string methodName, int spanStart, int spanEnd, string nameSpace)
    {
        Symbol = symbol;
        Syntax = null;

        Name = methodName;
        FullName = Symbol?.GetFullMemberName() ?? methodName;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
        Namespace = nameSpace;
    }

    public CSharpInterfaceSyntaxWrapper(string methodName, string methodFullName, int spanStart, int spanEnd, string nameSpace)
    {
        Name = methodName;
        FullName = methodFullName;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
        Namespace = nameSpace;
    }
}