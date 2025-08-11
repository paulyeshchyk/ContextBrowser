using Microsoft.CodeAnalysis;

namespace ContextKit.Model.Wrappers.Roslyn;

public class SyntaxNodeWrapper : ISyntaxNodeInfo
{
    private readonly SyntaxNode? _syntaxNode;

    public SyntaxNodeWrapper(SyntaxNode? syntaxNode)
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