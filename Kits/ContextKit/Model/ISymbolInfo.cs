namespace ContextKit.Model;

// context: syntax, model, symbol
public interface ISymbolInfo
{
    string Identifier { get; }

    string Namespace { get; }

    string GetShortName();

    string GetName();

    string GetFullName();

    S GetCoSyntax<S>();

    void SetSyntax(object syntax);
}