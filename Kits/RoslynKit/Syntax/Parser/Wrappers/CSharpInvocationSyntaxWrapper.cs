using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Syntax.Parser.Wrappers;

public record CSharpInvocationSyntaxWrapper
{
    public string FullName { get; init; }

    public bool IsPartial { get; init; }

    public string ShortName { get; init; }

    public int SpanEnd { get; init; }

    public int SpanStart { get; init; }

    public ISymbol? Symbol { get; private set; }

    public ExpressionSyntax? Syntax { get; private set; }

    public CSharpInvocationSyntaxWrapper(ISymbol symbol, ExpressionSyntax syntax)
    {
        Symbol = symbol;
        Syntax = syntax;


        ShortName = Symbol.GetShortestName();
        FullName = Symbol.GetFullMemberName();
        SpanStart = Syntax.Span.Start;
        SpanEnd = Syntax.Span.End;
    }

    public CSharpInvocationSyntaxWrapper(bool isPartial, string fullName, string shortName, int spanStart, int spanEnd)
    {
        IsPartial = isPartial;
        FullName = fullName;
        ShortName = shortName;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
    }
}
