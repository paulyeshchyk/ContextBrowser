using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Model.Wrappers;

public record MethodSyntaxWrapper
{
    public string MethodName { get; private set; }

    public string MethodFullName { get; private set; }

    public int SpanStart { get; private set; }

    public int SpanEnd { get; private set; }

    public ISymbol? Symbol;

    public MethodDeclarationSyntax? Syntax;

    public MethodSyntaxWrapper(ISymbol symbol, MethodDeclarationSyntax syntax)
    {
        Symbol = symbol;
        Syntax = syntax;

        MethodName = Syntax.Identifier.Text;
        MethodFullName = Symbol.GetFullMemberName();
        SpanStart = Syntax.Span.Start;
        SpanEnd = Syntax.Span.End;
    }

    public MethodSyntaxWrapper(ISymbol? symbol, string methodName, int spanStart, int spanEnd)
    {
        Symbol = symbol;
        Syntax = null;

        MethodName = methodName;
        MethodFullName = Symbol?.GetFullMemberName() ?? methodName;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
    }

    public MethodSyntaxWrapper(string methodName, string methodFullName, int spanStart, int spanEnd)
    {
        MethodName = methodName;
        MethodFullName = methodFullName;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
    }
}