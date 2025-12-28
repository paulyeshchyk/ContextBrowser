using System;
using System.Linq;
using ContextKit.Model;
using Microsoft.CodeAnalysis;

namespace RoslynKit.Wrappers.Syntax;

public abstract class CSharpSyntaxNodeWrapper<S> : ISyntaxNodeWrapper
    where S : notnull, SyntaxNode
{
    private S? _syntaxNode = null;

    public int SpanStart => GetCoSyntax<S>().Span.Start;

    public int SpanEnd => GetCoSyntax<S>().Span.End;

    public IOrderedEnumerable<T> DescendantSyntaxNodes<T>()
        where T : class
    {
        return GetCoSyntax<S>()
            .DescendantNodes()
            .OfType<T>()
            .OrderBy(c => (c as SyntaxNode)?.SpanStart ?? -1) ?? Enumerable.Empty<T>().OrderBy(t => -1);
    }

    public abstract string Identifier { get; }

    public abstract string Namespace { get; }

    public abstract string GetFullName();

    public abstract string GetName();

    public abstract string GetShortName();

    public void SetSyntax(object syntax)
    {
        if (syntax is not S coSyntax)
        {
            throw new Exception($"incorrect syntax was set: expected{typeof(S)}");
        }
        _syntaxNode = coSyntax;
    }

    public object GetSyntax() => GetCoSyntax<S>();

    public S1 GetCoSyntax<S1>()
    {
        if (_syntaxNode is not S1 coSyntax)
        {
            throw new Exception($"incorrect syntax was set: expected{typeof(S)}");
        }
        return coSyntax;
    }
}