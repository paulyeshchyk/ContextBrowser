namespace SemanticKit.Model;

public interface ISyntaxWrapper
{
    string FullName { get; }

    string Name { get; }

    string Namespace { get; }

    int SpanEnd { get; }

    int SpanStart { get; }
}