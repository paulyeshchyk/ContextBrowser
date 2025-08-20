using ContextKit.Model;
using Microsoft.CodeAnalysis;

namespace RoslynKit.Route.Wrappers.Syntax;

public class CSharpSyntaxNodeWrapper : ISyntaxNodeInfo
{
    private readonly SyntaxNode? _syntaxNode;

    public CSharpSyntaxNodeWrapper(SyntaxNode? syntaxNode)
    {
        _syntaxNode = syntaxNode;
    }

    public IOrderedEnumerable<T> DescendantNodes<T>() where T : class
    {
        return _syntaxNode?
            .DescendantNodes()
            .OfType<T>()
            .OrderBy(c => (c as SyntaxNode)?.SpanStart ?? -1) ?? Enumerable.Empty<T>().OrderBy(t => -1);
    }
}