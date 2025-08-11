using Microsoft.CodeAnalysis;
using RoslynKit.Extensions;

namespace ContextKit.Model.Wrappers.Roslyn;

public class SymbolWrapper : ISymbolInfo
{
    private readonly ISymbol? _symbol;

    public SymbolWrapper(ISymbol? symbol)
    {
        _symbol = symbol;
    }

    public string GetShortestName()
    {
        return _symbol?.GetShortestName() ?? " not_defined";
    }

    public string ToDisplayString()
    {
        return _symbol?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat) ?? " not_defined";
    }
}
