using ContextKit.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynKit.Extensions;

namespace RoslynKit.Wrappers.Syntax;

public abstract class CSharpSyntaxNodeWrapper<S> : ISyntaxNodeWrapper
    where S : notnull, SyntaxNode
{
    protected S? SyntaxNode;

    public int SpanStart => SyntaxNode?.Span.Start ?? -1;

    public int SpanEnd => SyntaxNode?.Span.End ?? -1;


    public IOrderedEnumerable<T> DescendantSyntaxNodes<T>()
        where T : class
    {
        return SyntaxNode?
            .DescendantNodes()
            .OfType<T>()
            .OrderBy(c => (c as SyntaxNode)?.SpanStart ?? -1) ?? Enumerable.Empty<T>().OrderBy(t => -1);
    }

    public abstract string Identifier { get; }

    public abstract string Namespace { get; }

    public abstract string GetFullName();

    public abstract string GetName();

    public abstract string GetShortName();

    public object? GetSyntax() => SyntaxNode;

    public void SetSyntax(object? syntax)
    {
        SyntaxNode = (S?)syntax;
    }
}