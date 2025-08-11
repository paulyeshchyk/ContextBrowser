namespace ContextKit.Model;

public interface ISymbolInfo
{
    string ToDisplayString();

    string GetShortestName();
}


public interface ISyntaxNodeInfo
{
    IOrderedEnumerable<T> DescendantNodes<T>() where T : class;
}
