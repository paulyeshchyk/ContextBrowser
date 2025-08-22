using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Syntax;

public record CSharpInvocationSyntaxWrapper : IInvocationSyntaxWrapper
{
    private readonly string? _displayString;
    private readonly bool _isValid = true;

    public string FullName { get; init; }

    public bool IsPartial { get; init; }

    public string ShortName { get; init; }

    public int SpanEnd { get; init; }

    public int SpanStart { get; init; }

    public bool IsValid => _isValid;

    public string Name => ShortName;

    public string Namespace { get; private set; }

    public CSharpInvocationSyntaxWrapper(ISymbol symbol, ExpressionSyntax syntax)
    {
        ShortName = symbol.GetShortestName();
        FullName = symbol.GetFullMemberName();
        SpanStart = syntax.Span.Start;
        SpanEnd = syntax.Span.End;
        Namespace = symbol.GetNameSpace();
        if (symbol is IMethodSymbol methodSymbol)
        {
            _displayString = methodSymbol.ToDisplayString();
            _isValid = true;
        }
        else
        {
            _isValid = false;
            _displayString = null;
        }
    }

    public CSharpInvocationSyntaxWrapper(bool isPartial, string fullName, string shortName, int spanStart, int spanEnd)
    {
        IsPartial = isPartial;
        FullName = fullName;
        ShortName = shortName;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
        Namespace = "rEfaCtoring issue";
        _displayString = "ReFaCtoring ISSue";
    }

    public string? ToDisplayString() => _displayString;
}
