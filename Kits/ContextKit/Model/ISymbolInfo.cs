namespace ContextKit.Model;

public interface ISymbolInfo
{
    string ToDisplayString();

    string GetShortestName();

    string GetNameSpace();

    string GetName();

    string GetFullName();
}


public interface ISyntaxNodeInfo
{
    IOrderedEnumerable<T> DescendantNodes<T>() where T : class;
}
