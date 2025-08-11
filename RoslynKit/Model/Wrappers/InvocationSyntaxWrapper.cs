using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Parser.Phases;

public record InvocationSyntaxWrapper
{
    public bool IsPartial { get; init; }

    public string FullName { get; init; }

    public string ShortName { get; init; }

    public int SpanStart { get; init; }

    public int SpanEnd { get; init; }

    public ISymbol? Symbol { get; private set; }

    public InvocationExpressionSyntax? Syntax { get; private set; }

    public InvocationSyntaxWrapper(ISymbol symbol, InvocationExpressionSyntax syntax)
    {
        Symbol = symbol;
        Syntax = syntax;


        ShortName = Symbol.GetShortestName();
        FullName = Symbol.GetFullMemberName();
        SpanStart = Syntax.Span.Start;
        SpanEnd = Syntax.Span.End;
    }

    public InvocationSyntaxWrapper(bool isPartial, string fullName, string shortName, int spanStart, int spanEnd)
    {
        IsPartial = isPartial;
        FullName = fullName;
        ShortName = shortName;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
    }
}
