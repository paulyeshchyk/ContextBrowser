using Microsoft.CodeAnalysis;

namespace RoslynKit.Extensions;

public static class ISymbolExtensions
{
    public static string GetShortestName(this ISymbol symbol)
    {
        var format = new SymbolDisplayFormat(
                    globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Omitted,
                    typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
                    propertyStyle: SymbolDisplayPropertyStyle.NameOnly,
                    localOptions: SymbolDisplayLocalOptions.None,
                    parameterOptions: SymbolDisplayParameterOptions.None,
                    genericsOptions: SymbolDisplayGenericsOptions.None,
                    memberOptions: SymbolDisplayMemberOptions.IncludeContainingType | SymbolDisplayMemberOptions.IncludeExplicitInterface,
                    miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

        return symbol.ToDisplayString(format);
    }

    public static string GetMinimallyQualifiedName(this ISymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
    }


    public static string GetFullMemberName(this ISymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
    }
}
