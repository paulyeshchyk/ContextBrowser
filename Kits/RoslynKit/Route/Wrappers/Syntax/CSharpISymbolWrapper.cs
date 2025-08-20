using ContextKit.Model;
using Microsoft.CodeAnalysis;
using RoslynKit.Extensions;

namespace RoslynKit.Route.Wrappers.Syntax;

public class CSharpISymbolWrapper : ISymbolInfo
{
    private readonly ISymbol? _symbol;

    public CSharpISymbolWrapper(object? symbol)
    {
        if(symbol is ISymbol isymbol)
        {
            _symbol = isymbol;
        }
        else
        {
            if(symbol is null)
            {
                return;
            }

            throw new Exception($"symbol is not isymbol ({symbol})");
        }
    }

    public string GetNameSpace() => _symbol?.GetNameSpace() ?? "not_defined_name";

    public string GetName() => _symbol?.Name ?? "not_defined_name";

    public string GetFullName() => _symbol?.ToDisplayString() ?? "not_defined_fullname";

    public string GetShortestName() => _symbol?.GetShortestName() ?? " not_defined";

    public string ToDisplayString() => _symbol?.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat) ?? " not_defined";
}
