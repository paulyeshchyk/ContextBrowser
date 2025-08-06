using ContextKit.Model;
using Microsoft.CodeAnalysis;

namespace RoslynKit.Syntax.Parser.ContextInfo;

public interface IContextCreationStrategy<TContext>
    where TContext : IContextWithReferences<TContext>
{
    ContextInfoElementType SupportedElementType { get; }

    TContext Create(
        TContext? parent,
        string nsName,
        string name,
        string? fullName,
        SyntaxNode? syntaxNode,
        int spanStart,
        int spanEnd,
        ISymbol? symbol);
}

public class TypeContextCreationStrategy<TContext> : IContextCreationStrategy<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public ContextInfoElementType SupportedElementType => ContextInfoElementType.@class;

    protected readonly IContextFactory<TContext>? _factory;

    public TContext Create(TContext? parent, string nsName, string name, string? fullName, SyntaxNode? syntaxNode, int spanStart, int spanEnd, ISymbol? symbol)
    {
        return _factory.Create(
                    parent,
                    SupportedElementType,
                    nsName,
                    name,
                    fullName,
                    syntaxNode,
                    spanStart,
                    spanEnd,
                    symbol);
    }
}

public class MethodContextCreationStrategy<TContext> : IContextCreationStrategy<TContext>
    where TContext : IContextWithReferences<TContext>
{
    public ContextInfoElementType SupportedElementType => ContextInfoElementType.method;

    protected readonly IContextFactory<TContext>? _factory;

    public TContext Create(TContext? parent, string nsName, string name, string? fullName, SyntaxNode? syntaxNode, int spanStart, int spanEnd, ISymbol? symbol)
    {
        return _factory.Create(
                    parent,
                    SupportedElementType,
                    nsName,
                    name,
                    fullName,
                    syntaxNode,
                    spanStart,
                    spanEnd,
                    symbol);
    }
}