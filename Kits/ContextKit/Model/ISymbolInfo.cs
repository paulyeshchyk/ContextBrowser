namespace ContextKit.Model;

public interface ISymbolInfo
{
    string Identifier { get; }

    string Namespace { get; }

    string GetShortName();

    string GetName();

    string GetFullName();

    object? GetSyntax();

    void SetSyntax(object? syntax);
}

public interface ISyntaxNodeWrapper : ISpanInfo
{
    IOrderedEnumerable<T> DescendantSyntaxNodes<T>()
        where T : class;

    string Identifier { get; }

    string Namespace { get; }

    string GetShortName();

    string GetName();

    string GetFullName();

    object? GetSyntax();

    void SetSyntax(object? syntax);
}