using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;
using SemanticKit.Model;

namespace RoslynKit.Wrappers.Syntax;

public record CSharpInvocationSyntaxWrapper : BaseSyntaxWrapper
{
    public string FullName { get; set; }

    public bool IsPartial { get; set; }

    public string ShortName { get; set; }

    public int SpanEnd { get; init; }

    public int SpanStart { get; init; }

    public bool IsValid { get; private set; }

    public string Name { get; set; }

    public string Namespace { get; set; }

    public string Identifier { get; set; }

    public CSharpInvocationSyntaxWrapper(ISymbol isymbol, ExpressionSyntax syntax)
    {
        ShortName = isymbol.GetShortName();
        FullName = isymbol.GetFullMemberName(includeParams: true);
        Name = isymbol.GetNameAndClassOwnerName();
        Namespace = isymbol.GetNamespaceOrGlobal();
        Identifier = isymbol.GetFullMemberName(includeParams: true);

        SpanStart = syntax.Span.Start;
        SpanEnd = syntax.Span.End;
        if (isymbol is IMethodSymbol methodSymbol)
        {
            IsValid = true;
        }
        else
        {
            IsValid = false;
        }
    }

    public CSharpInvocationSyntaxWrapper(bool isPartial, string fullName, string shortName, int spanStart, int spanEnd, string nameSpace)
    {
        IsPartial = isPartial;
        FullName = fullName;
        Identifier = fullName;
        ShortName = shortName;
        Name = shortName;
        SpanStart = spanStart;
        SpanEnd = spanEnd;
        Namespace = nameSpace;
        IsValid = true;
    }
}
